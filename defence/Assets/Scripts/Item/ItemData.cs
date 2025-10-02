using UnityEngine;
public enum EffectType
{
    AttackSpeed,
    Damage
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("������ ����")]
    public string itemName = "�� ������";
    public Sprite icon = null;

    [Header("������ ȿ��")]
    public EffectType effectType; // �� �������� � ������ ȿ���� ��������
    public float effectValue;     // ȿ���� �� (��: 2��, 1.5��)
    public float buffDuration = 10f; // ȿ�� ���� �ð�

    [Header("������ ���� ����")]
    public int maxStackSize = 16;
}