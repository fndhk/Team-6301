// ���� �̸�: MaterialsUI.cs (�ǽð� ���� ǥ�ÿ����� ����)
using UnityEngine;
using TMPro;
using System.Collections;

public class MaterialsUI : MonoBehaviour
{
    public static MaterialsUI instance;

    [Header("UI ����")]
    [Tooltip("���� ���ڸ� ǥ���� �ؽ�Ʈ")]
    public TextMeshProUGUI materialsText; //  ������ ����
    [Tooltip("정확도를 표시할 텍스트 (비워두면 정확도 표시 안 함)")]
    public TextMeshProUGUI accuracyText; // 정확도 표시

    [Header("�ִϸ��̼� ����")]
    public bool useScaleAnimation = true;
    public bool useColorAnimation = true;
    public Color increaseColor = Color.yellow;
    public Color decreaseColor = Color.red; // 정확도가 떨어질 때 색상

    //  ������ ���� (���� "���� ǥ�õ� ����"�� �ǹ�)
    private int currentDisplayedMaterials = 0;
    private float currentDisplayedAccuracy = 100f; // 현재 표시 중인 정확도
    private Color originalColor;
    private Color accuracyOriginalColor; // 정확도 텍스트 원본 색상
    private Vector3 originalScale;
    private Vector3 accuracyOriginalScale; // 정확도 텍스트 원본 스케일

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // 중복된 MaterialsUI 스크립트만 제거 (GameObject는 유지)
            Destroy(this);
        }
    }

    void Start()
    {
        // 이 인스턴스가 활성 인스턴스가 아니면 초기화 건너뛰기
        if (instance != this) return;

        if (materialsText != null)
        {
            originalColor = materialsText.color;
            originalScale = materialsText.transform.localScale;
        }
        if (accuracyText != null)
        {
            accuracyOriginalColor = accuracyText.color;
            accuracyOriginalScale = accuracyText.transform.localScale;
        }
        UpdateText(); // ���� �� 0���� �ʱ�ȭ
        UpdateAccuracyText(); // 정확도 100%로 초기화
    }

    void Update()
    {
        // 이 인스턴스가 활성 인스턴스가 아니면 업데이트 건너뛰기
        if (instance != this) return;

        // ScoreManager에서 실시간 정확도 가져와서 표시
        if (ScoreManager.instance != null && accuracyText != null)
        {
            float newAccuracy = ScoreManager.instance.GetAverageRhythmAccuracy();
            if (Mathf.Abs(newAccuracy - currentDisplayedAccuracy) > 0.1f) // 0.1% 이상 차이나면 업데이트
            {
                OnAccuracyChanged(newAccuracy);
            }
        }
    }

    // ---  ScoreManager�� ȣ���� �� �Լ� ---
    public void OnScoreChanged(long newScoreLong)
    {
        int actualScore = (int)newScoreLong;

        // ������ �������� ���� �ִϸ��̼� ���
        if (actualScore > currentDisplayedMaterials)
        {
            StartCoroutine(AnimateIncrease(actualScore));
        }
        // ������ �����߰ų� (ü�� ���� ��) ���� ���� �ִϸ��̼� ���� ��� �ݿ�
        else if (actualScore < currentDisplayedMaterials)
        {
            currentDisplayedMaterials = actualScore;
            UpdateText();
        }
        // (������ ������ �ƹ��͵� �� ��)
    }

    // ------ ���� ���� �ִϸ��̼� ------
    //  (���� ��� �ִϸ��̼� �ڵ�� ������ �����ϰ� ����)
    private IEnumerator AnimateIncrease(int targetAmount)
    {
        int startAmount = currentDisplayedMaterials;
        float duration = 0.2f; // ī��Ʈ�� �ӵ��� 0.2�ʷ� ���� (�� �ε巯��)
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

    // ------ ������ �ִϸ��̼� �ڷ�ƾ ------
    private IEnumerator ScaleAnimation(float targetScale, float duration)
    {
        if (materialsText == null) yield break; // null 체크

        Vector3 startScale = materialsText.transform.localScale;
        Vector3 endScale = originalScale * targetScale;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            if (materialsText != null) // 루프 중 null 체크
            {
                materialsText.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            }
            yield return null;
        }
        if (materialsText != null) materialsText.transform.localScale = endScale;
        yield return new WaitForSecondsRealtime(0.1f);
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            if (materialsText != null) // 루프 중 null 체크
            {
                materialsText.transform.localScale = Vector3.Lerp(endScale, originalScale, t);
            }
            yield return null;
        }
        if (materialsText != null) materialsText.transform.localScale = originalScale;
    }

    // ------ ���� �ִϸ��̼� �ڷ�ƾ ------
    private IEnumerator ColorAnimation(Color targetColor, float duration)
    {
        if (materialsText == null) yield break; // null 체크

        Color startColor = materialsText.color;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            if (materialsText != null) // 루프 중 null 체크
            {
                materialsText.color = Color.Lerp(startColor, targetColor, t);
            }
            yield return null;
        }
        if (materialsText != null) materialsText.color = targetColor;
    }

    // ------ �ؽ�Ʈ ������Ʈ (õ���� �޸�) ------
    private void UpdateText()
    {
        if (materialsText != null)
        {
            materialsText.text = currentDisplayedMaterials.ToString("N0");
        }
    }

    // ------ 정확도 변경 처리 ------
    public void OnAccuracyChanged(float newAccuracy)
    {
        if (accuracyText == null) return;

        float oldAccuracy = currentDisplayedAccuracy;

        // 정확도가 증가했는지 감소했는지 확인
        bool isIncrease = newAccuracy > oldAccuracy;

        // 애니메이션 시작
        StartCoroutine(AnimateAccuracyChange(newAccuracy, isIncrease));
    }

    // ------ 정확도 애니메이션 ------
    private IEnumerator AnimateAccuracyChange(float targetAccuracy, bool isIncrease)
    {
        float startAccuracy = currentDisplayedAccuracy;
        float duration = 0.2f;
        float elapsed = 0f;

        if (useScaleAnimation)
        {
            StartCoroutine(AccuracyScaleAnimation(1.2f, 0.2f));
        }
        if (useColorAnimation)
        {
            accuracyText.color = isIncrease ? increaseColor : decreaseColor;
        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            currentDisplayedAccuracy = Mathf.Lerp(startAccuracy, targetAccuracy, t);
            UpdateAccuracyText();
            yield return null;
        }

        currentDisplayedAccuracy = targetAccuracy;
        UpdateAccuracyText();

        yield return new WaitForSecondsRealtime(0.1f);

        if (useColorAnimation)
        {
            StartCoroutine(AccuracyColorAnimation(accuracyOriginalColor, 0.3f));
        }
    }

    // ------ 정확도 스케일 애니메이션 ------
    private IEnumerator AccuracyScaleAnimation(float targetScale, float duration)
    {
        if (accuracyText == null) yield break;

        Vector3 startScale = accuracyText.transform.localScale;
        Vector3 endScale = accuracyOriginalScale * targetScale;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            accuracyText.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        accuracyText.transform.localScale = endScale;
        yield return new WaitForSecondsRealtime(0.1f);
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            accuracyText.transform.localScale = Vector3.Lerp(endScale, accuracyOriginalScale, t);
            yield return null;
        }
        accuracyText.transform.localScale = accuracyOriginalScale;
    }

    // ------ 정확도 색상 애니메이션 ------
    private IEnumerator AccuracyColorAnimation(Color targetColor, float duration)
    {
        if (accuracyText == null) yield break;

        Color startColor = accuracyText.color;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            accuracyText.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        accuracyText.color = targetColor;
    }

    // ------ 정확도 텍스트 업데이트 ------
    private void UpdateAccuracyText()
    {
        if (accuracyText != null)
        {
            accuracyText.text = currentDisplayedAccuracy.ToString("F1") + "%"; // 소수점 1자리 + %
        }
    }
}