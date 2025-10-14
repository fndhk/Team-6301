// 파일 이름: InGameUI.cs (전체 교체)
using UnityEngine;
using TMPro;

public class InGameUI : MonoBehaviour
{
    [Header("Player Info UI")]
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI stageText; // 스테이지 이름 표시용

    void Start()
    {
        UpdateAllUI();
    }

    public void UpdateAllUI()
    {
        GameData data = SaveLoadManager.instance.gameData;
        nicknameText.text = "Player: " + data.nickname;

        // GameSession에서 현재 스테이지 정보를 가져와 이름을 표시합니다.
        if (GameSession.instance != null && GameSession.instance.selectedStage != null)
        {
            stageText.text = "Stage: " + GameSession.instance.selectedStage.stageName;
        }
        else
        {
            stageText.text = "Stage: Unknown";
        }
    }

    // 참고: 기존에 있던 레벨업/다운, 스테이지 이동, 아이템 추가, 저장 관련 버튼 기능들은
    // 이제 다른 스크립트(GameManager, StageSelectManager 등)에서 처리하므로
    // InGameUI에서는 제거되었습니다. 이 스크립트는 이제 정보 표시 역할만 합니다.
}