using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // �̱���(Singleton) ����: ��𼭵� Inventory ��ũ��Ʈ�� ���� ������ �� �ֵ��� ��
    public static Inventory instance;

    public List<Sprite> items = new List<Sprite>(); // ȹ���� �������� '�̹���'�� �����ϴ� ����Ʈ
    public InventorySlot[] slots; // �κ��丮�� ��� ���Ե�

    void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // �������� �߰��ϴ� �Լ�
    public bool AddItem(Sprite itemIcon)
    {
        // ����ִ� ������ ã�´�
        for (int i = 0; i < slots.Length; i++)
        {
            // items ����Ʈ�� ũ�Ⱑ ���� ���� �ε������� �۴ٸ�, �� ������ ����ִٴ� �ǹ�
            if (i >= items.Count)
            {
                items.Add(itemIcon); // ������ ����Ʈ�� �߰�
                slots[i].DisplayItem(itemIcon); // �ش� ���Կ� ������ ǥ��
                return true; // �߰� ����
            }
        }

        Debug.Log("�κ��丮�� ���� á���ϴ�.");
        return false; // �κ��丮�� ���� ���� �߰� ����
    }
}