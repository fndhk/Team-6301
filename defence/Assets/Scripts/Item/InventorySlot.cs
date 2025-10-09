using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI quantityText;

    private ItemData currentItem; // 현재 슬롯에 있는 아이템 데이터

    public void DisplayItem(Inventory.InventoryItem item)
    {
        currentItem = item.data;
        icon.sprite = currentItem.icon;
        icon.gameObject.SetActive(true);

        if (item.quantity > 0)
        {
            quantityText.text = item.quantity.ToString();
            quantityText.gameObject.SetActive(true);
        }
        else
        {
            quantityText.gameObject.SetActive(false);
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.gameObject.SetActive(false);
        quantityText.gameObject.SetActive(false);
    }
    public void OnSlotClick()
    {
        if (currentItem != null)
        {
            // TowerManager의 범용 ApplyBuff 함수를 호출하며 아이템 데이터 전체를 넘겨줌
            TowerManager.instance.ApplyBuff(currentItem);
            Inventory.instance.UseItem(currentItem);
        }
    }
}