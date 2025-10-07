using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 각 웨이브의 정보를 담는 '설계도'입니다. Inspector 창에서 직접 설정할 수 있습니다.
[System.Serializable]
public class Wave
{
    public GameObject enemyPrefab; // 이 웨이브에서 소환할 적 프리팹
    public int count;              // 몇 마리를 소환할지
    public float timeBetweenEnemies; // 각 적이 소환되는 시간 간격
}

public class EnemySpawner : MonoBehaviour
{
    [Header("웨이브 설정")]
    public List<Wave> waves = new List<Wave>(); // 여러 웨이브를 담을 리스트
    public float timeBetweenWaves = 5f;       // 다음 웨이브가 시작되기까지의 대기 시간

    [Header("스폰 위치")]
    public Transform[] spawnPoints; // 적이 생성될 위치들

    private int currentWaveIndex = 0; // 현재 진행 중인 웨이브 번호

    void Start()
    {
        // 게임이 시작되면 모든 웨이브를 순차적으로 시작하는 코루틴을 실행
        StartCoroutine(SpawnAllWaves());
    }

    IEnumerator SpawnAllWaves()
    {
        // 모든 웨이브를 다 진행할 때까지 반복
        while (currentWaveIndex < waves.Count)
        {
            // 현재 웨이브 정보를 가져옴
            Wave currentWave = waves[currentWaveIndex];
            Debug.Log((currentWaveIndex + 1) + "번째 웨이브 시작!");

            // 현재 웨이브에서 소환해야 할 적의 수만큼 반복
            for (int i = 0; i < currentWave.count; i++)
            {
                // 실제 적을 소환하는 함수 호출
                SpawnEnemy(currentWave.enemyPrefab);

                // 다음 적이 소환될 때까지 대기
                yield return new WaitForSeconds(currentWave.timeBetweenEnemies);
            }

            // 현재 웨이브가 모두 끝났으면, 다음 웨이브까지 대기
            if (currentWaveIndex < waves.Count - 1)
            {
                Debug.Log("다음 웨이브까지 " + timeBetweenWaves + "초 대기...");
                yield return new WaitForSeconds(timeBetweenWaves);
            }

            // 다음 웨이브로 넘어감
            currentWaveIndex++;
        }

        // 모든 웨이브가 끝나면 메시지 출력
        Debug.Log("모든 웨이브가 종료되었습니다!");
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("스폰 위치가 지정되지 않았습니다!");
            return;
        }

        // 스폰 위치 중 한 곳을 랜덤으로 선택
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];

        // 적 프리팹을 선택된 위치에 생성
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}