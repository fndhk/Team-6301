using UnityEngine;
using System.Collections.Generic; // List를 사용하기 위해 추가

[CreateAssetMenu(fileName = "IceSkill", menuName = "TowerDefense/Skills/Ice Skill")]
public class Ice : ItemEffect
{
    // ▼▼▼ 레벨별 빙결 지속시간 리스트 추가 ▼▼▼
    [Header("레벨별 스킬 계수")]
    [Tooltip("1, 2, 3레벨일 때의 빙결 지속시간(초)")]
    public List<float> durationByLevel = new List<float>(3);

    public override void ExecuteEffect()
    {
        float currentDuration = 5f; // 기본값
        if (GameSession.instance != null && GameSession.instance.selectedCharacter != null && SaveLoadManager.instance != null)
        {
            string charID = GameSession.instance.selectedCharacter.characterID;

            int charLevel;
            if (SaveLoadManager.instance.gameData.characterLevels.TryGetValue(charID, out charLevel) && charLevel > 0)
            {
                int index = charLevel - 1;
                if (index < durationByLevel.Count)
                {
                    currentDuration = durationByLevel[index];
                }
            }
        }

        if (EnemyManager.instance != null)
        {
            Debug.Log($"<color=cyan>Ice 스킬 발동! ({currentDuration}초간 빙결)</color>");
            EnemyManager.instance.FreezeAllEnemies(currentDuration);
        }
    }
}