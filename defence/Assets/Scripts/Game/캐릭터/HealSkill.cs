using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "HealSkill", menuName = "TowerDefense/Skills/Heal Skill")]
public class HealSkill : ItemEffect
{
    [Header("치유 설정")]
    [Tooltip("캐릭터 레벨 1, 2, 3일 때의 기본 회복량")]
    public List<int> healAmountByLevel = new List<int> { 200, 300, 400 };

    public override void ExecuteEffect()
    {
        // 1. CoreFacility 인스턴스 찾기
        CoreFacility core = FindFirstObjectByType<CoreFacility>();
        if (core == null)
        {
            Debug.LogError("HealSkill: 씬에서 CoreFacility를 찾을 수 없습니다!");
            return;
        }

        // 2. 현재 캐릭터 레벨 가져오기
        if (GameSession.instance == null || SaveLoadManager.instance == null) return;
        string charID = GameSession.instance.selectedCharacter.characterID;
        if (!SaveLoadManager.instance.gameData.characterLevels.ContainsKey(charID)) return;

        int charLevel = SaveLoadManager.instance.gameData.characterLevels[charID];
        int levelIndex = charLevel - 1;

        if (levelIndex < 0) return;

        // 3. 레벨에 맞는 회복량 계산
        int currentHealAmount = 0;
        if (levelIndex < healAmountByLevel.Count)
        {
            currentHealAmount = healAmountByLevel[levelIndex];
        }

        if (currentHealAmount <= 0)
        {
            Debug.LogWarning("HealSkill의 현재 레벨 회복량이 0 이하입니다.");
            return;
        }

        // 4. CoreFacility의 Heal 함수 호출
        core.Heal(currentHealAmount);
    }
}
