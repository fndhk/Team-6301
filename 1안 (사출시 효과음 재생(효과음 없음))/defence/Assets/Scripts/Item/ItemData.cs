// ���� �̸�: ItemData.cs
using UnityEngine;

// ������ ȿ���� ����
public enum EffectType
{
    AttackSpeed,
    Damage
}

// �������� ū ī�װ�
public enum ItemType
{
    Buff, // �Ϲ����� ���� ������
    TowerActivation // Ÿ���� Ȱ��ȭ��Ű�� Ư�� ������
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("������ ����")]
    public string itemName = "�� ������";
    public Sprite icon = null;
    public ItemType itemType; // ������ ������ ������ �� �ִ� ���� �߰�

    [Header("������ ȿ�� (������)")]
    public EffectType effectType;
    public float effectValue;
    public float buffDuration = 10f;

    [Header("������ ���� ����")]
    public int maxStackSize = 16;
}