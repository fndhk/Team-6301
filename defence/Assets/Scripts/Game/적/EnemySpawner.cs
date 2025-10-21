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
    // [Header("웨이브 설정")]
    // public List<Wave> waves = new List<Wave>(); // 이 줄을 삭제합니다.
    public float timeBetweenWaves = 5f;
    private bool isWaveFinished = false;
    [Header("스폰 위치")]
    public Transform[] spawnPoints;

    private int currentWaveIndex = 0;
    private StageData currentStage; // 현재 스테이지 정보를 담을 변수
    private bool isSpawningStarted = false;

    void Start()
    {
        // GameSession에서 선택된 스테이지 정보를 가져옵니다.
        currentStage = GameSession.instance.selectedStage;

        if (currentStage == null)
        {
            Debug.LogError("선택된 스테이지 정보가 없습니다! MainMenu부터 시작했는지 확인하세요.");
        }
    }
    public void StartSpawning()
    {
        if (isSpawningStarted) return; // 이미 시작했으면 무시

        isSpawningStarted = true;

        if (currentStage != null)
        {
            StartCoroutine(SpawnAllWaves());
            Debug.Log("EnemySpawner: 적 스폰 시작!");
        }
    }

    IEnumerator SpawnAllWaves()
    {
        // currentStage에 있는 waves 리스트를 사용하도록 수정합니다.
        while (currentWaveIndex < currentStage.waves.Count)
        {
            Wave currentWave = currentStage.waves[currentWaveIndex];
            Debug.Log((currentWaveIndex + 1) + "번째 웨이브 시작!");

            for (int i = 0; i < currentWave.count; i++)
            {
                SpawnEnemy(currentWave.enemyPrefab);
                yield return new WaitForSeconds(currentWave.timeBetweenEnemies);
            }

            // ------ 신규 수정: 마지막 웨이브가 아닐 때만 대기 ------
            if (currentWaveIndex < currentStage.waves.Count - 1)
            {
                Debug.Log("다음 웨이브까지 " + timeBetweenWaves + "초 대기...");
                yield return new WaitForSeconds(timeBetweenWaves);
            }
            currentWaveIndex++;
        }
        Debug.Log("모든 웨이브가 종료되었습니다!");
        isWaveFinished = true;
    }
    void Update()
    {
        // 웨이브가 끝났고, 맵에 살아있는 적이 없다면
        if (isWaveFinished && Enemy.liveEnemyCount <= 0)
        {
            // GameManager에 스테이지 클리어를 알림
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.StageClear();
            }

            // 클리어 처리는 한 번만 하도록 이 스크립트를 비활성화
            this.enabled = false;
        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        // 이 함수는 수정할 필요 없습니다.
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("스폰 위치가 지정되지 않았습니다!");
            return;
        }
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}