using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    [System.Serializable]
    public class InventoryItem
    {
        public ItemData data;
        public int quantity;

        public InventoryItem(ItemData itemData)
        {
            data = itemData;
            quantity = 1;
        }
    }

    public List<InventoryItem> items = new List<InventoryItem>();
    public InventorySlot[] slots;

    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
    }

    // 아이템 추가 로직 (완전히 변경)
    public bool AddItem(ItemData itemData)
    {
        // 1. 인벤토리 내에 동일하고, 꽉 차지 않은 아이템 묶음(스택)이 있는지 검색
        InventoryItem existingItem = items.FirstOrDefault(item => item.data == itemData && item.quantity < itemData.maxStackSize);

        if (existingItem != null)
        {
            // 2. 찾았다면, 해당 묶음의 수량을 1 증가
            existingItem.quantity++;
        }
        else
        {
            // 3. 못 찾았다면, 인벤토리에 빈 슬롯이 있는지 확인
            if (items.Count >= slots.Length)
            {
                Debug.Log("인벤토리가 가득 찼습니다.");
                return false;
            }
            // 4. 빈 슬롯이 있다면, 수량 1짜리 새 묶음을 추가
            items.Add(new InventoryItem(itemData));
        }

        UpdateUI();
        return true;
    }

    public void UseItem(ItemData itemData)
    {
        InventoryItem itemToUse = items.FirstOrDefault(item => item.data == itemData);

        if (itemToUse != null)
        {
            // ★★★ 이 부분만 수정하면 됩니다 ★★★
            // TowerManager의 범용 ApplyBuff 함수를 호출하며 아이템 데이터 전체를 넘겨줌
            TowerManager.instance.ApplyBuff(itemToUse.data);

            // 아이템 수량 감소
            itemToUse.quantity--;

            // 만약 수량이 0개가 되면 인벤토리에서 완전히 제거
            if (itemToUse.quantity <= 0)
            {
                items.Remove(itemToUse);
            }

            UpdateUI();
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                slots[i].DisplayItem(items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}