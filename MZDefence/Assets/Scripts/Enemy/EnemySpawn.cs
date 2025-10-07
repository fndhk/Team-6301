using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;  // 생성할 적 프리팹
    public float minX = -7f;        // X 최소 범위
    public float maxX = 7f;         // X 최대 범위
    public float spawnY = 6f;       // Y 고정 위치 (화면 위쪽)
    public float spawnInterval = 2f; // 생성 간격 (초 단위)

    void Start()
    {
        // 일정 간격으로 SpawnEnemy 실행
        InvokeRepeating("SpawnEnemy", 1f, spawnInterval);
    }

    void SpawnEnemy()
    {
        float randomX = Random.Range(minX, maxX); // 랜덤 X 좌표
        Vector3 spawnPos = new Vector3(randomX, spawnY, 0); // 스폰 위치
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}
