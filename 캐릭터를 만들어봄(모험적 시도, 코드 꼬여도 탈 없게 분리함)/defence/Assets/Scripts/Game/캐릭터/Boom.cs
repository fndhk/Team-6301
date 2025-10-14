// ���� �̸�: Boom.cs (�� ����)
using UnityEngine;

[CreateAssetMenu(fileName = "BoomSkill", menuName = "TowerDefense/Skills/Boom Skill")]
public class Boom : ItemEffect
{
    public int damage = 300;

    public override void ExecuteEffect()
    {
        // �� ��ų ���� EnemyManager�� ȣ���մϴ�.
        if (EnemyManager.instance != null)
        {
            EnemyManager.instance.DamageAllEnemies(damage);
        }
    }
}