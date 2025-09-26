using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshProUGUI를 사용하기 위해 필요합니다.

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject loadGamePanel; // 불러오기 슬롯들이 있는 패널
    public GameObject confirmDeletePanel;
    [Header("Slot UI Elements")]
    // 각 슬롯의 정보를 표시할 Text 컴포넌트 배열입니다.
    public TextMeshProUGUI[] slotInfoTexts;
    // 각 슬롯에 해당하는 불러오기 버튼 배열입니다.
    public UnityEngine.UI.Button[] slotLoadButtons;
    private int slotIndexToDelete = -1;
    // "처음부터" 버튼을 누르면 호출될 함수
    public void OnClickStartNewGame()
    {
        SaveLoadManager.instance.gameData = new GameData();
        SceneManager.LoadScene("NicknameSetup");
    }

    // "불러오기" 버튼을 누르면 호출될 함수
    public void OnClickOpenLoadPanel()
    {
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
        // 1번부터 5번 슬롯까지 반복합니다.
        for (int i = 0; i < 3; i++)
        {
            int slotIndex = i + 1;
            // 해당 슬롯에 저장된 데이터 요약 정보를 불러옵니다.
            GameData summaryData = SaveLoadManager.instance.LoadSaveSummary(slotIndex);

            // 저장된 데이터가 있다면,
            if (summaryData != null)
            {
                // 텍스트에 닉네임, 레벨, 스테이지, 저장 시간 정보를 표시합니다.
                slotInfoTexts[i].text = $"name: {summaryData.nickname}\n" +
                                        $"Level: {summaryData.playerLevel}\n" +
                                        $"Stage: {summaryData.currentStage}\n" +
                                        $"Last Play: {summaryData.lastSaveTime}";
                // 불러오기 버튼을 활성화합니다.
                slotLoadButtons[i].interactable = true;
            }
            // 저장된 데이터가 없다면,
            else
            {
                // "비어있음" 이라고 표시하고,
                slotInfoTexts[i].text = "- Empty -";
                // 버튼을 비활성화합니다.
                slotLoadButtons[i].interactable = false;
            }
        }
    }

    // 슬롯의 불러오기 버튼을 클릭했을 때 호출될 함수
    public void OnClickLoadFromSlot(int slotIndex)
    {
        // SaveLoadManager를 통해 해당 슬롯의 게임 데이터를 실제로 불러옵니다.
        if (SaveLoadManager.instance.LoadGame(slotIndex))
        {
            // 불러오기에 성공했다면 InGame 씬으로 이동합니다.
            SceneManager.LoadScene("InGame");
        }
        else
        {
            // 만약의 경우 불러오기에 실패했다면, 에러 메시지를 표시합니다.
            Debug.LogError($"Failed to load game from slot {slotIndex}!");
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