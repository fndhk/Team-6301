// ���� �̸�: DefaultSkill.cs (�� ����)
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultSkill", menuName = "TowerDefense/Skills/Default Skill")]
public class DefaultSkill : ItemEffect
{
    public float duration = 10f;
    public int levelIncrease = 1;

    public override void ExecuteEffect()
    {
        // �� ��ų�� TowerManager�� �ִ� Ư�� �Լ��� ȣ���� ���Դϴ�.
        if (TowerManager.instance != null)
        {
            TowerManager.instance.ApplyTempLevelBuffToAllTowers(levelIncrease, duration);
        }
    }
}