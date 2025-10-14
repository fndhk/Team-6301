// 파일 이름: Ice.cs (새 파일)
using UnityEngine;

[CreateAssetMenu(fileName = "IceSkill", menuName = "TowerDefense/Skills/Ice Skill")]
public class Ice : ItemEffect
{
    public float duration = 5f;

    public override void ExecuteEffect()
    {
        // 이 스킬은 모든 적을 관리할 EnemyManager를 호출할 것입니다.
        if (EnemyManager.instance != null)
        {
            EnemyManager.instance.FreezeAllEnemies(duration);
        }
    }
}