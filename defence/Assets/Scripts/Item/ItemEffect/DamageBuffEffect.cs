// 파일 이름: DamageBuffEffect.cs
using UnityEngine;

// CreateAssetMenu를 사용하면 유니티 에디터의 Assets/Create 메뉴에서 이 스크립트의 '데이터 파일'을 쉽게 만들 수 있습니다.
[CreateAssetMenu(fileName = "New DamageBuffEffect", menuName = "TowerDefense/ItemEffects/Damage Buff")]
public class DamageBuffEffect : ItemEffect
{
    public float baseMultiplier = 2f; // 기본 배율
    public float increasePerLevel = 0.2f; // 레벨당 증가량
    public float duration = 10f;

    public override void ExecuteEffect()
    {
        int upgradeLevel = SaveLoadManager.instance.gameData.quickSlotUpgradeLevel;
        float finalMultiplier = baseMultiplier + (upgradeLevel * increasePerLevel);

        TowerManager.instance.ApplyDamageBuff(finalMultiplier, duration);
    }
}