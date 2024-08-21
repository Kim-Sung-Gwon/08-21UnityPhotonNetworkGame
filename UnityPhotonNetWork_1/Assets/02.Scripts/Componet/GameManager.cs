using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<GameManager>();
            return instance;
        }
    }

    public GameObject playerPrefab;  // 생성할 플레이어 캐릭터 프리펩

    private int score = 0; // 현재 게임 점수

    public bool isGameOver{ get; private set; }

    void Awake()
    {
        // 씬에 싱글턴 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != null)
        {
            Destroy(gameObject);  // 자신을 파괴
        }
    }

    private void Start()
    {
        // FindAnyObjectByType은 Find 보다 찾는 속도가 느리다.
        //FindAnyObjectByType<PlayerHealth>().onDeath += EndGame;
        // 플레이어 캐릭터의 사망 이벤트 발생시 게임오버

        // 플레이어가 생성 할 위치
        Vector3 randomSpawnPos = Random.insideUnitSphere * 5f;
        randomSpawnPos.y = 1;
        PhotonNetwork.Instantiate(playerPrefab.name, randomSpawnPos, Quaternion.identity);
    }

    public void AddScore(int newScore)
    {
        if (!isGameOver)
        {
            score += newScore;
            UIManager.Instance.UpdateScoreText(score);
        }
    }

    public void EndGame()
    {
        isGameOver = true;
        UIManager.Instance.SetAciveGameOverUI(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)  // 로컬의 움직임을 송신
        {
            stream.SendNext(score);
        }
        else  // 다른 네트워크 유저의 움직임을 수신
        {
            score = (int)stream.ReceiveNext();
            // 네트워크를 통해 score 값을 받기
            UIManager.Instance.UpdateScoreText(score);
            // 동기화 해서 받은 점수를 UI로 표시
        }
    }
}