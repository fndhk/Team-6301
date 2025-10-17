using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "HealSkill", menuName = "TowerDefense/Skills/Heal Skill")]
public class HealSkill : ItemEffect
{
    [Header("ġ�� ����")]
    [Tooltip("ĳ���� ���� 1, 2, 3�� ���� �⺻ ȸ����")]
    public List<int> healAmountByLevel = new List<int> { 200, 300, 400 };

    public override void ExecuteEffect()
    {
        // 1. CoreFacility �ν��Ͻ� ã��
        CoreFacility core = FindFirstObjectByType<CoreFacility>();
        if (core == null)
        {
            Debug.LogError("HealSkill: ������ CoreFacility�� ã�� �� �����ϴ�!");
            return;
        }

        // 2. ���� ĳ���� ���� ��������
        if (GameSession.instance == null || SaveLoadManager.instance == null) return;
        string charID = GameSession.instance.selectedCharacter.characterID;
        if (!SaveLoadManager.instance.gameData.characterLevels.ContainsKey(charID)) return;

        int charLevel = SaveLoadManager.instance.gameData.characterLevels[charID];
        int levelIndex = charLevel - 1;

        if (levelIndex < 0) return;

        // 3. ������ �´� ȸ���� ���
        int currentHealAmount = 0;
        if (levelIndex < healAmountByLevel.Count)
        {
            currentHealAmount = healAmountByLevel[levelIndex];
        }

        if (currentHealAmount <= 0)
        {
            Debug.LogWarning("HealSkill�� ���� ���� ȸ������ 0 �����Դϴ�.");
            return;
        }

        // 4. CoreFacility�� Heal �Լ� ȣ��
        core.Heal(currentHealAmount);
    }
}
