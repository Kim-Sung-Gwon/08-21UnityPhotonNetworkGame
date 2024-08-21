using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;  // ����Ƽ�� ���� ���۳�Ʈ
using Photon.Realtime;  // ���� ���� ���� ���̺귯��
using UnityEngine.UI;
// ������ ����(���� ����)�� Mach Making �� ���� ���
public class LobbyManager : MonoBehaviourPunCallbacks
{   // ����Ƽ�� �����Ʈ��ũ���� �����ϴ� �Լ��� ���̻��
    private string gameVersion = "1";  // ���� ����
    public Text connectionInfoText;  // ��Ʈ��ũ ���� ǥ��
    public Button joinButton;  // �� ���� ��ư �游��� ��ư

    void Start()
    {// ������ �޶����� ���� ���� ���ӵȴ�.
        // ���ӿ� �ʿ��� ����(���� ����) ����
        PhotonNetwork.GameVersion = gameVersion;
        
        // ������ ������ ������ ���� ���� �õ�
        PhotonNetwork.ConnectUsingSettings();

        // �� ���� ��ư ��� ��Ȱ��ȭ
        joinButton.interactable = false;

        // ���� �õ� ������ �ؽ�Ʈ�� ǥ��
        connectionInfoText.text = "������ ������ ���� ��......";
    }

    // ������ ���� ���� ������ �ڵ� ����
    public override void OnConnectedToMaster()
    {
        joinButton.interactable = true;
        connectionInfoText.text = "�¶��� : ������ ������ �����......";
    }

    // ������ ���� ���� ���н� �ڵ� ����
    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false;
        connectionInfoText.text = "�������� : ������ ������ ������� ����......";
    }

    public void Connect()  // �� ���� �õ�.  joinButton ������ ȣ�� �� �Լ�
    {
        // �ߺ� ������ ���� ���ؼ� ��ư�� ��Ȱ��ȭ
        joinButton.interactable = false;
        if (PhotonNetwork.IsConnected)  // ������ ������ ���� ���̶��
        {
            connectionInfoText.text = "�뿡 ����......";
            PhotonNetwork.JoinRandomRoom();  // �ƹ��濡�� ���� : ���� ��ġ ����ŷ
        }
        else
        {
            connectionInfoText.text = "�������� : ������ ������ ������� ����......";
            PhotonNetwork.ConnectUsingSettings();
            // ������ ������ ������ �õ�
        }
    }

    // (����� ���) ���� �� ������ ������ ��� ����
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "�� ���� ����, ���ο� �� ����......";
        // �ִ� 4���� ���� ������ �� �� ����
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4 });
        // ������ �� ����� Ȯ�� �ϴ� ����� ������ �����Ƿ� ���� �̸���
        // �Է����� �ʰ� null�� �Է��ߴ�. ���� ������ �ο��� 4������ ����
        // ������ ���� ���� ���� ������� �ϸ� ���� ������ Ŭ���̾�Ʈ�� ȣ��Ʈ ������ �ô´�.
    }

    // �뿡 ���� �Ϸ�� ��� �ڵ� ����
    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "�� ���� ����";
        PhotonNetwork.LoadLevel("Main");
        // ��� �� �����ڰ� Main���� �ε��ϰ� �Ѵ�.
    }
}