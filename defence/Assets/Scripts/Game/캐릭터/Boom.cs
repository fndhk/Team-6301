// 파일 이름: Boom.cs (새 파일)
using UnityEngine;

[CreateAssetMenu(fileName = "BoomSkill", menuName = "TowerDefense/Skills/Boom Skill")]
public class Boom : ItemEffect
{
    public int damage = 300;

    public override void ExecuteEffect()
    {
        // 이 스킬 역시 EnemyManager를 호출합니다.
        if (EnemyManager.instance != null)
        {
            EnemyManager.instance.DamageAllEnemies(damage);
        }
    }
}