using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// IK : 왼손 오른손이 무기에 정확하게 부착되게
// 총알 발사
public class PlayerShooter : MonoBehaviourPun
{
    public Gun gun;
    public Transform gunPivot;       // 총기 배치 기준점
    public Transform leftHandMound;  // 총기의 왼쪽 손잡이 위치
    public Transform rightHandMound; // 총기의 오른쪽 손잡이 위치
    public PlayerInput PlayerInput;
    [SerializeField] private Animator playerAnimator;
    private readonly int hashReload = Animator.StringToHash("ReloadTrigger");

    private void OnEnable()
    {// 슈터가 활성화 될때 총도 함께 활성화
        gun.gameObject.SetActive(true);
    }

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        PlayerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        if (PlayerInput.fire)
        {
            gun.Fire();
        }
        else if (PlayerInput.reload)
        {
            if (gun.Reload())
            {
                playerAnimator.SetTrigger(hashReload);
            }
        }
        UpdateUI();
    }

    [PunRPC]
    public void UpdateUI()  // 탄알 UI 갱신
    {
        if (gun != null && UIManager.Instance != null)
        {
            gun.UpdateBulletText();
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {// 총이 기준점 gunPivot을 3D 모델의 오른쪽 팔꿈치 위치로 이동
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        // IK를 사용하여 왼손의 위치와 회전을 총의 왼쪽 손잡이 맞춤
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        // Weight : 가중치     가중치를 변경할때

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMound.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMound.rotation);

        // IK를 사용한 오른손의 위치와 회전을 총의 오른쪽 손잡이 맞춤
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMound.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMound.rotation);
    }

    private void OnDisable()
    {// 슈터가 비활성화 될때 총도 함께 비활성화
        gun.gameObject.SetActive(false);
    }
}
