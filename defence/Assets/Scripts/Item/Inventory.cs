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

    // ������ �߰� ���� (������ ����)
    public bool AddItem(ItemData itemData)
    {
        // 1. �κ��丮 ���� �����ϰ�, �� ���� ���� ������ ����(����)�� �ִ��� �˻�
        InventoryItem existingItem = items.FirstOrDefault(item => item.data == itemData && item.quantity < itemData.maxStackSize);

        if (existingItem != null)
        {
            // 2. ã�Ҵٸ�, �ش� ������ ������ 1 ����
            existingItem.quantity++;
        }
        else
        {
            // 3. �� ã�Ҵٸ�, �κ��丮�� �� ������ �ִ��� Ȯ��
            if (items.Count >= slots.Length)
            {
                Debug.Log("�κ��丮�� ���� á���ϴ�.");
                return false;
            }
            // 4. �� ������ �ִٸ�, ���� 1¥�� �� ������ �߰�
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
            // �ڡڡ� �� �κи� �����ϸ� �˴ϴ� �ڡڡ�
            // TowerManager�� ���� ApplyBuff �Լ��� ȣ���ϸ� ������ ������ ��ü�� �Ѱ���
            TowerManager.instance.ApplyBuff(itemToUse.data);

            // ������ ���� ����
            itemToUse.quantity--;

            // ���� ������ 0���� �Ǹ� �κ��丮���� ������ ����
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