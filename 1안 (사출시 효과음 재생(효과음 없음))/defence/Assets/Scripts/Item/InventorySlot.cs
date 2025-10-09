// 파일 이름: InventorySlot.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 드래그 이벤트를 사용하기 위해 추가
using TMPro;

// IBeginDragHandler 등을 사용하려면 인터페이스를 상속받아야 함
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

    // --- 드래그 기능 구현 ---

    // 드래그를 시작했을 때 딱 한 번 호출
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            DragDropManager.instance.StartDrag(currentItem.data.icon);
        }
    }

    // 드래그하는 동안 계속 호출
    public void OnDrag(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            DragDropManager.instance.OnDrag();
        }
    }

    // 드래그를 끝냈을 때(마우스를 놓았을 때) 딱 한 번 호출
    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

        DragDropManager.instance.EndDrag();

        // 1. 마우스 위치에 Ray를 쏴서 어떤 오브젝트가 있는지 확인
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // 2. Ray에 맞은 오브젝트가 있고, 그 오브젝트에 TowerSlot 스크립트가 있다면
        if (hit.collider != null)
        {
            TowerSlot targetSlot = hit.collider.GetComponent<TowerSlot>();
            if (targetSlot != null)
            {
                // 3. 타워 슬롯에 아이템 정보를 전달
                targetSlot.OnItemDropped(currentItem.data);

                // 4. 아이템 사용 처리
                Inventory.instance.UseItem(currentItem.data);
            }
        }
    }

    // (참고) 기존의 클릭 방식은 이제 사용하지 않으므로 OnSlotClick 함수는 삭제하거나 주석 처리합니다.
    /*
    public void OnSlotClick()
    {
        // ...
    }
    */
}