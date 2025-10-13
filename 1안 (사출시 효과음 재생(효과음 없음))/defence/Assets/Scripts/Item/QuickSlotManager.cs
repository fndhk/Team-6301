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