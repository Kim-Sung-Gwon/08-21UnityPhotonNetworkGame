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
        // 리지디 바디를 이용해서 게임오브젝트 위치 변경
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
        // 장점 로테이트와, 트렌스레이트를 안써도 된다.
        //              Rigidboby에서 MovePosition을 지원한다.
    }

    private void Rotate()
    {
        float turn = playerInput.rotate * rotateSpeed * Time.deltaTime;
        playerRigidbody.rotation = playerRigidbody.rotation * Quaternion.Euler(0f, turn, 0f);
        //                                                    Vector3 값으로 받아서 Y축으로 회전
    }
}
