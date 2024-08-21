using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Enemy : LivingEntity
{
    public LayerMask whatIsTarget;        // ���� ��� ���̾�
    private LivingEntity targetEntity;  // ���� ���
    private NavMeshAgent pathFinder; // ��� ��� AI ������Ʈ
    public ParticleSystem hitEffect;
    public AudioClip deatSound;
    public AudioClip hitSound;
    private Animator enemyAnimator;
    private AudioSource enemyAudioPlayer;
    private Renderer enemyRenderer;
    public float damage = 20f;
    public float timeBetAttack = 0.5f;  //���� ����
    private float lastAttackTime; // ������ ���� ����

    private readonly int hashHasTarget = Animator.StringToHash("HesTargetBool");
    private readonly int hashDie = Animator.StringToHash("DieTrigger");

    private bool hasTarget
    {
        get
        {   // ������ ����� �����ϰ� ����� ������� �ʾҴٸ�
            if (targetEntity != null && !targetEntity.dead)
                return true;
            return false;
        }
    }

    private void Awake()
    {
        pathFinder = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        enemyAudioPlayer = GetComponent<AudioSource>();
        enemyRenderer = GetComponentInChildren<Renderer>();
    }

    [PunRPC]
    public void Setup(float newHealth, float newDamage, float newSpeed, Color skinColor)
    {
        startingHealth = newHealth;
        health = newHealth;
        damage = newDamage;
        pathFinder.speed = newSpeed;
        enemyRenderer.material.color = skinColor;
    }

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;  // ȣ��Ʈ�� �ƴϸ� AI ���� ��ƾ�� ���� ��������
        //StartCoroutine("UpdatePaht");
        InvokeRepeating("UpdatePaht", 0.01f, 0.25f);
        // ���� ������Ʈ�� Ȱ��ȭ�� ���ÿ� AI ���� ��ƾ�� ����
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        enemyAnimator.SetBool(hashHasTarget, hasTarget);
    }

    void UpdatePaht()  // �ֱ������� ������ ����� ��ġ�� ã�� ������Ʈ �ȴ�.
    {
        // while (!dead)
        if (!dead)
        {
            if (hasTarget)  // ���� ����� �ִٸ�
            {
                pathFinder.isStopped = false;
                pathFinder.SetDestination(targetEntity.transform.position);
            }
            else  // ���� ����� ���ٸ�
            {
                pathFinder.isStopped = true;
                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f,
                    whatIsTarget);
                for (int i = 0; i < colliders.Length; i++)
                {
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                    if (livingEntity != null && !livingEntity.dead)
                    {
                        targetEntity = livingEntity;
                        break;
                    }
                }
            }
            //yield return new WaitForSeconds(0.25f);
        }
    }

    [PunRPC]
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dead)
        {
            // ���ݹ��� ������ �������� ��ƼŬ ȿ�� ���
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();
            enemyAudioPlayer.PlayOneShot(hitSound);
        }
        base.OnDamage(damage, hitPoint, hitNormal);
    }

    [PunRPC]
    public override void Die()
    {
        base.Die();  // �⺻���� ���ó�� �ϰ�
        Collider[] enemyColliders = GetComponents<Collider>();
        for (int i = 0; i < enemyColliders.Length; ++i)
        {// �ٸ� AI�� ���ظ� ���� �ʵ��� �ڽ��� ��� �ݶ��̴��� ��Ȱ��ȭ
            enemyColliders[i].enabled = false;
        }
        pathFinder.isStopped = true;
        pathFinder.enabled = false;
        enemyAudioPlayer.PlayOneShot(deatSound);
        enemyAnimator.SetTrigger(hashDie);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        // Ʈ���� �浹�� ���� ���� ������Ʈ�� ���� ����̶�� ���� ����
        if (!dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            // ������ LivingEntity Ÿ���� ��������
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();
            if (attackTarget != null && attackTarget == targetEntity)
            {
                // ������ LivingEntity�� �ڽ��� ���� ����̶�� ���� ����
                lastAttackTime = Time.time;
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                // ������ �ǰ���ġ�� �ǰ� ������ �ٻ簪���� ���
                Vector3 hitNormal = transform.position - other.transform.position;
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
        }
    }
}
