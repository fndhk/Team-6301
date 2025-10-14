// 파일 이름: ItemData.cs (최종 확인 버전)
using UnityEngine;

// ▼▼▼ EffectType과 ItemType enum이 있던 부분은 완전히 삭제되어야 합니다. ▼▼▼

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("아이템 정보")]
    public string itemName = "새 아이템";
    public Sprite icon = null;

    [Header("아이템 효과")]
    // 이 아이템이 사용할 '효과'를 ScriptableObject 형태로 연결합니다.
    public ItemEffect itemEffect;

    [Header("아이템 수량 정보")]
    public int maxStackSize = 16;
}