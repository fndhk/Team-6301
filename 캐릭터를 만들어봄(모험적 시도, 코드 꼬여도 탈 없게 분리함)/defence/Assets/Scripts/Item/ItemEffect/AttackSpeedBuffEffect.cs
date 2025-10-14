// ���� �̸�: AttackSpeedBuffEffect.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New AttackSpeedBuffEffect", menuName = "TowerDefense/ItemEffects/Attack Speed Buff")]
public class AttackSpeedBuffEffect : ItemEffect
{
    [Header("���� ����")]
    public float attackSpeedMultiplier = 2f; // ���� �ӵ� ����
    public float duration = 10f;             // ���� �ð�

    public override void ExecuteEffect()
    {
        // TowerManager�� �ƴ� RhythmManager�� �Լ��� ȣ���ϵ��� �����մϴ�.
        RhythmManager.instance.ApplySpeedBuff(attackSpeedMultiplier, duration);
    }
}