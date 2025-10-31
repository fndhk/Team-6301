// 파일 이름: StageBackgroundLoader.cs (SpriteRenderer 버전)
using UnityEngine;

public class StageBackgroundLoader : MonoBehaviour
{
    [Header("배경 연결")]
    [Tooltip("배경 이미지를 표시할 Sprite Renderer 컴포넌트입니다.")]
    // ▼▼▼ Image -> SpriteRenderer로 변경 ▼▼▼
    public SpriteRenderer backgroundRenderer;

    void Start()
    {
        // 1. GameSession에서 현재 선택된 스테이지 정보를 가져옵니다.
        if (GameSession.instance == null || GameSession.instance.selectedStage == null)
        {
            Debug.LogWarning("StageBackgroundLoader: GameSession 또는 selectedStage를 찾을 수 없습니다.");
            if (backgroundRenderer != null)
                backgroundRenderer.gameObject.SetActive(false); // 배경 비활성화
            return;
        }

        // 2. StageData에 배경 이미지가 할당되어 있는지 확인합니다.
        Sprite bgSprite = GameSession.instance.selectedStage.stageBackground;

        if (backgroundRenderer == null)
        {
            Debug.LogError("StageBackgroundLoader: backgroundRenderer 변수에 SpriteRenderer 컴포넌트가 연결되지 않았습니다!");
            return;
        }

        if (bgSprite != null)
        {
            // 3. 배경 이미지가 있으면, SpriteRenderer의 sprite를 교체합니다.
            backgroundRenderer.sprite = bgSprite;
            backgroundRenderer.gameObject.SetActive(true);
        }
        else
        {
            // 4. 할당된 배경 이미지가 없으면, 배경 오브젝트를 비활성화합니다.
            Debug.Log("이 스테이지에는 지정된 배경이 없습니다. 배경 오브젝트를 비활성화합니다.");
            backgroundRenderer.gameObject.SetActive(false);
        }
    }
}