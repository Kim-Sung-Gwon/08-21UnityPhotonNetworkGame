using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;  // �ó׸ӽ��� ���� ī�޶�� ���� �並 ����ȭ �ϱ����ؼ�
//                         MonoBehaviourPun ��ɿ��� ���� �並 �߰��� ������Ƽ�̴�.
public class CameraSetUp : MonoBehaviourPun
{
    void Start()
    {
        // ���� �ڽ��� ���� �÷��̾�(�ڱ��ڽ�)�̶��
        if (photonView.IsMine)  // ���� ��Ʈ��ũ ���� ����䰡 �ڱ� �ڽ��� ���̶��
        {
            // ���� �ִ� �ó׸ӽ��� ���� ī�޶� ã��
            CinemachineVirtualCamera followCam = FindObjectOfType<CinemachineVirtualCamera>();
            // ���� ī�޶��� ���� ����� �ڽ��� Ʈ���������� ����
            followCam.Follow = transform;
            followCam.LookAt = transform;
        }
    }
}
