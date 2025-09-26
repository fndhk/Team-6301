using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon; // 아이템의 이미지를 표시할 UI Image 컴포넌트

    // 슬롯에 아이템 이미지를 표시하는 함수
    public void DisplayItem(Sprite itemIcon)
    {
        icon.sprite = itemIcon;
        icon.gameObject.SetActive(true); // 아이템 이미지를 활성화하여 보여줌
    }

    // 슬롯을 비우는 함수
    public void ClearSlot()
    {
        icon.sprite = null;
        icon.gameObject.SetActive(false); // 아이템 이미지를 비활성화하여 숨김
    }
}