using UnityEngine;
using System.Collections.Generic;

// 다른 스크립트에서 참조할 수 있도록 클래스를 바깥으로 빼고 public으로 선언합니다.
[System.Serializable]
public class CharacterLevelGrowth
{
    [Tooltip("코어(기지)의 기본 체력에 더해지는 보너스")]
    public int coreHealthBonus = 0;
    [Tooltip("모든 타워의 기본 공격력에 더해지는 보너스")]
    public int towerAttackDamageBonus = 0;
    [Tooltip("모든 타워의 기본 공격속도에 곱해지는 배율 (1.2 = 20% 빠름)")]
    public float towerAttackSpeedMultiplier = 1f;
}


[CreateAssetMenu(fileName = "NewCharacter", menuName = "TowerDefense/Character Data")]
public class CharacterData : ScriptableObject
{
    public enum CharacterRarity { R, SR, SSR }
    [Header("캐릭터 등급")]
    public CharacterRarity rarity;
    public string characterID;

    [Header("캐릭터 정보")]
    public string characterName;
    [TextArea(3, 10)]
    public string characterDescription;
    public Sprite characterIcon;
    public Sprite characterIllustration;

    [Header("스킬 정보")]
    public string skillName;
    [TextArea(2, 5)]
    public string skillDescription;
    public Sprite skillCutsceneImage;
    public ItemEffect characterSkill;

    [Header("캐릭터 기본 능력치 (1레벨 기준)")]
    public CharacterLevelGrowth baseStats;

    [Header("레벨별 추가 성장 능력치")]
    [Tooltip("Size를 3으로 설정하세요. Element 0=1레벨, 1=2레벨, 2=3레벨")]
    public List<CharacterLevelGrowth> levelGrowthStats = new List<CharacterLevelGrowth>();

    /// <summary>
    /// 지정된 레벨에 대한 기본 능력치와 성장 능력치를 합산한 최종 능력치를 반환합니다.
    /// </summary>
    /// <param name="level">캐릭터의 현재 레벨 (1, 2, 3)</param>
    /// <returns>합산된 최종 능력치</returns>
    public CharacterLevelGrowth GetTotalStatsForLevel(int level)
    {
        CharacterLevelGrowth totalStats = new CharacterLevelGrowth
        {
            coreHealthBonus = baseStats.coreHealthBonus,
            towerAttackDamageBonus = baseStats.towerAttackDamageBonus,
            towerAttackSpeedMultiplier = baseStats.towerAttackSpeedMultiplier
        };

        // 레벨에 맞는 성장치를 더합니다.
        // level 1 -> index 0, level 2 -> index 1...
        int index = level - 1;
        if (index >= 0 && index < levelGrowthStats.Count)
        {
            CharacterLevelGrowth growth = levelGrowthStats[index];
            totalStats.coreHealthBonus += growth.coreHealthBonus;
            totalStats.towerAttackDamageBonus += growth.towerAttackDamageBonus;
            // 공격 속도는 곱연산이므로 1을 기준으로 더해줍니다. (예: 1f + 0.1f = 1.1f)
            totalStats.towerAttackSpeedMultiplier += (growth.towerAttackSpeedMultiplier - 1f);
        }

        return totalStats;
    }
}

