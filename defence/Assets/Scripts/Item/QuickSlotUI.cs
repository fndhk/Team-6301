// ���� �̸�: QuickSlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickSlotUI : MonoBehaviour
{
    // ����Ƽ �����Ϳ��� ������ UI ��ҵ�
    public Image itemIcon;
    public GameObject iconBackground; // �������� ���� ���� ���� ��� (���û���)

    // ������ �����͸� �޾� �������� ǥ���ϴ� �Լ�
    public void DisplayItem(ItemData item)
    {
        if (item == null)
        {
            ClearSlot();
            return;
        }

        itemIcon.sprite = item.icon;
        itemIcon.gameObject.SetActive(true);
        if (iconBackground != null) iconBackground.SetActive(true);
    }

    // ������ ���� �Լ�
    public void ClearSlot()
    {
        itemIcon.sprite = null;
        itemIcon.gameObject.SetActive(false);
        if (iconBackground != null) iconBackground.SetActive(false);
    }
}