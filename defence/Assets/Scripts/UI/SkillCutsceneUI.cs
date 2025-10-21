// 파일 이름: SkillCutsceneUI.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillCutsceneUI : MonoBehaviour
{
    public static SkillCutsceneUI instance;

    [Header("UI 연결")]
    [Tooltip("컷신 이미지가 표시될 패널")]
    public GameObject cutscenePanel;
    [Tooltip("컷신 이미지 컴포넌트")]
    public Image cutsceneImage;

    [Header("애니메이션 설정")]
    [Tooltip("컷신 표시 시간 (초)")]
    public float displayDuration = 1.0f;
    [Tooltip("페이드인 시간 (초)")]
    public float fadeInDuration = 0.2f;
    [Tooltip("페이드아웃 시간 (초)")]
    public float fadeOutDuration = 0.3f;
    [Tooltip("컷신 이미지 스케일 애니메이션 사용")]
    public bool useScaleAnimation = true;
    [Tooltip("시작 스케일 배율")]
    public float startScale = 1.2f;
    [Tooltip("끝 스케일 배율")]
    public float endScale = 1.0f;

    private CanvasGroup canvasGroup;
    private bool isPlaying = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // ------ 수정: Panel에서 CanvasGroup 가져오기 ------
        if (cutscenePanel != null)
        {
            canvasGroup = cutscenePanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = cutscenePanel.AddComponent<CanvasGroup>();
            }

            // ------ 수정: 시작 시 패널 숨김 (비활성화 대신 투명하게) ------
            canvasGroup.alpha = 0f;
            cutscenePanel.SetActive(true); // 항상 활성화 상태로 유지
        }
    }

    // ------ 스킬 컷신 재생 함수 ------
    public void PlayCutscene(Sprite cutsceneSprite)
    {
        if (cutsceneSprite == null)
        {
            Debug.LogWarning("SkillCutsceneUI: 컷신 스프라이트가 null입니다!");
            return;
        }

        if (isPlaying)
        {
            Debug.Log("SkillCutsceneUI: 이미 컷신이 재생 중입니다.");
            return;
        }

        // ------ 수정: 이 스크립트는 항상 활성화된 오브젝트에 있으므로 코루틴 시작 가능 ------
        StartCoroutine(PlayCutsceneCoroutine(cutsceneSprite));
    }

    // ------ 컷신 재생 코루틴 ------
    private IEnumerator PlayCutsceneCoroutine(Sprite cutsceneSprite)
    {
        isPlaying = true;

        // 이미지 설정
        if (cutsceneImage != null)
        {
            cutsceneImage.sprite = cutsceneSprite;
            cutsceneImage.gameObject.SetActive(true); // 이미지 활성화
        }

        // 초기 상태 설정
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (useScaleAnimation && cutsceneImage != null)
        {
            cutsceneImage.transform.localScale = Vector3.one * startScale;
        }

        // 페이드인 + 스케일 애니메이션
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeInDuration;

            // 페이드인
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            }

            // 스케일 애니메이션
            if (useScaleAnimation && cutsceneImage != null)
            {
                float currentScale = Mathf.Lerp(startScale, endScale, t);
                cutsceneImage.transform.localScale = Vector3.one * currentScale;
            }

            yield return null;
        }

        // 완전히 표시
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        if (useScaleAnimation && cutsceneImage != null)
        {
            cutsceneImage.transform.localScale = Vector3.one * endScale;
        }

        // 지정된 시간만큼 표시
        yield return new WaitForSecondsRealtime(displayDuration);

        // 페이드아웃
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeOutDuration;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            }

            yield return null;
        }

        // 완전히 숨김
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (cutsceneImage != null)
        {
            cutsceneImage.gameObject.SetActive(false); // 이미지 비활성화
        }

        isPlaying = false;
        Debug.Log("SkillCutsceneUI: 컷신 재생 완료!");
    }

    // ------ 즉시 컷신 중단 (필요시) ------
    public void StopCutscene()
    {
        StopAllCoroutines();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (cutsceneImage != null)
        {
            cutsceneImage.gameObject.SetActive(false);
        }

        isPlaying = false;
    }
}