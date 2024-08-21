using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class ItemSpawner : MonoBehaviourPun
{
    public GameObject[] items;  // ������ ������  1. ź��  2. HPȸ�� ������
     // �÷��̾� Ʈ������
    public float maxDist = 5f;  // �÷��̾� ��ġ���� �������� ��ġ�� �ִ� �ݰ�
    public float timeBetSpawnMax = 7f;  // �ִ� �ð� ����
    public float timeBetSpawnMin = 2f;  // �ּ� �ð� ����
    private float timeBetSpawn;  // ���� ����
    private float lastSpwanTime;  // ������ ���� ����

    void Start()
    {
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        lastSpwanTime = 0f;
        //playerTr = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;  // ȣ��Ʈ�� ������ ���� ���� ����
        if (Time.time >= lastSpwanTime + timeBetSpawn) // playerTr=!null ��ȿ�� �˻�
        {
            lastSpwanTime = Time.time;
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
            Spawn();
        }
    }

    void Spawn()
    {
        Vector3 spawnPos = GetRandomPointOnNavMesh(Vector3.zero, maxDist);
        spawnPos += Vector3.up * 0.5f;  // �ٴڿ��� ��ġ�� 0.5 ��ŭ �ø�
        // ������ �� �ϳ��� �������� ��� ������ġ�� ����
        GameObject selectecItem = items[Random.Range(0, items.Length)];

        #region �ڱ��ڽŸ� �����ǰ� �Ҹ��Ѵ�.
        //GameObject item = Instantiate(selectecItem, spawnPos, Quaternion.identity);
        // ������ �������� 5�� �Ŀ� �Ҹ�
        //Destroy(item, 5.0f);
        #endregion

        // ��Ʈ��ũ���� ��� Ŭ���̾�Ʈ���� �ش� ������ ����
        GameObject item = PhotonNetwork.Instantiate(selectecItem.name, spawnPos, Quaternion.identity);
        StartCoroutine(DestroyAfter(item, 5.0f));
    }

    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target != null)  // �ı����� �ʾҴٸ�
        {
            PhotonNetwork.Destroy(target);  // �Ҹ� ���Ѷ�
        }
    }

    // �׺�޽����� ������ ��ġ�� ��ȯ�ϴ� �޼���
    // center�� �߽����� �Ÿ� �ݰ� �ȿ��� ������ ��ġ�� ã��
    private Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance)
    {//                     �������� 1�� ��(��) �ȿ��� ������ ���� ��ȯ�ϴ� ������Ƽ
        Vector3 randomPos = Random.insideUnitSphere * distance + center;
        // �׺�޽� ���ø��� ��� ������ �����ϴ� ����
        NavMeshHit hit;
        // maxdistance �ݰ� �ȿ��� randomPos�� ���� ����� �׺�޽� ���� ������ ã�´�.
        NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas);
        return hit.position;
    }
}
