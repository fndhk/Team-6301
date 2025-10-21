using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TempoMagicSkill", menuName = "TowerDefense/Skills/Tempo Magic Skill")]
public class TempoMagicSkill : ItemEffect
{
    [Header("스킬 효과 설정")]
    [Tooltip("캐릭터 레벨 1, 2, 3일 때의 스킬 지속시간")]
    public List<float> durationByLevel = new List<float> { 5f, 7f, 10f };

    [Header("적 디버프")]
    [Tooltip("적 이동속도 배율 (예: 0.7 = 30% 감소). 레벨 1, 2, 3 순서.")]
    public List<float> enemySpeedMultiplierByLevel = new List<float> { 0.7f, 0.6f, 0.5f };

    [Header("타워 버프")]
    [Tooltip("타워 공격속도 배율 (예: 1.5 = 50% 증가). 레벨 1, 2, 3 순서.")]
    public List<float> towerAttackSpeedMultiplierByLevel = new List<float> { 1.2f, 1.4f, 1.6f };

    public override void ExecuteEffect()
    {
        // 현재 캐릭터 레벨 가져오기
        if (GameSession.instance == null || SaveLoadManager.instance == null) return;
        string charID = GameSession.instance.selectedCharacter.characterID;
        if (!SaveLoadManager.instance.gameData.characterLevels.ContainsKey(charID)) return;

        int charLevel = SaveLoadManager.instance.gameData.characterLevels[charID];
        int levelIndex = charLevel - 1;

        if (levelIndex < 0) return;

        // 레벨에 맞는 계수 가져오기
        float duration = (levelIndex < durationByLevel.Count) ? durationByLevel[levelIndex] : 5f;
        float enemyMultiplier = (levelIndex < enemySpeedMultiplierByLevel.Count) ? enemySpeedMultiplierByLevel[levelIndex] : 0.7f;
        float towerMultiplier = (levelIndex < towerAttackSpeedMultiplierByLevel.Count) ? towerAttackSpeedMultiplierByLevel[levelIndex] : 1.2f;

        // 매니저를 통해 효과 적용
        if (EnemyManager.instance != null)
        {
            EnemyManager.instance.ApplySpeedDebuffToAll(enemyMultiplier, duration);
        }

        if (TowerManager.instance != null)
        {
            TowerManager.instance.ApplyAttackSpeedBuffToAll(towerMultiplier, duration);
        }
    }
}
