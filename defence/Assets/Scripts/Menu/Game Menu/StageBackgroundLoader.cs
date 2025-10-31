// 파일 이름: StageBackgroundLoader.cs (SpriteRenderer 버전)
using UnityEngine;

public class StageBackgroundLoader : MonoBehaviour
{
    [Header("배경 연결")]
    [Tooltip("배경 이미지를 표시할 Sprite Renderer 컴포넌트입니다.")]
    // ▼▼▼ Image -> SpriteRenderer로 변경 ▼▼▼
    public SpriteRenderer backgroundRenderer;
    [Tooltip("디펜스 배경 이미지를 표시할 Sprite Renderer 컴포넌트입니다.")]
    public SpriteRenderer defenseBackgroundRenderer;
    void Start()
    {
        // 1. GameSession에서 현재 선택된 스테이지 정보를 가져옵니다.
        if (GameSession.instance == null || GameSession.instance.selectedStage == null)
        {
            Debug.LogWarning("StageBackgroundLoader: GameSession 또는 selectedStage를 찾을 수 없습니다.");
            if (backgroundRenderer != null)
                backgroundRenderer.gameObject.SetActive(false);
            if (defenseBackgroundRenderer != null)
                defenseBackgroundRenderer.gameObject.SetActive(false);
            return;
        }

        // 2. StageData에서 각 배경 이미지를 가져옵니다.
        Sprite bgSprite = GameSession.instance.selectedStage.stageBackground;

        // ▼▼▼▼▼▼▼▼▼▼ 이 줄을 빠뜨리신 것 같습니다! ▼▼▼▼▼▼▼▼▼▼
        Sprite defenseBgSprite = GameSession.instance.selectedStage.defenseBackground;
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        // --- 기본 배경 로드 ---
        if (backgroundRenderer == null)
        {
            Debug.LogError("StageBackgroundLoader: backgroundRenderer 변수에 SpriteRenderer가 연결되지 않았습니다!");
        }
        else if (bgSprite != null)
        {
            backgroundRenderer.sprite = bgSprite;
            backgroundRenderer.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("이 스테이지에는 지정된 기본 배경이 없습니다. 기본 배경을 비활성화합니다.");
            backgroundRenderer.gameObject.SetActive(false);
        }

        // --- 디펜스 배경 로드 ---
        if (defenseBackgroundRenderer == null)
        {
            Debug.LogWarning("StageBackgroundLoader: defenseBackgroundRenderer 변수에 SpriteRenderer가 연결되지 않았습니다! (디펜스 배경)");
        }
        else if (defenseBgSprite != null) // <- 여기서 오류 발생
        {
            defenseBackgroundRenderer.sprite = defenseBgSprite;
            defenseBackgroundRenderer.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("이 스테이지에는 지정된 디펜스 배경이 없습니다. 디펜스 배경을 비활성화합니다.");
            defenseBackgroundRenderer.gameObject.SetActive(false);
        }
    }
}