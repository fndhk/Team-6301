using System.Collections.Generic;
using UnityEngine;

public class QuickSlotManager : MonoBehaviour
{
    public static QuickSlotManager instance;

    // Inspector 창에서 슬롯들을 연결
    public List<QuickSlot> slots = new List<QuickSlot>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // 아이템을 추가하는 함수 (핵심 로직 변경)
    public void AddItem(ItemData itemData)
    {
        // 1. 지정된 아이템을 담당하는 슬롯을 찾습니다.
        foreach (QuickSlot slot in slots)
        {
            // 슬롯에 지정된 아이템이 방금 들어온 아이템과 같은지 확인
            if (slot.designatedItem == itemData)
            {
                // 2. 올바른 슬롯을 찾았다면, 해당 슬롯에 아이템을 1개 추가합니다.
                slot.AddItem();
                return; // 작업 완료
            }
        }

        // 3. 룻 테이블에는 있지만 퀵슬롯에는 지정되지 않은 아이템일 경우
        Debug.LogWarning(itemData.itemName + " 아이템을 추가할 수 있는 지정된 슬롯이 없습니다.");
    }
}