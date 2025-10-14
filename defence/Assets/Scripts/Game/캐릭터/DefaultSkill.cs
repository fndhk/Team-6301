// 파일 이름: DefaultSkill.cs (새 파일)
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultSkill", menuName = "TowerDefense/Skills/Default Skill")]
public class DefaultSkill : ItemEffect
{
    public float duration = 10f;
    public int levelIncrease = 1;

    public override void ExecuteEffect()
    {
        // 이 스킬은 TowerManager에 있는 특정 함수를 호출할 것입니다.
        if (TowerManager.instance != null)
        {
            TowerManager.instance.ApplyTempLevelBuffToAllTowers(levelIncrease, duration);
        }
    }
}