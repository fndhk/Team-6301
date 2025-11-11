// 파일 이름: RyuSkill.cs
using UnityEngine;
using System.Collections.Generic;

// ▼▼▼ 메뉴 이름과 파일 이름을 RyuSkill로 변경 ▼▼▼
[CreateAssetMenu(fileName = "RyuSkill", menuName = "TowerDefense/Skills/Ryu Skill")]

public class RyuSkill : ItemEffect
{
    [Header("스킬 시전 UI")]
    [Tooltip("시간이 멈췄을 때 켤 반투명 패널 프리팹")]
    public GameObject positioningPanelPrefab;

    [Header("생성할 이펙트")]
    [Tooltip("생성할 기둥 프리팹")]
    public GameObject pillarPrefab; // 이 프리팹에는 이제 RyuPillar.cs가 붙어야 합니다.
    [Tooltip("타격 지점에 생성될 캐릭터 이미지 프리팹")]
    public GameObject characterImagePrefab;

    [Header("레벨별 스킬 계수")]
    [Tooltip("1, 2, 3레벨일 때의 스킬 지속시간(초)")]
    public List<float> durationByLevel = new List<float> { 3f, 3f, 4f };
    [Tooltip("1, 2, 3레벨일 때의 초당 데미지")]
    public List<int> damagePerSecondByLevel = new List<int> { 80, 110, 150 };
    [Header("스킬 위치 설정")]
    // 이제 이 부분은 BattleFocusTargeter.cs에 정의된 SkillPositionInfo를 사용합니다.
    public List<SkillPositionInfo> skillPositions = new List<SkillPositionInfo>();

    public override void ExecuteEffect()
    {
        BattleFocusTargeter.instance.BeginTargeting(positioningPanelPrefab, skillPositions, SpawnPillarAtTarget);
    }

    private void SpawnPillarAtTarget(GameObject targetPositionObject)
    {
        if (targetPositionObject == null) return;

        // ... (레벨 및 능력치 계산 코드는 그대로) ...
        int levelIndex = 0;
        if (GameSession.instance != null && SaveLoadManager.instance != null)
        {
            string charID = GameSession.instance.selectedCharacter.characterID;
            int charLevel = SaveLoadManager.instance.gameData.characterLevels[charID];
            levelIndex = Mathf.Clamp(charLevel - 1, 0, durationByLevel.Count - 1);
        }
        float duration = durationByLevel[levelIndex];
        int dps = damagePerSecondByLevel[levelIndex];

        // 기둥 생성
        GameObject pillarGO = Instantiate(pillarPrefab, targetPositionObject.transform.position, Quaternion.identity);
        pillarGO.GetComponent<RyuPillar>().Initialize(dps, duration);

        if (characterImagePrefab != null)
        {
            // Quaternion.identity 대신 Quaternion.Euler(0, 0, 90)으로 변경하여 Z축으로 90도 회전
            Quaternion characterRotation = Quaternion.Euler(0, 0, 90);
            GameObject charImage = Instantiate(characterImagePrefab, targetPositionObject.transform.position, characterRotation);
            Destroy(charImage, duration + 0.1f);
        }

        Debug.Log($"<color=blue>RYU 스킬 발동!</color> Lv.{levelIndex + 1} (지속:{duration}s, DPS:{dps})");
    }
}
