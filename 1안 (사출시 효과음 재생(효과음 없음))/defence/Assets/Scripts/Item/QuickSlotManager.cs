// ���� �̸�: QuickSlotManager.cs (������ ���� ����)
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotManager : MonoBehaviour
{
    public static QuickSlotManager instance;

    [Header("UI ����")]
    public QuickSlotUI[] slots; // 4���� ���� UI�� ������ �迭

    [Header("������ ������")]
    public List<ItemData> quickSlotItems = new List<ItemData>();
    public int maxSlots = 4;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // ���� ���� �� ��� ���� UI�� �ʱ�ȭ
        UpdateAllSlotsUI();
    }

    void Update()
    {
        // ����Ű �Է� ����
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0); // 1�� Ű�� 0�� �ε���
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(1); // 2�� Ű�� 1�� �ε���
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseItem(2); // 3�� Ű�� 2�� �ε���
        if (Input.GetKeyDown(KeyCode.Alpha4)) UseItem(3); // 4�� Ű�� 3�� �ε���
    }

    // �������� �����Կ� �߰��ϴ� �Լ�
    public bool AddItem(ItemData item)
    {
        if (quickSlotItems.Count >= maxSlots)
        {
            Debug.Log("�������� ���� á���ϴ�!");
            return false;
        }

        quickSlotItems.Add(item);
        Debug.Log(item.itemName + " �������� ȹ���߽��ϴ�!");

        UpdateAllSlotsUI(); // UI ���ΰ�ħ
        return true;
    }

    // ������ ������ �������� ����ϴ� �Լ�
    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= quickSlotItems.Count)
        {
            // Debug.Log((slotIndex + 1) + "�� ������ ����ֽ��ϴ�."); // �����Ѱ� ���ٸ� �ּ�ó��
            return;
        }

        ItemData itemToUse = quickSlotItems[slotIndex];

        // --- ���Ⱑ �ٽ� ������ ---
        // �����ۿ��� ����� 'ȿ��'�� �ִ��� Ȯ���ϰ�, �ִٸ� ����!
        if (itemToUse.itemEffect != null)
        {
            Debug.Log(itemToUse.itemName + " ������ ȿ�� �ߵ�!");
            itemToUse.itemEffect.ExecuteEffect();
        }
        else
        {
            Debug.LogWarning(itemToUse.itemName + "�� ����� ȿ���� �����ϴ�.");
        }

        quickSlotItems.RemoveAt(slotIndex);
        UpdateAllSlotsUI();
    }

    // ��� ���� UI�� ���� ������ ����Ʈ�� �°� ���ΰ�ħ�ϴ� �Լ�
    private void UpdateAllSlotsUI()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            // i��° ���Կ� �������� �ִٸ�
            if (i < quickSlotItems.Count)
            {
                slots[i].DisplayItem(quickSlotItems[i]);
            }
            // �������� ���ٸ�
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}