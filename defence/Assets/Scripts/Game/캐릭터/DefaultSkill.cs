using UnityEngine;
using System.Collections.Generic; // List�� ����ϱ� ���� �߰�

[CreateAssetMenu(fileName = "DefaultSkill", menuName = "TowerDefense/Skills/Default Skill")]
public class DefaultSkill : ItemEffect
{
    // ���� ������ ��� ����Ʈ �߰� ����
    [Header("������ ��ų ���")]
    [Tooltip("1, 2, 3������ ���� Ÿ�� ���� ������")]
    public List<int> levelIncreaseByLevel = new List<int>(3);
    [Tooltip("1, 2, 3������ ���� ��ų ���ӽð�(��)")]
    public List<float> durationByLevel = new List<float>(3);

    public override void ExecuteEffect()
    {
        float currentDuration = 10f; // �⺻��
        int levelIncrease = 1;       // �⺻��

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
            Debug.Log($"<color=green>Default ��ų �ߵ�! (��� Ÿ�� ���� +{levelIncrease}, {currentDuration}�ʰ�)</color>");
            TowerManager.instance.ApplyTempLevelBuffToAllTowers(levelIncrease, currentDuration);
        }
    }
}