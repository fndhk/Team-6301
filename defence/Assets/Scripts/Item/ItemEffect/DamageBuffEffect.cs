// ���� �̸�: DamageBuffEffect.cs
using UnityEngine;

// CreateAssetMenu�� ����ϸ� ����Ƽ �������� Assets/Create �޴����� �� ��ũ��Ʈ�� '������ ����'�� ���� ���� �� �ֽ��ϴ�.
[CreateAssetMenu(fileName = "New DamageBuffEffect", menuName = "TowerDefense/ItemEffects/Damage Buff")]
public class DamageBuffEffect : ItemEffect
{
    [Header("���� ����")]
    public float damageMultiplier = 1.5f; // ������ ����
    public float duration = 10f;          // ���� �ð�

    public override void ExecuteEffect()
    {
        // TowerManager�� ���� ����� �״�� ȣ���մϴ�.
        // �� ȿ���� �ڽ��� � ������ ��� �ϴ����� ���� '������'�� ������ �ֽ��ϴ�.
        TowerManager.instance.ApplyDamageBuff(damageMultiplier, duration);
    }
}