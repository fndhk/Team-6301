using UnityEngine;
using System.Collections.Generic;

// �ٸ� ��ũ��Ʈ���� ������ �� �ֵ��� Ŭ������ �ٱ����� ���� public���� �����մϴ�.
[System.Serializable]
public class CharacterLevelGrowth
{
    [Tooltip("�ھ�(����)�� �⺻ ü�¿� �������� ���ʽ�")]
    public int coreHealthBonus = 0;
    [Tooltip("��� Ÿ���� �⺻ ���ݷ¿� �������� ���ʽ�")]
    public int towerAttackDamageBonus = 0;
    [Tooltip("��� Ÿ���� �⺻ ���ݼӵ��� �������� ���� (1.2 = 20% ����)")]
    public float towerAttackSpeedMultiplier = 1f;
}


[CreateAssetMenu(fileName = "NewCharacter", menuName = "TowerDefense/Character Data")]
public class CharacterData : ScriptableObject
{
    public enum CharacterRarity { R, SR, SSR }
    [Header("ĳ���� ���")]
    public CharacterRarity rarity;
    public string characterID;

    [Header("ĳ���� ����")]
    public string characterName;
    [TextArea(3, 10)]
    public string characterDescription;
    public Sprite characterIcon;
    public Sprite characterIllustration;

    [Header("��ų ����")]
    public string skillName;
    [TextArea(2, 5)]
    public string skillDescription;
    public Sprite skillCutsceneImage;
    public ItemEffect characterSkill;

    [Header("ĳ���� �⺻ �ɷ�ġ (1���� ����)")]
    public CharacterLevelGrowth baseStats;

    [Header("������ �߰� ���� �ɷ�ġ")]
    [Tooltip("Size�� 3���� �����ϼ���. Element 0=1����, 1=2����, 2=3����")]
    public List<CharacterLevelGrowth> levelGrowthStats = new List<CharacterLevelGrowth>();

    /// <summary>
    /// ������ ������ ���� �⺻ �ɷ�ġ�� ���� �ɷ�ġ�� �ջ��� ���� �ɷ�ġ�� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="level">ĳ������ ���� ���� (1, 2, 3)</param>
    /// <returns>�ջ�� ���� �ɷ�ġ</returns>
    public CharacterLevelGrowth GetTotalStatsForLevel(int level)
    {
        CharacterLevelGrowth totalStats = new CharacterLevelGrowth
        {
            coreHealthBonus = baseStats.coreHealthBonus,
            towerAttackDamageBonus = baseStats.towerAttackDamageBonus,
            towerAttackSpeedMultiplier = baseStats.towerAttackSpeedMultiplier
        };

        // ������ �´� ����ġ�� ���մϴ�.
        // level 1 -> index 0, level 2 -> index 1...
        int index = level - 1;
        if (index >= 0 && index < levelGrowthStats.Count)
        {
            CharacterLevelGrowth growth = levelGrowthStats[index];
            totalStats.coreHealthBonus += growth.coreHealthBonus;
            totalStats.towerAttackDamageBonus += growth.towerAttackDamageBonus;
            // ���� �ӵ��� �������̹Ƿ� 1�� �������� �����ݴϴ�. (��: 1f + 0.1f = 1.1f)
            totalStats.towerAttackSpeedMultiplier += (growth.towerAttackSpeedMultiplier - 1f);
        }

        return totalStats;
    }
}

