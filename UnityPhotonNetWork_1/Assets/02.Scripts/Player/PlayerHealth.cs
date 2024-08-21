using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHealth : LivingEntity
{
    public Slider healthSlider;        // ü���� ǥ���� �����̴�
    public AudioClip hitClip;         // �ǰ� �Ҹ�
    public AudioClip itemPickupClip;  // ������ �ݴ� �Ҹ�
    public AudioClip deahtClip;
    private AudioSource playerAudioPlayer;  // �÷��̾� �Ҹ� �����
    private Animator playerAnimator;  // �÷��̾� �ִϸ�����
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

    protected override void OnEnable()  // �������̵� �ٸ��� �� ����
    {
        base.OnEnable();  // �θ� Ŭ������ �̺�Ʈ �Լ�
        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = startingHealth;
        healthSlider.value = health;
        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }

    // ü�� ȸ��
    [PunRPC]
    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
        healthSlider.value = newHealth;
        // ������Ʈ�� ü��
    }

    // ������ ó��
    [PunRPC]
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (!dead)
        {
            playerAudioPlayer.PlayOneShot(hitClip);
            // ������� ���� ��쿡�� ȿ���� ���
        }
        // LivingEnity�� OnDamage �����ؼ� ������ ����
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
        Invoke("ReSpawn", 5.0f);  // 5.0�� �Ŀ� ������
    }

    public void ReSpawn()  // �÷��̾ ����� 5�� �Ŀ� ��Ȱ
    {
        if (photonView.IsMine)  // ���� �÷��̸� ���� ��ġ ����
        {
            // �������� �ݰ� 5 ���� ������ ���� ��ġ ����
            Vector3 randomSpawnPos = Random.insideUnitSphere * 5.0f;
            randomSpawnPos.y = 0f;  // ���� ��ġ ���� 0���� ����
            // ������ ��ġ�� �̵�
            transform.position = randomSpawnPos;
        }
        gameObject.SetActive(false);  // OnDisable ȣ���ϱ� ����
        gameObject.SetActive(true);   // OnEnable ȣ���ϱ� ����
    }

    private void OnTriggerEnter(Collider other)
    {
        // �����۰� �浹�� ��� �ش� �������� ����ϴ� ó��
        if (!dead)
        {   // �浹�� �������� ���� IItem ���۳�Ʈ�� ������ �´�.
            IItem item = other.GetComponent<IItem>();
            if (item != null)
            {//    ȣ��Ʈ�� ������ ��� ����
                // ȣ��Ʈ���� ������ ����� ���� ȿ���� ��� Ŭ���̾�Ʈ�� ����ȭ ��Ŵ
                if (PhotonNetwork.IsMasterClient)
                {
                    // Use �޼��带 �����Ͽ� ������ ���
                    item.Use(gameObject);
                    // ������ ���� �Ҹ� ���
                }
                playerAudioPlayer.PlayOneShot(itemPickupClip, 1.0f);
            }
        }
    }
}
