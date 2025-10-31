// 파일 이름: MaterialsUI.cs (실시간 점수 표시용으로 수정)
using UnityEngine;
using TMPro;
using System.Collections;

public class MaterialsUI : MonoBehaviour
{
    public static MaterialsUI instance;

    [Header("UI 연결")]
    [Tooltip("점수 숫자를 표시할 텍스트")]
    public TextMeshProUGUI materialsText; //  변수명 유지

    [Header("애니메이션 설정")]
    public bool useScaleAnimation = true;
    public bool useColorAnimation = true;
    public Color increaseColor = Color.yellow;

    //  변수명 유지 (이제 "현재 표시된 점수"를 의미)
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
        UpdateText(); // 시작 시 0으로 초기화
    }

    // ---  ScoreManager가 호출할 새 함수 ---
    public void OnScoreChanged(long newScoreLong)
    {
        int actualScore = (int)newScoreLong;

        // 점수가 증가했을 때만 애니메이션 재생
        if (actualScore > currentDisplayedMaterials)
        {
            StartCoroutine(AnimateIncrease(actualScore));
        }
        // 점수가 감소했거나 (체력 감소 등) 같을 때는 애니메이션 없이 즉시 반영
        else if (actualScore < currentDisplayedMaterials)
        {
            currentDisplayedMaterials = actualScore;
            UpdateText();
        }
        // (점수가 같으면 아무것도 안 함)
    }

    // ------ 점수 증가 애니메이션 ------
    //  (이하 모든 애니메이션 코드는 원본과 동일하게 유지)
    private IEnumerator AnimateIncrease(int targetAmount)
    {
        int startAmount = currentDisplayedMaterials;
        float duration = 0.2f; // 카운트업 속도를 0.2초로 고정 (더 부드러움)
        float elapsed = 0f;

        if (useScaleAnimation)
        {
            StartCoroutine(ScaleAnimation(1.2f, 0.2f));
        }
        if (useColorAnimation)
        {
            materialsText.color = increaseColor;
        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            currentDisplayedMaterials = Mathf.RoundToInt(Mathf.Lerp(startAmount, targetAmount, t));
            UpdateText();
            yield return null;
        }

        currentDisplayedMaterials = targetAmount;
        UpdateText();

        yield return new WaitForSecondsRealtime(0.1f);

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
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            materialsText.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        materialsText.transform.localScale = endScale;
        yield return new WaitForSecondsRealtime(0.1f);
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
            materialsText.text = currentDisplayedMaterials.ToString("N0");
        }
    }
}