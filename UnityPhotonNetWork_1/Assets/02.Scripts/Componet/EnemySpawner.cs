using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class EnemySpawner : MonoBehaviourPun, IPunObservable
{
    public Enemy enemyPrefab;
    public Transform[] spawnPoints;

    public float damageMax = 40f;  // �ִ� ���ݷ�
    public float damageMin = 20f;  // �ּ� ���ݷ�

    public float healthMax = 200f;
    public float healthMin = 100f;

    public float speedMax = 3f;
    public float speedMin = 1.0f;

    public Color strongEnemyColor = Color.red;
    // ���� �� AI�� ������ �� �Ǻλ�
    private List<Enemy> enemies = new List<Enemy>();
    private int wave;  // ���� ���̺�
    private int enemyCount = 0;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ���� ������ ��Ʈ��ũ�� ������
            stream.SendNext(enemies.Count);
            stream.SendNext(wave);  // ���� ���̺긦 ��Ʈ��ũ�� ������
        }
        else
        {
            // ����Ʈ ������Ʈ �б� �κ��� �����
            // ���� ���� ���� ��Ʈ��ũ�� ���� �ޱ�
            enemyCount = (int)stream.ReceiveNext();
            // ���� ���̺긦 ��Ʈ��ũ�� ���� �ޱ�
            wave = (int)stream.ReceiveNext();
        }
    }

    void Awake()
    {
        // ���� ������ ����ȭ �Ǿ ������ ���ٰ� �ٽ� ������ȭ �Ǿ color������ �ǵ��ƿ´�.
        PhotonPeer.RegisterType(typeof(Color), 128, ColorSerialization.SerializeColor,
            ColorSerialization.DeserializeColor);
    }

    void Update()
    {
        // ȣ��Ʈ�� ���� ���� �����Ѵ�.
        // �ٸ� Ŭ���̾�Ʈ�� ȣ��Ʈ�� ������ ���� ����ȭ�ؼ� �޾ƿ´�.
        if (PhotonNetwork.IsMasterClient)
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameOver)
                return;
            // ���� ��� ����ģ ��� ���� ������ ����
            if (enemies.Count <= 0)
            {
                SpawnWave();
            }
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // ȣ��Ʈ�� ���� ���� �� ������Ʈ�� �̿��� ���� ���� ���� ǥ��
            UIManager.Instance.UpdateWaveText(wave, enemies.Count);
        }
        else
        {
            // Ŭ���̾�Ʈ�� ������Ʈ�� ������ �� ��������
            // ȣ��Ʈ�� ������ enemyCount�� �̿��� ���� ���� ǥ��
            UIManager.Instance.UpdateWaveText(wave, enemyCount);
        }
    }

    void SpawnWave()  // ���� ���̺꿡 ���� �� ����
    {
        wave++;
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);
        // ���� ���̺� * 1.5�� �ݿø� �� ����ŭ ����
        for (int i = 0; i < spawnCount; i++)
        {
            // ���� ���� (����)  = 0 ~ 100 �� �������� ����
            float enemyintensity = Random.Range(0f, 1f);
            CreateEnemy(enemyintensity);
        }
    }

    void CreateEnemy(float intensity)  // ���� �����ϰ� ������ ����� �Ҵ�
    {
        // intensity�� ������� ���� �ɷ�ġ�� �����ȴ�.
        float health = Mathf.Lerp(healthMin, healthMax, intensity);
        float damage = Mathf.Lerp(damageMin, damageMax, intensity);
        float speed = Mathf.Lerp(speedMin, speedMax, intensity);
        // intensity�� ������� �Ͼ���� enemyStrength ���̿��� �� �Ǻλ��� ����
        Color skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity);
        // ������ ��ġ�� �������� ����
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        // �� ���������� ���� �� ����
        //Enemy enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        //enemy.Setup(health, damage, speed, skinColor);

        GameObject createdEnemy = PhotonNetwork.Instantiate(enemyPrefab.gameObject.name,
            spawnPoint.position, spawnPoint.rotation);
        Enemy enemy = createdEnemy.GetComponent<Enemy>();
        enemy.photonView.RPC("Setup", RpcTarget.All, health, damage, speed, skinColor);
        enemies.Add(enemy);  // ������ ���� ����Ʈ�� �߰�
        enemy.onDeath += () => enemies.Remove(enemy);
        enemy.onDeath += () => StartCoroutine(DestoryAfter(enemy.gameObject, 10f));
        enemy.onDeath += () => GameManager.Instance.AddScore(100);
    }

    IEnumerator DestoryAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target != null)
        {
            PhotonNetwork.Destroy(target);
        }
    }
}
