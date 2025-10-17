using UnityEngine;
using System.Collections.Generic; // List를 사용하기 위해 추가

[CreateAssetMenu(fileName = "DefaultSkill", menuName = "TowerDefense/Skills/Default Skill")]
public class DefaultSkill : ItemEffect
{
    // ▼▼▼ 레벨별 계수 리스트 추가 ▼▼▼
    [Header("레벨별 스킬 계수")]
    [Tooltip("1, 2, 3레벨일 때의 타워 레벨 증가량")]
    public List<int> levelIncreaseByLevel = new List<int>(3);
    [Tooltip("1, 2, 3레벨일 때의 스킬 지속시간(초)")]
    public List<float> durationByLevel = new List<float>(3);

    public override void ExecuteEffect()
    {
        float currentDuration = 10f; // 기본값
        int levelIncrease = 1;       // 기본값

        if (GameSession.instance != null && GameSession.instance.selectedCharacter != null && SaveLoadManager.instance != null)
        {
            string charID = GameSession.instance.selectedCharacter.characterID;

            int charLevel;
            if (SaveLoadManager.instance.gameData.characterLevels.TryGetValue(charID, out charLevel) && charLevel > 0)
            {
                int index = charLevel - 1;
                if (index < levelIncreaseByLevel.Count)
                {
                    levelIncrease = levelIncreaseByLevel[index];
                }
                if (index < durationByLevel.Count)
                {
                    currentDuration = durationByLevel[index];
                }
            }
        }

        if (TowerManager.instance != null)
        {
            Debug.Log($"<color=green>Default 스킬 발동! (모든 타워 레벨 +{levelIncrease}, {currentDuration}초간)</color>");
            TowerManager.instance.ApplyTempLevelBuffToAllTowers(levelIncrease, currentDuration);
        }
    }
}