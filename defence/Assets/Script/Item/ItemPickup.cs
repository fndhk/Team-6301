using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemPickup : MonoBehaviour
{
    public ItemData itemData; // 이 아이템의 '설계도'
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        // 자기 자신의 SpriteRenderer 컴포넌트를 미리 찾아놓음
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 아이템이 생성될 때 호출되어 어떤 아이템인지 설정하는 함수
    public void Initialize(ItemData data)
    {
        itemData = data;
        // SpriteRenderer에 ItemData에 있는 아이콘을 표시
        spriteRenderer.sprite = itemData.icon;
    }

    private void OnMouseDown()
    {
        if (itemData == null) return;

        bool wasAdded = Inventory.instance.AddItem(itemData);
        if (wasAdded)
        {
            Destroy(gameObject);
        }
    }
}