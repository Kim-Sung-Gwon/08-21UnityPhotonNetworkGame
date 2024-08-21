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

    public GameObject playerPrefab;  // ������ �÷��̾� ĳ���� ������

    private int score = 0; // ���� ���� ����

    public bool isGameOver{ get; private set; }

    void Awake()
    {
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
        if (instance != null)
        {
            Destroy(gameObject);  // �ڽ��� �ı�
        }
    }

    private void Start()
    {
        // FindAnyObjectByType�� Find ���� ã�� �ӵ��� ������.
        //FindAnyObjectByType<PlayerHealth>().onDeath += EndGame;
        // �÷��̾� ĳ������ ��� �̺�Ʈ �߻��� ���ӿ���

        // �÷��̾ ���� �� ��ġ
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
        if (stream.IsWriting)  // ������ �������� �۽�
        {
            stream.SendNext(score);
        }
        else  // �ٸ� ��Ʈ��ũ ������ �������� ����
        {
            score = (int)stream.ReceiveNext();
            // ��Ʈ��ũ�� ���� score ���� �ޱ�
            UIManager.Instance.UpdateScoreText(score);
            // ����ȭ �ؼ� ���� ������ UI�� ǥ��
        }
    }
}