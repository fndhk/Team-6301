using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public GameObject enemyPrefab;
    public int count;
    public float timeBetweenEnemies;
}

public class EnemySpawner : MonoBehaviour
{
    // [Header("���̺� ����")]
    // public List<Wave> waves = new List<Wave>(); // �� ���� �����մϴ�.
    public float timeBetweenWaves = 5f;
    private bool isWaveFinished = false;
    [Header("���� ��ġ")]
    public Transform[] spawnPoints;

    private int currentWaveIndex = 0;
    private StageData currentStage; // ���� �������� ������ ���� ����

    void Start()
    {
        // GameSession���� ���õ� �������� ������ �����ɴϴ�.
        currentStage = GameSession.instance.selectedStage;

        if (currentStage != null)
        {
            // �������� ������ ������ ���̺� ����
            StartCoroutine(SpawnAllWaves());
        }
        else
        {
            Debug.LogError("���õ� �������� ������ �����ϴ�! MainMenu���� �����ߴ��� Ȯ���ϼ���.");
        }
    }

    IEnumerator SpawnAllWaves()
    {
        // currentStage�� �ִ� waves ����Ʈ�� ����ϵ��� �����մϴ�.
        while (currentWaveIndex < currentStage.waves.Count)
        {
            Wave currentWave = currentStage.waves[currentWaveIndex];
            Debug.Log((currentWaveIndex + 1) + "��° ���̺� ����!");

            for (int i = 0; i < currentWave.count; i++)
            {
                SpawnEnemy(currentWave.enemyPrefab);
                yield return new WaitForSeconds(currentWave.timeBetweenEnemies);
            }

            if (currentWaveIndex < currentStage.waves.Count - 1)
            {
                Debug.Log("���� ���̺���� " + timeBetweenWaves + "�� ���...");
                yield return new WaitForSeconds(timeBetweenWaves);
            }
            currentWaveIndex++;
        }
        Debug.Log("��� ���̺갡 ����Ǿ����ϴ�!");
        isWaveFinished = true;
    }
    void Update()
    {
        // ���̺갡 ������, �ʿ� ����ִ� ���� ���ٸ�
        if (isWaveFinished && Enemy.liveEnemyCount <= 0)
        {
            // GameManager�� �������� Ŭ��� �˸�
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.StageClear();
            }

            // Ŭ���� ó���� �� ���� �ϵ��� �� ��ũ��Ʈ�� ��Ȱ��ȭ
            this.enabled = false;
        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        // �� �Լ��� ������ �ʿ� �����ϴ�.
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("���� ��ġ�� �������� �ʾҽ��ϴ�!");
            return;
        }
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}