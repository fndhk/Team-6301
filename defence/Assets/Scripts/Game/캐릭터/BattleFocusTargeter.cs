// 파일 이름: BattleFocusTargeter.cs (수정된 최종 버전)
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// ▼▼▼ 여기에 SkillPositionInfo 클래스 정의를 추가합니다 ▼▼▼
// 이제 이 클래스는 공개적으로 접근 가능하므로 다른 스크립트에서도 사용할 수 있습니다.
[System.Serializable]
public class SkillPositionInfo
{
    [Tooltip("이 위치를 선택할 때 누를 키")]
    public KeyCode activationKey;
    [Tooltip("스킬이 생성될 위치 (씬에 있는 빈 오브젝트)")]
    public GameObject positionObject;
}


public class BattleFocusTargeter : MonoBehaviour
{
    public static BattleFocusTargeter instance;

    private bool isTargeting = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void BeginTargeting(GameObject panelPrefab, List<SkillPositionInfo> positions, Action<GameObject> onTargetSelected)
    {
        if (isTargeting) return;
        StartCoroutine(TargetingCoroutine(panelPrefab, positions, onTargetSelected));
    }

    private IEnumerator TargetingCoroutine(GameObject panelPrefab, List<SkillPositionInfo> positions, Action<GameObject> onTargetSelected)
    {
        isTargeting = true;

        Time.timeScale = 0f;
        AudioManager.instance.PauseAllMusic();
        GameObject panelInstance = null;
        if (panelPrefab != null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                panelInstance = Instantiate(panelPrefab, canvas.transform);
            }
        }

        GameObject targetPositionObject = null;
        while (targetPositionObject == null)
        {
            foreach (var posInfo in positions)
            {
                if (Input.GetKeyDown(posInfo.activationKey))
                {
                    targetPositionObject = posInfo.positionObject;
                    break;
                }
            }
            yield return null;
        }

        if (panelInstance != null) Destroy(panelInstance);
        Time.timeScale = 1f;
        AudioManager.instance.UnpauseAllMusic();

        onTargetSelected?.Invoke(targetPositionObject);

        isTargeting = false;
    }
}