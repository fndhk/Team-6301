// ���� �̸�: Inventory.cs (�ܼ� ������ �Ҹ� ����)

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    // ... (InventoryItem Ŭ������ ���� ������ �״��) ...
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

    // ... (AddItem, UpdateUI �Լ��� �״��) ...
    public bool AddItem(ItemData itemData)
    {
        InventoryItem existingItem = items.FirstOrDefault(item => item.data == itemData && item.quantity < itemData.maxStackSize);
        if (existingItem != null)
        {
            existingItem.quantity++;
        }
        else
        {
            if (items.Count >= slots.Length) { Debug.Log("�κ��丮�� ���� á���ϴ�."); return false; }
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
            // ����/���� Ȯ�� ���� TowerManager���� ���� ������ ��û
            TowerManager.instance.ApplyBuff(itemToUse.data);

            // �ٷ� �������� �Ҹ�
            Debug.Log(itemToUse.data.itemName + " �������� ����߽��ϴ�.");
            itemToUse.quantity--;

            if (itemToUse.quantity <= 0)
            {
                items.Remove(itemToUse);
            }

            UpdateUI();
        }
    }
}