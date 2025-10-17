using UnityEngine;

[CreateAssetMenu(fileName = "AssassinationSkill", menuName = "TowerDefense/Skills/Assassination Skill")]
public class AssassinationSkill : ItemEffect
{
    [Header("��ų ����")]
    [Tooltip("��� �� ����� ���� ������ ����Ʈ ������")]
    public GameObject assassinationEffectPrefab;

    public override void ExecuteEffect()
    {
        // 1. EnemyManager���� ���� �տ� �ִ� ���� ã���ϴ�.
        if (EnemyManager.instance == null)
        {
            Debug.LogError("AssassinationSkill: EnemyManager�� ã�� �� �����ϴ�!");
            return;
        }

        Enemy target = EnemyManager.instance.FindFrontmostEnemy();

        // 2. Ÿ���� ã�Ҵٸ�
        if (target != null)
        {
            // 3. �ش� ���� InstantKill �Լ��� ȣ���Ͽ� ����ŵ�ϴ�.
            target.InstantKill(assassinationEffectPrefab);
        }
        else
        {
            // 4. Ÿ���� ���ٸ� (��� ���� óġ�� ���)
            Debug.Log("AssassinationSkill: �ϻ��� ����� �����ϴ�.");
        }
    }
}
