using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;  // 유니티용 포톤 컴퍼넌트
using Photon.Realtime;  // 포톤 서비스 관련 라이브러리
using UnityEngine.UI;
// 마스터 서버(리슨 서버)와 Mach Making 룸 접속 담당
public class LobbyManager : MonoBehaviourPunCallbacks
{   // 유니티와 포톤네트워크에서 제공하는 함수를 같이사용
    private string gameVersion = "1";  // 게임 버전
    public Text connectionInfoText;  // 네트워크 정보 표시
    public Button joinButton;  // 룸 접속 버튼 방만들기 버튼

    void Start()
    {// 버전이 달라지면 버전 별로 접속된다.
        // 접속에 필요한 정보(게임 버전) 설정
        PhotonNetwork.GameVersion = gameVersion;
        
        // 설정한 정보로 마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();

        // 룸 접속 버튼 잠시 비활성화
        joinButton.interactable = false;

        // 접속 시도 중임을 텍스트로 표시
        connectionInfoText.text = "마스터 서버에 접속 중......";
    }

    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster()
    {
        joinButton.interactable = true;
        connectionInfoText.text = "온라인 : 마스터 서버와 연결됨......";
    }

    // 마스터 서버 접속 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false;
        connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음......";
    }

    public void Connect()  // 룸 접속 시도.  joinButton 누를때 호출 될 함수
    {
        // 중복 접속을 막기 위해서 버튼을 비활성화
        joinButton.interactable = false;
        if (PhotonNetwork.IsConnected)  // 마스터 서버에 접속 중이라면
        {
            connectionInfoText.text = "룸에 접속......";
            PhotonNetwork.JoinRandomRoom();  // 아무방에나 접속 : 랜덤 메치 메이킹
        }
        else
        {
            connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음......";
            PhotonNetwork.ConnectUsingSettings();
            // 마스터 서버로 재접속 시도
        }
    }

    // (빈룸이 없어서) 랜덤 룸 참가에 실패한 경우 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "빈 방이 없음, 새로운 방 생성......";
        // 최대 4명을 수용 가능한 빈 방 생성
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4 });
        // 생성된 룸 목록을 확인 하는 기능은 만들지 않으므로 룸의 이름은
        // 입력하지 않고 null로 입력했다. 수용 가능한 인원은 4명으로 제한
        // 생성된 룸은 리슨 서버 방식으로 하며 룸을 생성한 클라이언트가 호스트 역할을 맡는다.
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "방 참가 성공";
        PhotonNetwork.LoadLevel("Main");
        // 모든 룸 참가자가 Main씬을 로드하게 한다.
    }
}