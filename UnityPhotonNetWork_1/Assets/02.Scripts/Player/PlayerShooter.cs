using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// IK : �޼� �������� ���⿡ ��Ȯ�ϰ� �����ǰ�
// �Ѿ� �߻�
public class PlayerShooter : MonoBehaviourPun
{
    public Gun gun;
    public Transform gunPivot;       // �ѱ� ��ġ ������
    public Transform leftHandMound;  // �ѱ��� ���� ������ ��ġ
    public Transform rightHandMound; // �ѱ��� ������ ������ ��ġ
    public PlayerInput PlayerInput;
    [SerializeField] private Animator playerAnimator;
    private readonly int hashReload = Animator.StringToHash("ReloadTrigger");

    private void OnEnable()
    {// ���Ͱ� Ȱ��ȭ �ɶ� �ѵ� �Բ� Ȱ��ȭ
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
    public void UpdateUI()  // ź�� UI ����
    {
        if (gun != null && UIManager.Instance != null)
        {
            gun.UpdateBulletText();
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {// ���� ������ gunPivot�� 3D ���� ������ �Ȳ�ġ ��ġ�� �̵�
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        // IK�� ����Ͽ� �޼��� ��ġ�� ȸ���� ���� ���� ������ ����
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        // Weight : ����ġ     ����ġ�� �����Ҷ�

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMound.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMound.rotation);

        // IK�� ����� �������� ��ġ�� ȸ���� ���� ������ ������ ����
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMound.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMound.rotation);
    }

    private void OnDisable()
    {// ���Ͱ� ��Ȱ��ȭ �ɶ� �ѵ� �Բ� ��Ȱ��ȭ
        gun.gameObject.SetActive(false);
    }
}
