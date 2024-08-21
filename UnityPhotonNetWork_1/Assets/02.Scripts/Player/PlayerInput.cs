using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// 입력과 움직임과 분리해서 스크립트를 만든다.
// 입력과 액터를 나누기 : 장점 게임 플랫폼이 바뀌더라도 여기서만 바꾸면 된다.
public class PlayerInput : MonoBehaviourPun
{// 입력 만되게 하는 스크립트
    public string moveAxisName = "Vertical";
    public string rotateAxisName = "Horizontal";
    public string fireButtonName = "Fire1";       // 마우스 왼쪽 버튼 ,왼쪽 컨트롤 키
    public string reloadButtonName = "Reload";
    // 키관련 프로퍼티 만들기
    public float move { get; private set; }
    public float rotate { get; private set; }
    public bool fire { get; private set; }
    public bool reload { get; private set; }

    void Start()
    {
        
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        // 로컬 플레이어가 아니면 입력을 받지 않음
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            move = 0f;
            rotate = 0f;
            fire = false;
            reload = false;
            return;
        }

        move = Input.GetAxis(moveAxisName);
        rotate = Input.GetAxis(rotateAxisName);
        fire = Input.GetButton(fireButtonName);     // Input.GetButton은 자체로 bool값으로 받고 있다.
        reload = Input.GetButtonDown(reloadButtonName);
    }
}
