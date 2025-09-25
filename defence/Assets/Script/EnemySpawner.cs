using System.Collections;
using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    [Header("���� ����")]
    public GameObject enemyPrefab; // ������ �� ������
    public Transform[] spawnPoints; // ���� ������ ��ġ��
    public float spawnInterval = 2f; // �� ���� ���� (��)

    void Start()
    {
        // ������ ���۵Ǹ� �� ���� �ڷ�ƾ�� ����
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        // ������ ���� ������ ���� �ݺ�
        while (true)
        {
            // spawnInterval ��ŭ ���
            yield return new WaitForSeconds(spawnInterval);

            // 0���� spawnPoints �迭�� ���� -1 ������ ������ �ε��� ����
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            // ���õ� ��ġ(Transform)�� ������
            Transform spawnPoint = spawnPoints[spawnIndex];

            // �� �������� ���õ� ��ġ��, �⺻ ȸ�������� ����
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}