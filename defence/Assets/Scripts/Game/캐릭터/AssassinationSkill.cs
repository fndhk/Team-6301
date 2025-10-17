using UnityEngine;

[CreateAssetMenu(fileName = "AssassinationSkill", menuName = "TowerDefense/Skills/Assassination Skill")]
public class AssassinationSkill : ItemEffect
{
    [Header("스킬 설정")]
    [Tooltip("즉사 시 대상의 몸에 생성될 이펙트 프리팹")]
    public GameObject assassinationEffectPrefab;

    public override void ExecuteEffect()
    {
        // 1. EnemyManager에서 가장 앞에 있는 적을 찾습니다.
        if (EnemyManager.instance == null)
        {
            Debug.LogError("AssassinationSkill: EnemyManager를 찾을 수 없습니다!");
            return;
        }

        Enemy target = EnemyManager.instance.FindFrontmostEnemy();

        // 2. 타겟을 찾았다면
        if (target != null)
        {
            // 3. 해당 적의 InstantKill 함수를 호출하여 즉사시킵니다.
            target.InstantKill(assassinationEffectPrefab);
        }
        else
        {
            // 4. 타겟이 없다면 (모든 적을 처치한 경우)
            Debug.Log("AssassinationSkill: 암살할 대상이 없습니다.");
        }
    }
}
