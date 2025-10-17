using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TempoMagicSkill", menuName = "TowerDefense/Skills/Tempo Magic Skill")]
public class TempoMagicSkill : ItemEffect
{
    [Header("��ų ȿ�� ����")]
    [Tooltip("ĳ���� ���� 1, 2, 3�� ���� ��ų ���ӽð�")]
    public List<float> durationByLevel = new List<float> { 5f, 7f, 10f };

    [Header("�� �����")]
    [Tooltip("�� �̵��ӵ� ���� (��: 0.7 = 30% ����). ���� 1, 2, 3 ����.")]
    public List<float> enemySpeedMultiplierByLevel = new List<float> { 0.7f, 0.6f, 0.5f };

    [Header("Ÿ�� ����")]
    [Tooltip("Ÿ�� ���ݼӵ� ���� (��: 1.5 = 50% ����). ���� 1, 2, 3 ����.")]
    public List<float> towerAttackSpeedMultiplierByLevel = new List<float> { 1.2f, 1.4f, 1.6f };

    public override void ExecuteEffect()
    {
        // ���� ĳ���� ���� ��������
        if (GameSession.instance == null || SaveLoadManager.instance == null) return;
        string charID = GameSession.instance.selectedCharacter.characterID;
        if (!SaveLoadManager.instance.gameData.characterLevels.ContainsKey(charID)) return;

        int charLevel = SaveLoadManager.instance.gameData.characterLevels[charID];
        int levelIndex = charLevel - 1;

        if (levelIndex < 0) return;

        // ������ �´� ��� ��������
        float duration = (levelIndex < durationByLevel.Count) ? durationByLevel[levelIndex] : 5f;
        float enemyMultiplier = (levelIndex < enemySpeedMultiplierByLevel.Count) ? enemySpeedMultiplierByLevel[levelIndex] : 0.7f;
        float towerMultiplier = (levelIndex < towerAttackSpeedMultiplierByLevel.Count) ? towerAttackSpeedMultiplierByLevel[levelIndex] : 1.2f;

        // �Ŵ����� ���� ȿ�� ����
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
