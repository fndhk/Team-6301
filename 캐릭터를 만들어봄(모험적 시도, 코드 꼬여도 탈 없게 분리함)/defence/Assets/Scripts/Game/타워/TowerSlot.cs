// 파일 이름: TowerSlot.cs (최종 수정)
using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    [Header("관리할 타워")]
    // 이 슬롯이 관리하는 타워 오브젝트 (Inspector에서 연결)
    public BaseTower towerInSlot;

    // 타워의 Sprite Renderer를 미리 찾아둠 (색상 변경용)
    private SpriteRenderer towerSpriteRenderer;

    void Start()
    {
        // 타워의 자식 오브젝트에서 Sprite Renderer를 찾아옴
        if (towerInSlot != null)
        {
            towerSpriteRenderer = towerInSlot.GetComponentInChildren<SpriteRenderer>();
        }

        // 게임 시작 시 비활성화 상태라면 색상을 회색으로 변경
        if (towerInSlot != null && !towerInSlot.gameObject.activeSelf && towerSpriteRenderer != null)
        {
            towerSpriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 0.8f); // 어두운 회색
        }
    }

    // 외부(아이템 시스템)에서 호출할 활성화 함수
    public void ActivateTower()
    {
        if (towerInSlot != null && !towerInSlot.gameObject.activeSelf)
        {
            towerInSlot.gameObject.SetActive(true);
            if (towerSpriteRenderer != null)
            {
                towerSpriteRenderer.color = Color.white; // 원래 색상으로 복원
            }
            Debug.Log(towerInSlot.name + " 활성화!");
        }
    }

    // InventorySlot에서 아이템을 성공적으로 드롭했을 때 호출될 함수
    public void OnItemDropped(ItemData droppedItem)
    {
        // 1. 이 슬롯의 타워가 비활성화 상태라면 (가장 먼저 체크)
        if (!towerInSlot.gameObject.activeSelf)
        {
            // 아이템 종류와 상관없이 무조건 활성화
            ActivateTower();
        }
        // 2. 타워가 이미 활성화 상태이고, 드롭된 아이템에 '효과'가 연결되어 있다면
        else if (droppedItem.itemEffect != null)
        {
            Debug.Log($"{towerInSlot.name}에 {droppedItem.itemName} 버프 적용!");

            // 아이템에게 직접 효과 실행을 요청
            droppedItem.itemEffect.ExecuteEffect();
        }
    }
}