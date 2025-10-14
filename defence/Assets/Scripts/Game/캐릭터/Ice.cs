// ���� �̸�: Ice.cs (�� ����)
using UnityEngine;

[CreateAssetMenu(fileName = "IceSkill", menuName = "TowerDefense/Skills/Ice Skill")]
public class Ice : ItemEffect
{
    public float duration = 5f;

    public override void ExecuteEffect()
    {
        // �� ��ų�� ��� ���� ������ EnemyManager�� ȣ���� ���Դϴ�.
        if (EnemyManager.instance != null)
        {
            EnemyManager.instance.FreezeAllEnemies(duration);
        }
    }
}