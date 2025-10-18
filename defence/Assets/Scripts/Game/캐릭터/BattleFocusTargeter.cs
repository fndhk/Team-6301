// ���� �̸�: BattleFocusTargeter.cs (������ ���� ����)
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// ���� ���⿡ SkillPositionInfo Ŭ���� ���Ǹ� �߰��մϴ� ����
// ���� �� Ŭ������ ���������� ���� �����ϹǷ� �ٸ� ��ũ��Ʈ������ ����� �� �ֽ��ϴ�.
[System.Serializable]
public class SkillPositionInfo
{
    [Tooltip("�� ��ġ�� ������ �� ���� Ű")]
    public KeyCode activationKey;
    [Tooltip("��ų�� ������ ��ġ (���� �ִ� �� ������Ʈ)")]
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