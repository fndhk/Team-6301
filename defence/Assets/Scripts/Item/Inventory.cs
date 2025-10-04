// 파일 이름: Inventory.cs (단순 아이템 소모 버전)

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    // ... (InventoryItem 클래스와 변수 선언은 그대로) ...
    public static Inventory instance;
    public List<InventoryItem> items = new List<InventoryItem>();
    public InventorySlot[] slots;

    [System.Serializable]
    public class InventoryItem
    {
        public ItemData data;
        public int quantity;
        public InventoryItem(ItemData itemData) { data = itemData; quantity = 1; }
    }


    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
    }

    // ... (AddItem, UpdateUI 함수는 그대로) ...
    public bool AddItem(ItemData itemData)
    {
        InventoryItem existingItem = items.FirstOrDefault(item => item.data == itemData && item.quantity < itemData.maxStackSize);
        if (existingItem != null)
        {
            existingItem.quantity++;
        }
        else
        {
            if (items.Count >= slots.Length) { Debug.Log("인벤토리가 가득 찼습니다."); return false; }
            items.Add(new InventoryItem(itemData));
        }
        UpdateUI();
        return true;
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count) { slots[i].DisplayItem(items[i]); }
            else { slots[i].ClearSlot(); }
        }
    }


    public void UseItem(ItemData itemData)
    {
        InventoryItem itemToUse = items.FirstOrDefault(item => item.data == itemData);

        if (itemToUse != null)
        {
            // 성공/실패 확인 없이 TowerManager에게 버프 적용을 요청
            TowerManager.instance.ApplyBuff(itemToUse.data);

            // 바로 아이템을 소모
            Debug.Log(itemToUse.data.itemName + " 아이템을 사용했습니다.");
            itemToUse.quantity--;

            if (itemToUse.quantity <= 0)
            {
                items.Remove(itemToUse);
            }

            UpdateUI();
        }
    }
}