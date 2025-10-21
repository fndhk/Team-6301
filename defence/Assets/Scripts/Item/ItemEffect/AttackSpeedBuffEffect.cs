// 파일 이름: AttackSpeedBuffEffect.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New AttackSpeedBuffEffect", menuName = "TowerDefense/ItemEffects/Attack Speed Buff")]
public class AttackSpeedBuffEffect : ItemEffect
{
    [Header("버프 정보")]
    [Tooltip("타워의 공격 속도를 몇 배로 만들지 (예: 2 = 2배 빠름)")]
    public float attackSpeedMultiplier = 2f; // 공격 속도 배율
    [Tooltip("버프 지속 시간 (초)")]
    public float duration = 10f;

    public override void ExecuteEffect()
    {
        // TowerManager가 아닌 RhythmManager의 함수를 호출하도록 변경합니다.
        // RhythmManager.instance.ApplySpeedBuff(attackSpeedMultiplier, duration);

        if (TowerManager.instance != null)
        {
            TowerManager.instance.ApplyAttackSpeedBuffToAll(attackSpeedMultiplier, duration);
            Debug.Log($"공격 속도 아이템 사용! {duration}초간 모든 타워 공속 {attackSpeedMultiplier}배 적용");
        }
        else
        {
            Debug.LogError("AttackSpeedBuffEffect: TowerManager 인스턴스를 찾을 수 없습니다!");
        }
    }
}