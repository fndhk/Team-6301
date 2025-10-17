using UnityEngine;
using System.Collections.Generic; // List�� ����ϱ� ���� �߰�

[CreateAssetMenu(fileName = "IceSkill", menuName = "TowerDefense/Skills/Ice Skill")]
public class Ice : ItemEffect
{
    // ���� ������ ���� ���ӽð� ����Ʈ �߰� ����
    [Header("������ ��ų ���")]
    [Tooltip("1, 2, 3������ ���� ���� ���ӽð�(��)")]
    public List<float> durationByLevel = new List<float>(3);

    public override void ExecuteEffect()
    {
        float currentDuration = 5f; // �⺻��
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
            Debug.Log($"<color=cyan>Ice ��ų �ߵ�! ({currentDuration}�ʰ� ����)</color>");
            EnemyManager.instance.FreezeAllEnemies(currentDuration);
        }
    }
}