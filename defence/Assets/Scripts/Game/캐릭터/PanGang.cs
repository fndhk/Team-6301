// 파일 이름: PanGang.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PanGangSkill", menuName = "TowerDefense/Skills/PanGang Skill")]
public class PanGang : ItemEffect
{
    [Header("레벨별 스킬 계수")]
    [Tooltip("1, 2, 3레벨일 때의 판정 강화 지속시간(초)")]
    public List<float> durationByLevel = new List<float> { 5f, 7f, 10f };

    [Header("스킬 효과")]
    [Tooltip("판정 범위를 몇 배로 넓힐지 (예: 1.5 = 50% 넓어짐)")]
    public float windowMultiplier = 1.5f;

    public override void ExecuteEffect()
    {
        if (RhythmInputManager.instance == null)
        {
            Debug.LogError("PanGangSkill: RhythmInputManager.instance가 없습니다!");
            return;
        }

        // 1. 현재 캐릭터 레벨 가져오기
        int charLevel = 1; // 기본값
        if (GameSession.instance != null && GameSession.instance.selectedCharacter != null && SaveLoadManager.instance != null)
        {
            string charID = GameSession.instance.selectedCharacter.characterID;
            if (SaveLoadManager.instance.gameData.characterLevels.TryGetValue(charID, out int foundLevel))
            {
                charLevel = foundLevel;
            }
        }

        // 2. 레벨에 맞는 지속시간 계산
        // level 1 -> index 0, level 2 -> index 1...
        int levelIndex = Mathf.Clamp(charLevel - 1, 0, durationByLevel.Count - 1);
        float currentDuration = durationByLevel[levelIndex];

        // 3. RhythmInputManager에 버프 적용 요청
        RhythmInputManager.instance.ApplyJudgmentBuff(windowMultiplier, currentDuration);

        Debug.Log($"<color=cyan>판강 스킬 발동! (Lv.{charLevel}) {currentDuration}초간 판정 범위 {windowMultiplier}배 증가!</color>");
    }
}