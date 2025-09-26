using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // 싱글톤(Singleton) 패턴: 어디서든 Inventory 스크립트에 쉽게 접근할 수 있도록 함
    public static Inventory instance;

    public List<Sprite> items = new List<Sprite>(); // 획득한 아이템의 '이미지'를 저장하는 리스트
    public InventorySlot[] slots; // 인벤토리의 모든 슬롯들

    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // 아이템을 추가하는 함수
    public bool AddItem(Sprite itemIcon)
    {
        // 비어있는 슬롯을 찾는다
        for (int i = 0; i < slots.Length; i++)
        {
            // items 리스트의 크기가 현재 슬롯 인덱스보다 작다면, 그 슬롯은 비어있다는 의미
            if (i >= items.Count)
            {
                items.Add(itemIcon); // 아이템 리스트에 추가
                slots[i].DisplayItem(itemIcon); // 해당 슬롯에 아이템 표시
                return true; // 추가 성공
            }
        }

        Debug.Log("인벤토리가 가득 찼습니다.");
        return false; // 인벤토리가 가득 차서 추가 실패
    }
}