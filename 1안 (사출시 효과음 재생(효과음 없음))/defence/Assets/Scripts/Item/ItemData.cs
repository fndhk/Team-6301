// ���� �̸�: ItemData.cs (���� Ȯ�� ����)
using UnityEngine;

// ���� EffectType�� ItemType enum�� �ִ� �κ��� ������ �����Ǿ�� �մϴ�. ����

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("������ ����")]
    public string itemName = "�� ������";
    public Sprite icon = null;

    [Header("������ ȿ��")]
    // �� �������� ����� 'ȿ��'�� ScriptableObject ���·� �����մϴ�.
    public ItemEffect itemEffect;

    [Header("������ ���� ����")]
    public int maxStackSize = 16;
}