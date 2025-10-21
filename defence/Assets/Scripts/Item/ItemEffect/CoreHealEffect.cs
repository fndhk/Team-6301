// 파일 이름: CoreHealEffect.cs
using UnityEngine;

[CreateAssetMenu(fileName = "CoreHealEffect", menuName = "TowerDefense/ItemEffects/Core Heal")]
public class CoreHealEffect : ItemEffect
{
    public int baseHealAmount = 200; // 기본 회복량
    public int increasePerLevel = 10; // 레벨당 증가량

    public override void ExecuteEffect()
    {
        CoreFacility core = FindFirstObjectByType<CoreFacility>();
        if (core == null) return;

        int upgradeLevel = SaveLoadManager.instance.gameData.quickSlotUpgradeLevel;
        int finalHeal = baseHealAmount + (upgradeLevel * increasePerLevel);

        core.Heal(finalHeal);
    }
}