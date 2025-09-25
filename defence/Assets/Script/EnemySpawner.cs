using System.Collections;
using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject enemyPrefab; // 스폰할 적 프리팹
    public Transform[] spawnPoints; // 적이 생성될 위치들
    public float spawnInterval = 2f; // 적 생성 간격 (초)

    void Start()
    {
        // 게임이 시작되면 적 생성 코루틴을 실행
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        // 게임이 끝날 때까지 무한 반복
        while (true)
        {
            // spawnInterval 만큼 대기
            yield return new WaitForSeconds(spawnInterval);

            // 0부터 spawnPoints 배열의 길이 -1 사이의 랜덤한 인덱스 선택
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            // 선택된 위치(Transform)를 가져옴
            Transform spawnPoint = spawnPoints[spawnIndex];

            // 적 프리펩을 선택된 위치에, 기본 회전값으로 생성
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}