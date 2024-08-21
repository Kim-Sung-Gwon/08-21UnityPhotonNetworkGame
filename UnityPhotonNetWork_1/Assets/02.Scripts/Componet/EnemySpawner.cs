using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class EnemySpawner : MonoBehaviourPun, IPunObservable
{
    public Enemy enemyPrefab;
    public Transform[] spawnPoints;

    public float damageMax = 40f;  // 최대 공격력
    public float damageMin = 20f;  // 최소 공격력

    public float healthMax = 200f;
    public float healthMin = 100f;

    public float speedMax = 3f;
    public float speedMin = 1.0f;

    public Color strongEnemyColor = Color.red;
    // 강한 적 AI가 가지게 될 피부색
    private List<Enemy> enemies = new List<Enemy>();
    private int wave;  // 현재 웨이브
    private int enemyCount = 0;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 남은 적수를 네트워크로 보내기
            stream.SendNext(enemies.Count);
            stream.SendNext(wave);  // 현재 웨이브를 네트워크로 보내기
        }
        else
        {
            // 리모트 오브젝트 읽기 부분이 실행됨
            // 남은 적의 수를 네트워크를 통해 받기
            enemyCount = (int)stream.ReceiveNext();
            // 현재 웨이브를 네트워크를 통해 받기
            wave = (int)stream.ReceiveNext();
        }
    }

    void Awake()
    {
        // 좀비 생상이 직렬화 되어서 서버에 갔다가 다시 역직렬화 되어서 color값으로 되돌아온다.
        PhotonPeer.RegisterType(typeof(Color), 128, ColorSerialization.SerializeColor,
            ColorSerialization.DeserializeColor);
    }

    void Update()
    {
        // 호스트만 적을 직접 생성한다.
        // 다른 클라이언트는 호스트가 생성한 적을 동기화해서 받아온다.
        if (PhotonNetwork.IsMasterClient)
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameOver)
                return;
            // 적을 모두 물리친 경우 다음 스판을 실행
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
            // 호스트는 직접 갱신 한 적리스트를 이용해 남은 적의 수를 표시
            UIManager.Instance.UpdateWaveText(wave, enemies.Count);
        }
        else
        {
            // 클라이언트는 적리스트를 갱신할 수 없음으로
            // 호스트가 보내준 enemyCount를 이용해 적의 수를 표시
            UIManager.Instance.UpdateWaveText(wave, enemyCount);
        }
    }

    void SpawnWave()  // 현재 웨이브에 맞춰 적 생성
    {
        wave++;
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);
        // 현재 웨이브 * 1.5를 반올림 한 수만큼 생성
        for (int i = 0; i < spawnCount; i++)
        {
            // 적의 강도 (세기)  = 0 ~ 100 중 랜덤으로 결정
            float enemyintensity = Random.Range(0f, 1f);
            CreateEnemy(enemyintensity);
        }
    }

    void CreateEnemy(float intensity)  // 적을 생성하고 추적할 대상을 할당
    {
        // intensity를 기반으로 적의 능력치가 결정된다.
        float health = Mathf.Lerp(healthMin, healthMax, intensity);
        float damage = Mathf.Lerp(damageMin, damageMax, intensity);
        float speed = Mathf.Lerp(speedMin, speedMax, intensity);
        // intensity를 기반으로 하얀색과 enemyStrength 사이에서 적 피부색이 결정
        Color skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity);
        // 생설할 위치를 랜덤으로 결정
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        // 적 프리팹으로 부터 적 생성
        //Enemy enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        //enemy.Setup(health, damage, speed, skinColor);

        GameObject createdEnemy = PhotonNetwork.Instantiate(enemyPrefab.gameObject.name,
            spawnPoint.position, spawnPoint.rotation);
        Enemy enemy = createdEnemy.GetComponent<Enemy>();
        enemy.photonView.RPC("Setup", RpcTarget.All, health, damage, speed, skinColor);
        enemies.Add(enemy);  // 생성한 적을 리스트에 추가
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
