// 파일 이름: QuickSlotManager.cs (수정된 최종 버전)
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotManager : MonoBehaviour
{
    public static QuickSlotManager instance;

    [Header("UI 연결")]
    public QuickSlotUI[] slots; // 4개의 슬롯 UI를 연결할 배열

    [Header("아이템 데이터")]
    public List<ItemData> quickSlotItems = new List<ItemData>();
    public int maxSlots = 4;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 게임 시작 시 모든 슬롯 UI를 초기화
        UpdateAllSlotsUI();
    }

    void Update()
    {
        // 단축키 입력 감지
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0); // 1번 키는 0번 인덱스
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(1); // 2번 키는 1번 인덱스
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseItem(2); // 3번 키는 2번 인덱스
        if (Input.GetKeyDown(KeyCode.Alpha4)) UseItem(3); // 4번 키는 3번 인덱스
    }

    // 아이템을 퀵슬롯에 추가하는 함수
    public bool AddItem(ItemData item)
    {
        if (quickSlotItems.Count >= maxSlots)
        {
            Debug.Log("퀵슬롯이 가득 찼습니다!");
            return false;
        }

        quickSlotItems.Add(item);
        Debug.Log(item.itemName + " 아이템을 획득했습니다!");

        UpdateAllSlotsUI(); // UI 새로고침
        return true;
    }

    // 지정된 슬롯의 아이템을 사용하는 함수
    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= quickSlotItems.Count)
        {
            // Debug.Log((slotIndex + 1) + "번 슬롯은 비어있습니다."); // 조용한게 좋다면 주석처리
            return;
        }

        ItemData itemToUse = quickSlotItems[slotIndex];

        // --- 여기가 핵심 변경점 ---
        // 아이템에게 연결된 '효과'가 있는지 확인하고, 있다면 실행!
        if (itemToUse.itemEffect != null)
        {
            Debug.Log(itemToUse.itemName + " 아이템 효과 발동!");
            itemToUse.itemEffect.ExecuteEffect();
        }
        else
        {
            Debug.LogWarning(itemToUse.itemName + "에 연결된 효과가 없습니다.");
        }

        quickSlotItems.RemoveAt(slotIndex);
        UpdateAllSlotsUI();
    }

    // 모든 슬롯 UI를 현재 아이템 리스트에 맞게 새로고침하는 함수
    private void UpdateAllSlotsUI()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            // i번째 슬롯에 아이템이 있다면
            if (i < quickSlotItems.Count)
            {
                slots[i].DisplayItem(quickSlotItems[i]);
            }
            // 아이템이 없다면
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}