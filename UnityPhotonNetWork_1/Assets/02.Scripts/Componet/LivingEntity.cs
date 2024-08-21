using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LivingEntity : MonoBehaviourPun,IDamageable
{
    public float startingHealth = 100f;          // 시작 체력
    public float health { get; protected set; }  // 현재 체력
    public bool dead { get; protected set; }     // 사망 상태
    public event Action onDeath;                 // 사망시 발동 할 이벤트

    [PunRPC]  // 호스트(방장) -> 모든 클라이언트 순서 방향으로 사망 상태를 동기화 하는 메서드
    public void ApplyUpdateHealth(float newhealth, bool newDead)
    {
        health = newhealth;
        dead = newDead;
    }

    // 생명체가 활성화 될때 상태를 리셋시킴
    protected virtual void OnEnable()
    {// virtual : 물려받을 가상 메서드
        dead = false;  // 사망하지 않은 상태로
        health = startingHealth;  // 체력을 시작 체력으로 초기화
    }

    // 데미지 처리
    [PunRPC]  // 호스트에서 단독 실행 되고 호스트를 통해 다른 클라이언트에서 일괄 실행된다.
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {// 호스트 방장
        if (PhotonNetwork.IsMasterClient)
        {
            health -= damage;   // 데미지 만큼 체력 감소
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);
            // 호스트에서 클라이언트로 동기화
            photonView.RPC("Ondamage", RpcTarget.Others, damage, hitPoint, hitNormal);
        }
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    // 체력을 회복하는 기능
    [PunRPC]
    public virtual void RestoreHealth(float newHealth)
    {
        if (dead)
        {// 이미 죽었다면 체력을 회복 할 수 없다.
            return;
        }
        if (PhotonNetwork.IsMasterClient)
        {// 호스트 인경우에만 체력이 증가 되는 데
            health += newHealth;
            // 서버에서 클라이너트로 동기화
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);
            // 다른 클라이언트도 RestoreHealth를 실행 하도록 함
            photonView.RPC("RestoreHealth", RpcTarget.Others, newHealth);
        }
    }

    [PunRPC]
    public virtual void Die()
    {
        if (onDeath != null)
        {
            onDeath();
        }
        dead = true;
    }
}
// LivingEntity 클래스는 IDamageable을 상속하므로 OnDamage()
// 메서드를 반드시 구현 해야한다.
