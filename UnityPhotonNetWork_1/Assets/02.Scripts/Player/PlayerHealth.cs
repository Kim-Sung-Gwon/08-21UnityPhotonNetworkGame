using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHealth : LivingEntity
{
    public Slider healthSlider;        // 체력을 표시할 슬라이더
    public AudioClip hitClip;         // 피격 소리
    public AudioClip itemPickupClip;  // 아이템 줍는 소리
    public AudioClip deahtClip;
    private AudioSource playerAudioPlayer;  // 플레이어 소리 재생기
    private Animator playerAnimator;  // 플레이어 애니메이터
    private PlayerMovement playerMovement;
    private PlayerShooter playerShooter;

    private readonly int hashDie = Animator.StringToHash("DieTrigger");

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAudioPlayer = GetComponent<AudioSource>();
        playerShooter = GetComponent<PlayerShooter>();
    }

    protected override void OnEnable()  // 오버라이드 다르게 쓸 예정
    {
        base.OnEnable();  // 부모 클래스의 이벤트 함수
        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = startingHealth;
        healthSlider.value = health;
        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }

    // 체력 회복
    [PunRPC]
    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
        healthSlider.value = newHealth;
        // 업데이트된 체력
    }

    // 데미지 처리
    [PunRPC]
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (!dead)
        {
            playerAudioPlayer.PlayOneShot(hitClip);
            // 사망하지 않은 경우에만 효과음 재생
        }
        // LivingEnity의 OnDamage 실행해서 데미지 적용
        base.OnDamage(damage, hitPoint, hitDirection);
        healthSlider.value = health;
    }

    public override void Die()
    {
        base.Die();
        healthSlider.gameObject.SetActive(false);
        playerAudioPlayer.PlayOneShot(deahtClip, 1.0f);
        playerAnimator.SetTrigger(hashDie);
        playerMovement.enabled = false;
        playerShooter.enabled = false;
        Invoke("ReSpawn", 5.0f);  // 5.0초 후에 리스폰
    }

    public void ReSpawn()  // 플레이어가 사망후 5초 후에 부활
    {
        if (photonView.IsMine)  // 로컬 플레이만 직접 위치 변경
        {
            // 원점에서 반경 5 유닛 내부의 랜덤 위치 지정
            Vector3 randomSpawnPos = Random.insideUnitSphere * 5.0f;
            randomSpawnPos.y = 0f;  // 랜덤 위치 값을 0으로 변경
            // 지정된 위치로 이동
            transform.position = randomSpawnPos;
        }
        gameObject.SetActive(false);  // OnDisable 호출하기 위해
        gameObject.SetActive(true);   // OnEnable 호출하기 위해
    }

    private void OnTriggerEnter(Collider other)
    {
        // 아이템과 충돌한 경우 해당 아이템을 사용하는 처리
        if (!dead)
        {   // 충돌한 상대방으로 부터 IItem 컴퍼넌트를 가지고 온다.
            IItem item = other.GetComponent<IItem>();
            if (item != null)
            {//    호스트만 아이템 사용 가능
                // 호스트에서 아이템 사용후 사용된 효과를 모든 클라이언트에 동기화 시킴
                if (PhotonNetwork.IsMasterClient)
                {
                    // Use 메서드를 실행하여 아이템 사용
                    item.Use(gameObject);
                    // 아이템 습득 소리 재생
                }
                playerAudioPlayer.PlayOneShot(itemPickupClip, 1.0f);
            }
        }
    }
}
