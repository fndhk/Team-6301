// 파일 이름: DamageBuffEffect.cs
using UnityEngine;

// CreateAssetMenu를 사용하면 유니티 에디터의 Assets/Create 메뉴에서 이 스크립트의 '데이터 파일'을 쉽게 만들 수 있습니다.
[CreateAssetMenu(fileName = "New DamageBuffEffect", menuName = "TowerDefense/ItemEffects/Damage Buff")]
public class DamageBuffEffect : ItemEffect
{
    [Header("버프 정보")]
    public float damageMultiplier = 1.5f; // 데미지 배율
    public float duration = 10f;          // 지속 시간

    public override void ExecuteEffect()
    {
        // TowerManager의 기존 기능을 그대로 호출합니다.
        // 이 효과는 자신이 어떤 버프를 줘야 하는지에 대한 '데이터'만 가지고 있습니다.
        TowerManager.instance.ApplyDamageBuff(damageMultiplier, duration);
    }
}