// 파일 이름: AttackSpeedBuffEffect.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New AttackSpeedBuffEffect", menuName = "TowerDefense/ItemEffects/Attack Speed Buff")]
public class AttackSpeedBuffEffect : ItemEffect
{
    [Header("버프 정보")]
    public float attackSpeedMultiplier = 2f; // 공격 속도 배율
    public float duration = 10f;             // 지속 시간

    public override void ExecuteEffect()
    {
        // TowerManager가 아닌 RhythmManager의 함수를 호출하도록 변경합니다.
        RhythmManager.instance.ApplySpeedBuff(attackSpeedMultiplier, duration);
    }
}