using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject loadGamePanel;
    public GameObject confirmDeletePanel;

    [Header("Slot UI Elements")]
    public TextMeshProUGUI[] slotInfoTexts;
    public UnityEngine.UI.Button[] slotLoadButtons;
    [Header("컷신 설정")]
    public CutsceneData firstPlayCutscene;
    private int slotIndexToDelete = -1;

    // 슬롯 패널이 '불러오기' 모드인지 '새 게임' 모드인지 구분하기 위한 변수
    private enum SlotPanelMode { Load, NewGame }
    private SlotPanelMode currentMode;

    // "새 게임 시작" 버튼을 누르면 호출될 함수
    public void OnClickStartNewGame()
    {
        // 1. 현재 모드를 '새 게임'으로 설정합니다.
        currentMode = SlotPanelMode.NewGame;

        // 2. 슬롯 선택 패널을 엽니다.
        UpdateLoadSlotsUI(); // UI를 최신 정보로 업데이트
        loadGamePanel.SetActive(true);
    }

    // "불러오기" 버튼을 누르면 호출될 함수
    public void OnClickOpenLoadPanel()
    {
        // 1. 현재 모드를 '불러오기'로 설정합니다.
        currentMode = SlotPanelMode.Load;

        // 2. 슬롯 선택 패널을 엽니다.
        UpdateLoadSlotsUI();
        loadGamePanel.SetActive(true);
    }

    // 불러오기 패널의 "닫기" 버튼을 누르면 호출될 함수
    public void OnClickCloseLoadPanel()
    {
        loadGamePanel.SetActive(false);
    }

    // 각 슬롯의 UI 정보를 업데이트하는 함수
    private void UpdateLoadSlotsUI()
    {
        for (int i = 0; i < 3; i++)
        {
            int slotIndex = i + 1;
            GameData summaryData = SaveLoadManager.instance.LoadSaveSummary(slotIndex);

            if (summaryData != null)
            {
                slotInfoTexts[i].text = $"Name: {summaryData.nickname}\n" +
                                        $"Stage: {summaryData.highestClearedStage}\n" +
                                        $"Last Play: {summaryData.lastSaveTime}";

                // '불러오기' 모드일 때만 데이터가 있는 슬롯을 활성화합니다.
                // '새 게임' 모드에서는 모든 슬롯을 선택할 수 있어야 하므로 항상 활성화합니다.
                slotLoadButtons[i].interactable = (currentMode == SlotPanelMode.NewGame) ? true : true;
            }
            else
            {
                slotInfoTexts[i].text = "- Empty -";

                // '불러오기' 모드에서는 비어있는 슬롯을 비활성화합니다.
                slotLoadButtons[i].interactable = (currentMode == SlotPanelMode.NewGame) ? true : false;
            }
        }
    }

    // 슬롯 버튼을 클릭했을 때 호출될 함수 (가장 큰 변화)
    public void OnClickLoadFromSlot(int slotIndex)
    {
        // --- 현재 모드가 '불러오기'일 경우 ---
        if (currentMode == SlotPanelMode.Load)
        {
            if (SaveLoadManager.instance.LoadGame(slotIndex))
            {
                GameSession.instance.currentSaveSlot = slotIndex;
                SceneManager.LoadScene("StageSelect");
            }
            else
            {
                Debug.LogError($"슬롯 {slotIndex}에서 게임을 불러오는데 실패했습니다!");
            }
        }
        // --- 현재 모드가 '새 게임'일 경우 ---
        else if (currentMode == SlotPanelMode.NewGame)
        {
            // TODO: 덮어쓰기 확인 로직 (선택사항)

            GameSession.instance.currentSaveSlot = slotIndex;
            SaveLoadManager.instance.gameData = new GameData(); // 새 게임 데이터 생성

            // --- ( 수정 시작) ---
            if (firstPlayCutscene != null)
            {
                // GameSession에 첫 컷신 정보와 '새 게임' 상태를 임시 저장
                GameSession.instance.cutsceneToPlay = firstPlayCutscene;
                GameSession.instance.isNewGameCutscene = true;
                GameSession.instance.selectedStage = null; // 스테이지 컷신이 아님을 명확히 함

                // 닉네임 설정 대신 CutScene 씬 로드
                SceneManager.LoadScene("CutScene");
            }
            else
            {
                // 첫 컷신이 없으면 바로 닉네임 설정으로
                Debug.LogWarning("첫 플레이 컷신이 설정되지 않았습니다. NicknameSetup으로 바로 이동합니다.");
                SceneManager.LoadScene("NicknameSetup");
            }
            // --- ( 수정 끝) ---

            loadGamePanel.SetActive(false);
        }
    }

    // '삭제' 버튼을 클릭했을 때 (삭제 확인 창 열기)
    public void OnClickDeleteSlot(int slotIndex)
    {
        this.slotIndexToDelete = slotIndex;
        confirmDeletePanel.SetActive(true);
    }

    // 삭제 확인 창에서 "예"를 눌렀을 때
    public void OnClickConfirmDeleteYes()
    {
        SaveLoadManager.instance.DeleteSaveFile(slotIndexToDelete);
        confirmDeletePanel.SetActive(false);
        UpdateLoadSlotsUI(); // 슬롯 UI 새로고침
    }

    // 삭제 확인 창에서 "아니오"를 눌렀을 때
    public void OnClickConfirmDeleteNo()
    {
        this.slotIndexToDelete = -1;
        confirmDeletePanel.SetActive(false);
    }

    // "게임 종료" 버튼을 누르면 호출될 함수
    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}