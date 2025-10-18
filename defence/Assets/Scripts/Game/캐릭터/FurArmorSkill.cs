// ���� �̸�: FurArmorSkill.cs
using UnityEngine;
using static UnityEngine.Android.AndroidGame;

[CreateAssetMenu(fileName = "FurArmorSkill", menuName = "TowerDefense/Skills/Fur Armor Skill")]
public class FurArmorSkill : ItemEffect
{
    [Header("��ų ���")]
    [Tooltip("���� ������ ������ �Ҹ�Ǵ� ��ų ������ ��")]
    public float gaugeCostPerHit = 5f;
    [Tooltip("���� �о�� �� (�Ÿ�)")]
    public float pushbackForce = 2f;
    [Tooltip("���� �з����� �ð� (��)")]
    public float pushbackDuration = 0.2f;

    public FurArmorSkill()
    {
        // �� ��ų�� �������� �� ���� �ڵ����� �ߵ��ǵ��� ����
        isAutomatic = true;
    }

    public override void ExecuteEffect()
    {
        // 1. CoreFacility ������Ʈ�� ã���ϴ�.
        CoreFacility core = FindFirstObjectByType<CoreFacility>();
        if (core == null)
        {
            Debug.LogError("FurArmorSkill: ������ CoreFacility�� ã�� �� �����ϴ�!");
            return;
        }

        // 2. CoreFacility�� FurShield ������Ʈ�� �̹� �ִ��� Ȯ���մϴ�.
        if (core.GetComponent<FurShield>() != null)
        {
            // �̹� ��ų�� Ȱ��ȭ �����̹Ƿ� �ƹ��͵� ���� ����
            return;
        }

        // 3. CoreFacility�� FurShield ������Ʈ�� �߰��ϰ� �ʱ�ȭ�մϴ�.
        FurShield shield = core.gameObject.AddComponent<FurShield>();
        shield.Initialize(gaugeCostPerHit, pushbackForce, pushbackDuration);

        Debug.Log("<color=brown>�� ���� Ȱ��ȭ!</color> �������� ������ �ݰ��մϴ�.");
    }
}