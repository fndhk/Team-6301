// 파일 이름: MaterialsUI.cs
using UnityEngine;
using TMPro;
using System.Collections;

public class MaterialsUI : MonoBehaviour
{
    public static MaterialsUI instance;

    [Header("UI 연결")]
    [Tooltip("재화 숫자를 표시할 텍스트")]
    public TextMeshProUGUI materialsText;

    [Header("애니메이션 설정")]
    [Tooltip("재화가 증가할 때 텍스트가 커지는 효과")]
    public bool useScaleAnimation = true;
    [Tooltip("재화가 증가할 때 색상 변경 효과")]
    public bool useColorAnimation = true;
    [Tooltip("증가 시 표시할 색상")]
    public Color increaseColor = Color.yellow;

    private int currentDisplayedMaterials = 0;
    private Color originalColor;
    private Vector3 originalScale;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (materialsText != null)
        {
            originalColor = materialsText.color;
            originalScale = materialsText.transform.localScale;
        }

        // 게임 시작 시 현재 재화량 표시
        UpdateMaterialsDisplay();
    }

    // ------ 재화 표시 업데이트 (실시간) ------
    public void UpdateMaterialsDisplay()
    {
        if (SaveLoadManager.instance == null || SaveLoadManager.instance.gameData == null)
        {
            Debug.LogWarning("MaterialsUI: SaveLoadManager 또는 gameData가 없습니다!");
            return;
        }

        int actualMaterials = SaveLoadManager.instance.gameData.enhancementMaterials;

        // 현재 표시된 숫자와 실제 재화량이 다르면 애니메이션
        if (actualMaterials != currentDisplayedMaterials)
        {
            int difference = actualMaterials - currentDisplayedMaterials;

            if (difference > 0)
            {
                // 재화 증가
                StartCoroutine(AnimateIncrease(actualMaterials));
            }
            else
            {
                // 재화 감소 (즉시 반영)
                currentDisplayedMaterials = actualMaterials;
                UpdateText();
            }
        }
        else
        {
            // 변화 없으면 그냥 업데이트
            UpdateText();
        }
    }

    // ------ 재화 증가 애니메이션 ------
    private IEnumerator AnimateIncrease(int targetAmount)
    {
        int startAmount = currentDisplayedMaterials;
        int difference = targetAmount - startAmount;
        float duration = Mathf.Min(0.5f, difference * 0.05f); // 최대 0.5초
        float elapsed = 0f;

        // 스케일 애니메이션 시작
        if (useScaleAnimation)
        {
            StartCoroutine(ScaleAnimation(1.2f, 0.2f));
        }

        // 색상 변경
        if (useColorAnimation)
        {
            materialsText.color = increaseColor;
        }

        // 숫자 카운트업
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Time.timeScale 영향 안 받음
            float t = elapsed / duration;

            currentDisplayedMaterials = Mathf.RoundToInt(Mathf.Lerp(startAmount, targetAmount, t));
            UpdateText();

            yield return null;
        }

        currentDisplayedMaterials = targetAmount;
        UpdateText();

        // 약간의 대기
        yield return new WaitForSecondsRealtime(0.1f);

        // 색상 복원
        if (useColorAnimation)
        {
            StartCoroutine(ColorAnimation(originalColor, 0.3f));
        }
    }

    // ------ 스케일 애니메이션 코루틴 ------
    private IEnumerator ScaleAnimation(float targetScale, float duration)
    {
        Vector3 startScale = materialsText.transform.localScale;
        Vector3 endScale = originalScale * targetScale;
        float elapsed = 0f;

        // 커지기
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            materialsText.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        materialsText.transform.localScale = endScale;

        // 잠깐 대기
        yield return new WaitForSecondsRealtime(0.1f);

        // 원래대로
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            materialsText.transform.localScale = Vector3.Lerp(endScale, originalScale, t);
            yield return null;
        }

        materialsText.transform.localScale = originalScale;
    }

    // ------ 색상 애니메이션 코루틴 ------
    private IEnumerator ColorAnimation(Color targetColor, float duration)
    {
        Color startColor = materialsText.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            materialsText.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        materialsText.color = targetColor;
    }

    // ------ 텍스트 업데이트 (천단위 콤마) ------
    private void UpdateText()
    {
        if (materialsText != null)
        {
            materialsText.text = currentDisplayedMaterials.ToString("N0"); // 1,234 형식
        }
    }

    // ------ 외부에서 재화 변경 시 호출할 함수 ------
    public void OnMaterialsChanged()
    {
        UpdateMaterialsDisplay();
    }
}