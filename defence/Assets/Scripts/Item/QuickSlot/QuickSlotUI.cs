// 파일 이름: QuickSlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickSlotUI : MonoBehaviour
{
    // 유니티 에디터에서 연결할 UI 요소들
    public Image itemIcon;
    public GameObject iconBackground; // 아이콘이 있을 때만 켜줄 배경 (선택사항)

    // 아이템 데이터를 받아 아이콘을 표시하는 함수
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

    // 슬롯을 비우는 함수
    public void ClearSlot()
    {
        itemIcon.sprite = null;
        itemIcon.gameObject.SetActive(false);
        if (iconBackground != null) iconBackground.SetActive(false);
    }
}