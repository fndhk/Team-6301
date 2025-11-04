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

    void Start()
    {
        foreach (QuickSlot slot in slots)
        {
            slot.ClearSlot();
        }
    }
    void Update()
    {
        // Z 키를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseSlot(0); // 첫 번째 슬롯 (인덱스 0) 사용
        }
        // X 키를 눌렀을 때
        else if (Input.GetKeyDown(KeyCode.W))
        {
            UseSlot(1); // 두 번째 슬롯 (인덱스 1) 사용
        }
        // . (마침표) 키를 눌렀을 때
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            UseSlot(2); // 세 번째 슬롯 (인덱스 2) 사용
        }
        // / (슬래시) 키를 눌렀을 때
        else if (Input.GetKeyDown(KeyCode.RightShift))
        {
            UseSlot(3); // 네 번째 슬롯 (인덱스 3) 사용
        }
    }
    public void UseSlot(int slotIndex)
    {
        // 유효한 슬롯 번호인지 확인 (슬롯이 4개 미만일 경우를 대비)
        if (slotIndex >= 0 && slotIndex < slots.Count)
        {
            // 해당 슬롯에게 아이템을 사용하라고 명령
            slots[slotIndex].UseItem();
        }
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