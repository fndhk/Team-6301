// 파일 이름: ItemData.cs
using UnityEngine;

// 아이템 효과의 종류
public enum EffectType
{
    AttackSpeed,
    Damage
}

// 아이템의 큰 카테고리
public enum ItemType
{
    Buff, // 일반적인 버프 아이템
    TowerActivation // 타워를 활성화시키는 특별 아이템
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("아이템 정보")]
    public string itemName = "새 아이템";
    public Sprite icon = null;
    public ItemType itemType; // 아이템 종류를 선택할 수 있는 변수 추가

    [Header("아이템 효과 (버프용)")]
    public EffectType effectType;
    public float effectValue;
    public float buffDuration = 10f;

    [Header("아이템 수량 정보")]
    public int maxStackSize = 16;
}