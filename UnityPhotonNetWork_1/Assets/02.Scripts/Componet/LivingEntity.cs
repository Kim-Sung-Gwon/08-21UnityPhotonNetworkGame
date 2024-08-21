using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LivingEntity : MonoBehaviourPun,IDamageable
{
    public float startingHealth = 100f;          // ���� ü��
    public float health { get; protected set; }  // ���� ü��
    public bool dead { get; protected set; }     // ��� ����
    public event Action onDeath;                 // ����� �ߵ� �� �̺�Ʈ

    [PunRPC]  // ȣ��Ʈ(����) -> ��� Ŭ���̾�Ʈ ���� �������� ��� ���¸� ����ȭ �ϴ� �޼���
    public void ApplyUpdateHealth(float newhealth, bool newDead)
    {
        health = newhealth;
        dead = newDead;
    }

    // ����ü�� Ȱ��ȭ �ɶ� ���¸� ���½�Ŵ
    protected virtual void OnEnable()
    {// virtual : �������� ���� �޼���
        dead = false;  // ������� ���� ���·�
        health = startingHealth;  // ü���� ���� ü������ �ʱ�ȭ
    }

    // ������ ó��
    [PunRPC]  // ȣ��Ʈ���� �ܵ� ���� �ǰ� ȣ��Ʈ�� ���� �ٸ� Ŭ���̾�Ʈ���� �ϰ� ����ȴ�.
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {// ȣ��Ʈ ����
        if (PhotonNetwork.IsMasterClient)
        {
            health -= damage;   // ������ ��ŭ ü�� ����
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);
            // ȣ��Ʈ���� Ŭ���̾�Ʈ�� ����ȭ
            photonView.RPC("Ondamage", RpcTarget.Others, damage, hitPoint, hitNormal);
        }
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    // ü���� ȸ���ϴ� ���
    [PunRPC]
    public virtual void RestoreHealth(float newHealth)
    {
        if (dead)
        {// �̹� �׾��ٸ� ü���� ȸ�� �� �� ����.
            return;
        }
        if (PhotonNetwork.IsMasterClient)
        {// ȣ��Ʈ �ΰ�쿡�� ü���� ���� �Ǵ� ��
            health += newHealth;
            // �������� Ŭ���̳�Ʈ�� ����ȭ
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);
            // �ٸ� Ŭ���̾�Ʈ�� RestoreHealth�� ���� �ϵ��� ��
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
// LivingEntity Ŭ������ IDamageable�� ����ϹǷ� OnDamage()
// �޼��带 �ݵ�� ���� �ؾ��Ѵ�.
