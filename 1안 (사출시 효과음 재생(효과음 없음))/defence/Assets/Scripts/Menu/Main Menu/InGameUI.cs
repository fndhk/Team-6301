using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text; // StringBuilder를 사용하기 위해 필요합니다.

public class InGameUI : MonoBehaviour
{
    // --- UI 요소 연결 변수들 ---
    [Header("Player Info UI")]
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI itemsText;

    [Header("Item Input UI")]
    public TMP_InputField itemInputField; // 아이템 이름을 입력받을 InputField

    [Header("Save UI")]
    public GameObject saveSlotPanel; // 세이브 슬롯 선택을 위한 패널 UI
    public TextMeshProUGUI[] saveSlotInfoTexts;
    [Header("Overwrite Confirmation UI")]
    public GameObject confirmOverwritePanel; // 덮어쓰기 확인 패널
    public TextMeshProUGUI confirmOverwriteText; // 덮어쓰기 안내 텍스트
    private int selectedSlotIndex = -1; // 사용자가 선택한 슬롯 번호를 임시 저장할 변수

    // 스크립트가 활성화될 때 처음 한 번 호출되는 함수입니다.
    void Start()
    {
        // 씬이 시작될 때 현재 게임 데이터를 기반으로 UI를 업데이트합니다.
        UpdateAllUI();
    }

    // UI의 모든 텍스트 정보를 현재 GameData 기준으로 새로고침하는 함수입니다.
    public void UpdateAllUI()
    {
        GameData data = SaveLoadManager.instance.gameData;

        nicknameText.text = "Player: " + data.nickname;
        stageText.text = "Stage: " + data.currentStage;

        // 아이템 목록을 UI에 표시하기 위해 문자열을 만듭니다.
        StringBuilder itemsStringBuilder = new StringBuilder("Items:\n");
        if (data.items.Count > 0)
        {
            foreach (string item in data.items)
            {
                // 각 아이템을 한 줄씩 추가합니다.
                itemsStringBuilder.Append("- ").Append(item).Append("\n");
            }
        }
        else
        {
            itemsStringBuilder.Append("You have no items.");
        }
        itemsText.text = itemsStringBuilder.ToString();
    }

    // --- 아래부터는 UI 버튼과 연결될 함수들입니다. ---

    #region Player_Data_Controls
    // 레벨업 버튼을 클릭했을 때 호출됩니다.
    public void OnClickLevelUp()
    {
        UpdateAllUI();
    }

    // 레벨다운 버튼을 클릭했을 때 호출됩니다.
    public void OnClickLevelDown()
    {
       
    }

    // 다음 스테이지 버튼을 클릭했을 때 호출됩니다.
    public void OnClickStageNext()
    {
        // 스테이지가 10을 넘어가지 않도록 방지합니다.
        if (SaveLoadManager.instance.gameData.currentStage < 10)
        {
            SaveLoadManager.instance.gameData.currentStage++;
            UpdateAllUI();
        }
    }

    // 이전 스테이지 버튼을 클릭했을 때 호출됩니다.
    public void OnClickStagePrev()
    {
        // 스테이지가 1보다 작아지지 않도록 방지합니다.
        if (SaveLoadManager.instance.gameData.currentStage > 1)
        {
            SaveLoadManager.instance.gameData.currentStage--;
            UpdateAllUI();
        }
    }
    #endregion

    #region Item_Controls
    // '아이템 추가' 확인 버튼을 클릭했을 때 호출됩니다.
    public void OnClickConfirmAddItem()
    {
        string newItemName = itemInputField.text;

        // 입력창이 비어있지 않을 때만 아이템을 추가합니다.
        if (!string.IsNullOrWhiteSpace(newItemName))
        {
            SaveLoadManager.instance.gameData.items.Add(newItemName);

            // 다음 입력을 위해 입력창을 깨끗하게 비웁니다.
            itemInputField.text = "";

            // 새 아이템이 반영되도록 UI를 새로고침합니다.
            UpdateAllUI();
        }
    }
    #endregion

    #region Scene_And_Save_Controls
    // '메인 메뉴로' 버튼을 클릭했을 때 호출됩니다.
    public void OnClickReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // '저장하기' 버튼을 클릭했을 때 호출됩니다.
    private void UpdateSaveSlotsUI()
    {
        for (int i = 0; i < saveSlotInfoTexts.Length; i++)
        {
            int slotIndex = i + 1;
            GameData summaryData = SaveLoadManager.instance.LoadSaveSummary(slotIndex);

            if (summaryData != null)
            {
                // 데이터가 있으면 요약 정보를 표시합니다.
                saveSlotInfoTexts[i].text = $"Player: {summaryData.nickname}\n" +
                                            $"Last Save: {summaryData.lastSaveTime}";
            }
            else
            {
                // 데이터가 없으면 "비어있음"을 표시합니다.
                saveSlotInfoTexts[i].text = "- Empty -";
            }
        }
    }
    public void OnClickOpenSavePanel()
    {
        UpdateSaveSlotsUI();
        saveSlotPanel.SetActive(true); // 세이브 패널을 화면에 표시합니다.
    }

    // 세이브 패널의 '닫기' 버튼을 클릭했을 때 호출됩니다.
    public void OnClickCloseSavePanel()
    {
        saveSlotPanel.SetActive(false); // 세이브 패널을 숨깁니다.
    }

    // 1~5번 세이브 슬롯 버튼 중 하나를 클릭했을 때 호출됩니다.
    // 1~5번 세이브 슬롯 버튼 중 하나를 클릭했을 때 호출됩니다.
    public void OnClickSelectSaveSlot(int slotIndex)
    {
        // --- 아래 Debug.Log 한 줄만 추가해주세요 ---
        Debug.Log($"--- OnClickSelectSaveSlot called by '{this.gameObject.name}' with index: {slotIndex} ---");

        // 만약 해당 슬롯에 이미 파일이 존재한다면,
        if (SaveLoadManager.instance.DoesSaveFileExist(slotIndex))
        {
            // (이하 기존 코드와 동일)
            this.selectedSlotIndex = slotIndex;
            confirmOverwriteText.text = $"Aleady data in SLOT {slotIndex}\nOverWrite?";
            confirmOverwritePanel.SetActive(true);
            saveSlotPanel.SetActive(false);
        }
        // 파일이 존재하지 않는다면,
        else
        {
            Debug.Log($"Saving game to new slot {slotIndex}...");
            SaveLoadManager.instance.SaveGame(slotIndex);
            saveSlotPanel.SetActive(false);
        }
    }
    public void OnClickConfirmOverwriteYes()
    {
        // 임시 저장해둔 슬롯 번호로 저장을 진행합니다.
        Debug.Log($"Overwriting and saving game to slot {selectedSlotIndex}...");
        SaveLoadManager.instance.SaveGame(this.selectedSlotIndex);
        // 확인 창을 닫습니다.
        confirmOverwritePanel.SetActive(false);
        // 임시 슬롯 번호를 초기화합니다.
        this.selectedSlotIndex = -1;
    }

    // 덮어쓰기 확인 창에서 "아니오"를 눌렀을 때 호출됩니다.
    public void OnClickConfirmOverwriteNo()
    {
        // 확인 창만 닫고,
        confirmOverwritePanel.SetActive(false);
        this.selectedSlotIndex = -1;
        // 다시 슬롯 선택 창을 열어줍니다. (사용자 편의성)
        saveSlotPanel.SetActive(true);
    }
    
    #endregion
}