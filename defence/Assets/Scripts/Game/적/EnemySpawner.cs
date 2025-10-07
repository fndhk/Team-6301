using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� ���̺��� ������ ��� '���赵'�Դϴ�. Inspector â���� ���� ������ �� �ֽ��ϴ�.
[System.Serializable]
public class Wave
{
    public GameObject enemyPrefab; // �� ���̺꿡�� ��ȯ�� �� ������
    public int count;              // �� ������ ��ȯ����
    public float timeBetweenEnemies; // �� ���� ��ȯ�Ǵ� �ð� ����
}

public class EnemySpawner : MonoBehaviour
{
    [Header("���̺� ����")]
    public List<Wave> waves = new List<Wave>(); // ���� ���̺긦 ���� ����Ʈ
    public float timeBetweenWaves = 5f;       // ���� ���̺갡 ���۵Ǳ������ ��� �ð�

    [Header("���� ��ġ")]
    public Transform[] spawnPoints; // ���� ������ ��ġ��

    private int currentWaveIndex = 0; // ���� ���� ���� ���̺� ��ȣ

    void Start()
    {
        // ������ ���۵Ǹ� ��� ���̺긦 ���������� �����ϴ� �ڷ�ƾ�� ����
        StartCoroutine(SpawnAllWaves());
    }

    IEnumerator SpawnAllWaves()
    {
        // ��� ���̺긦 �� ������ ������ �ݺ�
        while (currentWaveIndex < waves.Count)
        {
            // ���� ���̺� ������ ������
            Wave currentWave = waves[currentWaveIndex];
            Debug.Log((currentWaveIndex + 1) + "��° ���̺� ����!");

            // ���� ���̺꿡�� ��ȯ�ؾ� �� ���� ����ŭ �ݺ�
            for (int i = 0; i < currentWave.count; i++)
            {
                // ���� ���� ��ȯ�ϴ� �Լ� ȣ��
                SpawnEnemy(currentWave.enemyPrefab);

                // ���� ���� ��ȯ�� ������ ���
                yield return new WaitForSeconds(currentWave.timeBetweenEnemies);
            }

            // ���� ���̺갡 ��� ��������, ���� ���̺���� ���
            if (currentWaveIndex < waves.Count - 1)
            {
                Debug.Log("���� ���̺���� " + timeBetweenWaves + "�� ���...");
                yield return new WaitForSeconds(timeBetweenWaves);
            }

            // ���� ���̺�� �Ѿ
            currentWaveIndex++;
        }

        // ��� ���̺갡 ������ �޽��� ���
        Debug.Log("��� ���̺갡 ����Ǿ����ϴ�!");
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("���� ��ġ�� �������� �ʾҽ��ϴ�!");
            return;
        }

        // ���� ��ġ �� �� ���� �������� ����
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];

        // �� �������� ���õ� ��ġ�� ����
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}