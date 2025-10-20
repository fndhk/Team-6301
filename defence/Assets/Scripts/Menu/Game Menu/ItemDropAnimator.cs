// 파일 이름: ItemDropAnimator.cs (디버그 로그 추가 버전)
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemDropAnimator : MonoBehaviour
{
    [Header("UI 연결 (자동)")]
    [SerializeField] private Image iconImage;

    private ItemData targetItem;
    private RectTransform targetSlotTransform;
    private Vector3 startPosition;
    private Camera mainCamera;

    public void Initialize(ItemData itemData, Vector3 enemyWorldPosition)
    {
        // ▼▼▼ 디버그 로그 1: Initialize 함수가 호출되었는지 확인 ▼▼▼
        Debug.Log("--- ItemDropAnimator: Initialize 시작! ---");

        this.targetItem = itemData;
        this.mainCamera = Camera.main;

        foreach (var slot in QuickSlotManager.instance.slots)
        {
            if (slot.designatedItem == itemData)
            {
                targetSlotTransform = slot.GetComponent<RectTransform>();
                break;
            }
        }

        if (targetSlotTransform == null)
        {
            Debug.LogWarning($"<color=red>문제 발견:</color> {itemData.name}에 해당하는 퀵슬롯을 찾지 못했습니다. 퀵슬롯 설정을 확인하세요.");
            QuickSlotManager.instance.AddItem(itemData);
            Destroy(gameObject);
            return;
        }

        // ▼▼▼ 디버그 로그 2: 목표 슬롯을 제대로 찾았는지 확인 ▼▼▼
        Debug.Log($"목표 아이템: {itemData.name}, 목표 슬롯: {targetSlotTransform.name}");

        iconImage.sprite = itemData.icon;
        startPosition = mainCamera.WorldToScreenPoint(enemyWorldPosition);
        iconImage.rectTransform.position = startPosition;

        // ▼▼▼ 디버그 로그 3: 시작 좌표와 목표 좌표 출력 ▼▼▼
        Debug.Log($"<color=yellow>계산된 시작 좌표 (Screen): {startPosition}</color>");
        Debug.Log($"<color=cyan>찾아낸 목표 좌표 (Slot): {targetSlotTransform.position}</color>");

        StartCoroutine(AnimateDrop());
    }

    private IEnumerator AnimateDrop()
    {
        // ▼▼▼ 디버그 로그 4: 코루틴이 시작되었는지 확인 ▼▼▼
        Debug.Log("--- AnimateDrop 코루틴 시작! 0.5초 대기합니다. ---");
        iconImage.enabled = true; // 이미지가 보이도록 강제로 활성화

        //  수정 1: WaitForSeconds -> WaitForSecondsRealtime 
        // timeScale에 영향을 받지 않고 실제 시간으로 0.5초를 기다립니다.
        yield return new WaitForSecondsRealtime(0.5f);

        // ▼▼▼ 디버그 로그 5: 날아가기 시작함을 알림 ▼▼▼
        Debug.Log("--- 0.5초 대기 완료! 목표 지점으로 이동 시작! ---");

        float duration = 0.4f;
        float elapsed = 0f;
        Vector3 initialPosition = iconImage.rectTransform.position;

        while (elapsed < duration)
        {
            iconImage.rectTransform.position = Vector3.Lerp(
                initialPosition,
                targetSlotTransform.position,
                elapsed / duration
            );

            //  수정 2: Time.deltaTime -> Time.unscaledDeltaTime 
            // timeScale에 영향을 받지 않는 실제 프레임 시간으로 계산합니다.
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        iconImage.rectTransform.position = targetSlotTransform.position;
        QuickSlotManager.instance.AddItem(targetItem);

        // ▼▼▼ 디버그 로그 6: 애니메이션 종료 및 파괴 알림 ▼▼▼
        Debug.Log("--- 이동 완료! 아이템 추가 후 오브젝트를 파괴합니다. ---");
        Destroy(gameObject);
    }
}