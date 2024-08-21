using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;  // 시네머신의 가상 카메라와 포톤 뷰를 동기화 하기위해서
//                         MonoBehaviourPun 기능에서 포톤 뷰를 추가한 프로퍼티이다.
public class CameraSetUp : MonoBehaviourPun
{
    void Start()
    {
        // 만약 자신이 로컬 플레이어(자기자신)이라면
        if (photonView.IsMine)  // 포톤 네트워크 상의 포톤뷰가 자기 자신의 것이라면
        {
            // 씬에 있는 시네머신의 가상 카메라를 찾고
            CinemachineVirtualCamera followCam = FindObjectOfType<CinemachineVirtualCamera>();
            // 가상 카메라의 추적 대상을 자신의 트랜스폼으로 변경
            followCam.Follow = transform;
            followCam.LookAt = transform;
        }
    }
}
