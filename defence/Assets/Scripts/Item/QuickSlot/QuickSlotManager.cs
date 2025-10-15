using System.Collections.Generic;
using UnityEngine;

public class QuickSlotManager : MonoBehaviour
{
    public static QuickSlotManager instance;

    // Inspector â���� ���Ե��� ����
    public List<QuickSlot> slots = new List<QuickSlot>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        foreach (QuickSlot slot in slots)
        {
            slot.ClearSlot();
        }
    }
    void Update()
    {
        // ���� Ű 1���� ������ ��
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseSlot(0); // ù ��° ���� (�ε��� 0) ���
        }
        // ���� Ű 2���� ������ ��
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseSlot(1); // �� ��° ���� (�ε��� 1) ���
        }
        // ���� Ű 3���� ������ ��
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseSlot(2); // �� ��° ���� (�ε��� 2) ���
        }
        // ���� Ű 4���� ������ ��
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UseSlot(3); // �� ��° ���� (�ε��� 3) ���
        }
    }
    public void UseSlot(int slotIndex)
    {
        // ��ȿ�� ���� ��ȣ���� Ȯ�� (������ 4�� �̸��� ��츦 ���)
        if (slotIndex >= 0 && slotIndex < slots.Count)
        {
            // �ش� ���Կ��� �������� ����϶�� ���
            slots[slotIndex].UseItem();
        }
    }
    // �������� �߰��ϴ� �Լ� (�ٽ� ���� ����)
    public void AddItem(ItemData itemData)
    {
        // 1. ������ �������� ����ϴ� ������ ã���ϴ�.
        foreach (QuickSlot slot in slots)
        {
            // ���Կ� ������ �������� ��� ���� �����۰� ������ Ȯ��
            if (slot.designatedItem == itemData)
            {
                // 2. �ùٸ� ������ ã�Ҵٸ�, �ش� ���Կ� �������� 1�� �߰��մϴ�.
                slot.AddItem();
                return; // �۾� �Ϸ�
            }
        }

        // 3. �� ���̺��� ������ �����Կ��� �������� ���� �������� ���
        Debug.LogWarning(itemData.itemName + " �������� �߰��� �� �ִ� ������ ������ �����ϴ�.");
    }
}