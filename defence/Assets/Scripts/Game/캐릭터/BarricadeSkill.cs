// 파일 이름: BarricadeSkill.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Linq를 사용하기 위해 추가

[CreateAssetMenu(fileName = "BarricadeSkill", menuName = "TowerDefense/Skills/Barricade Skill")]
public class BarricadeSkill : ItemEffect
{
    [Header("소환 설정")]
    public GameObject barricadePrefab; // 1단계에서 만들 바리케이드 프리팹

    [Header("레벨별 소환 개수")]
    public List<int> spawnCountByLevel = new List<int> { 1, 2, 3 };

    [Header("레인 위치 설정 (중요!)")]
    [Tooltip("적이 내려오는 5개의 레인 중앙 위치를 나타내는 Transform 리스트 (반드시 5개 연결)")]
    public List<Transform> lanePositions = new List<Transform>();

    public override void ExecuteEffect()
    {
        // 1. 필수 요소 확인
        if (barricadePrefab == null)
        {
            Debug.LogError("BarricadeSkill: Barricade Prefab이 설정되지 않았습니다!");
            return;
        }

        if (EnemyManager.instance == null)
        {
            Debug.LogError("BarricadeSkill: EnemyManager를 찾을 수 없습니다!");
            return;
        }

        if (lanePositions.Count != 5)
        {
            Debug.LogError("BarricadeSkill: lanePositions에 5개의 레인 위치가 정확히 연결되어야 합니다!");
            return;
        }

        // 2. 가장 앞선 적 찾기
        Enemy frontEnemy = EnemyManager.instance.FindFrontmostEnemy(); //
        if (frontEnemy == null)
        {
            Debug.Log("BarricadeSkill: 설치할 위치를 찾을 적이 없습니다.");
            return; // 적이 없으면 스킬 발동 취소
        }

        // 3. 소환 위치 및 개수 계산
        float spawnY = frontEnemy.transform.position.y - 8f; // 가장 앞선 적의 Y좌표
        int targetLaneIndex = FindClosestLaneIndex(frontEnemy.transform.position);

        int currentLevel = GetCurrentCharacterLevel();
        int levelIndex = Mathf.Clamp(currentLevel - 1, 0, spawnCountByLevel.Count - 1);
        int spawnCount = spawnCountByLevel[levelIndex];

        List<int> lanesToSpawn = GetSpawnLaneIndices(targetLaneIndex, spawnCount);

        // 4. 바리케이드 소환
        foreach (int laneIndex in lanesToSpawn)
        {
            // 레인의 X좌표와 적의 Y좌표를 조합하여 소환 위치 결정
            Vector3 spawnPos = new Vector3(lanePositions[laneIndex].position.x, spawnY, 0);
            Instantiate(barricadePrefab, spawnPos, Quaternion.identity);
        }

        Debug.Log($"<color=brown>바리케이드 스킬 발동! (Lv.{currentLevel}) {spawnCount}개 소환.</color>");
    }

    // 현재 캐릭터 레벨 가져오기 (다른 스킬 스크립트들 참고)
    private int GetCurrentCharacterLevel()
    {
        if (GameSession.instance != null && GameSession.instance.selectedCharacter != null && SaveLoadManager.instance != null)
        {
            string charID = GameSession.instance.selectedCharacter.characterID;
            if (SaveLoadManager.instance.gameData.characterLevels.TryGetValue(charID, out int charLevel))
            {
                return charLevel;
            }
        }
        return 1; // 기본값
    }

    // 타겟과 가장 가까운 레인 인덱스(0~4) 찾기
    private int FindClosestLaneIndex(Vector3 targetPosition)
    {
        int closestIndex = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < lanePositions.Count; i++)
        {
            float distance = Mathf.Abs(lanePositions[i].position.x - targetPosition.x);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    // 소환할 레인 인덱스 목록 계산 (가운데 정렬)
    private List<int> GetSpawnLaneIndices(int targetIndex, int count)
    {
        List<int> indices = new List<int>();
        int maxLaneIndex = 4; // 5레인 (0, 1, 2, 3, 4)

        if (count == 1)
        {
            indices.Add(targetIndex);
        }
        else if (count == 2)
        {
            indices.Add(targetIndex);
            // 오른쪽에 공간이 있으면 오른쪽에, 없으면 왼쪽에
            if (targetIndex + 1 <= maxLaneIndex)
            {
                indices.Add(targetIndex + 1);
            }
            else
            {
                indices.Add(targetIndex - 1);
            }
        }
        else if (count == 3)
        {
            // 가운데 3칸 (항상 3개가 되도록 보정)
            if (targetIndex == 0) // 왼쪽 끝
            {
                indices.AddRange(new int[] { 0, 1, 2 });
            }
            else if (targetIndex == maxLaneIndex) // 오른쪽 끝
            {
                indices.AddRange(new int[] { 2, 3, 4 });
            }
            else // 중간
            {
                indices.AddRange(new int[] { targetIndex - 1, targetIndex, targetIndex + 1 });
            }
        }
        return indices;
    }
}