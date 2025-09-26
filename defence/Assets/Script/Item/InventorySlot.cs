using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon; // �������� �̹����� ǥ���� UI Image ������Ʈ

    // ���Կ� ������ �̹����� ǥ���ϴ� �Լ�
    public void DisplayItem(Sprite itemIcon)
    {
        icon.sprite = itemIcon;
        icon.gameObject.SetActive(true); // ������ �̹����� Ȱ��ȭ�Ͽ� ������
    }

    // ������ ���� �Լ�
    public void ClearSlot()
    {
        icon.sprite = null;
        icon.gameObject.SetActive(false); // ������ �̹����� ��Ȱ��ȭ�Ͽ� ����
    }
}