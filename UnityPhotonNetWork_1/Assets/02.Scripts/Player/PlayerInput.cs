using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// �Է°� �����Ӱ� �и��ؼ� ��ũ��Ʈ�� �����.
// �Է°� ���͸� ������ : ���� ���� �÷����� �ٲ���� ���⼭�� �ٲٸ� �ȴ�.
public class PlayerInput : MonoBehaviourPun
{// �Է� ���ǰ� �ϴ� ��ũ��Ʈ
    public string moveAxisName = "Vertical";
    public string rotateAxisName = "Horizontal";
    public string fireButtonName = "Fire1";       // ���콺 ���� ��ư ,���� ��Ʈ�� Ű
    public string reloadButtonName = "Reload";
    // Ű���� ������Ƽ �����
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
        // ���� �÷��̾ �ƴϸ� �Է��� ���� ����
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
        fire = Input.GetButton(fireButtonName);     // Input.GetButton�� ��ü�� bool������ �ް� �ִ�.
        reload = Input.GetButtonDown(reloadButtonName);
    }
}
