using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 180f;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Animator playeranimator;

    private readonly int hashmove = Animator.StringToHash("MoveFloat");

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playeranimator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        Rotate();
        Move();
        playeranimator.SetFloat(hashmove, playerInput.move);
    }

    private void Move()
    {
        Vector3 moveDistance = playerInput.move * transform.forward * moveSpeed * Time.deltaTime;
        // ������ �ٵ� �̿��ؼ� ���ӿ�����Ʈ ��ġ ����
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
        // ���� ������Ʈ��, Ʈ��������Ʈ�� �Ƚᵵ �ȴ�.
        //              Rigidboby���� MovePosition�� �����Ѵ�.
    }

    private void Rotate()
    {
        float turn = playerInput.rotate * rotateSpeed * Time.deltaTime;
        playerRigidbody.rotation = playerRigidbody.rotation * Quaternion.Euler(0f, turn, 0f);
        //                                                    Vector3 ������ �޾Ƽ� Y������ ȸ��
    }
}
