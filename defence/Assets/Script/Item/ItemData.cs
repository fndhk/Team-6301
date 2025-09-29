using UnityEngine;
public enum EffectType
{
    AttackSpeed,
    Damage
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("아이템 정보")]
    public string itemName = "새 아이템";
    public Sprite icon = null;

    [Header("아이템 효과")]
    public EffectType effectType; // 이 아이템이 어떤 종류의 효과를 가졌는지
    public float effectValue;     // 효과의 값 (예: 2배, 1.5배)
    public float buffDuration = 10f; // 효과 지속 시간

    [Header("아이템 수량 정보")]
    public int maxStackSize = 16;
}