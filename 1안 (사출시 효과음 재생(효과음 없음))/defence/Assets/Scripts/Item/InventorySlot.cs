// ���� �̸�: InventorySlot.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // �巡�� �̺�Ʈ�� ����ϱ� ���� �߰�
using TMPro;

// IBeginDragHandler ���� ����Ϸ��� �������̽��� ��ӹ޾ƾ� ��
public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image icon;
    public TextMeshProUGUI quantityText;

    private Inventory.InventoryItem currentItem;

    public void DisplayItem(Inventory.InventoryItem item)
    {
        currentItem = item;
        icon.sprite = item.data.icon;
        icon.gameObject.SetActive(true);
        quantityText.text = item.quantity > 1 ? item.quantity.ToString() : "";
        quantityText.gameObject.SetActive(item.quantity > 1);
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.gameObject.SetActive(false);
        quantityText.gameObject.SetActive(false);
    }

    // --- �巡�� ��� ���� ---

    // �巡�׸� �������� �� �� �� �� ȣ��
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            DragDropManager.instance.StartDrag(currentItem.data.icon);
        }
    }

    // �巡���ϴ� ���� ��� ȣ��
    public void OnDrag(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            DragDropManager.instance.OnDrag();
        }
    }

    // �巡�׸� ������ ��(���콺�� ������ ��) �� �� �� ȣ��
    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

        DragDropManager.instance.EndDrag();

        // 1. ���콺 ��ġ�� Ray�� ���� � ������Ʈ�� �ִ��� Ȯ��
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // 2. Ray�� ���� ������Ʈ�� �ְ�, �� ������Ʈ�� TowerSlot ��ũ��Ʈ�� �ִٸ�
        if (hit.collider != null)
        {
            TowerSlot targetSlot = hit.collider.GetComponent<TowerSlot>();
            if (targetSlot != null)
            {
                // 3. Ÿ�� ���Կ� ������ ������ ����
                targetSlot.OnItemDropped(currentItem.data);

                // 4. ������ ��� ó��
                Inventory.instance.UseItem(currentItem.data);
            }
        }
    }

    // (����) ������ Ŭ�� ����� ���� ������� �����Ƿ� OnSlotClick �Լ��� �����ϰų� �ּ� ó���մϴ�.
    /*
    public void OnSlotClick()
    {
        // ...
    }
    */
}