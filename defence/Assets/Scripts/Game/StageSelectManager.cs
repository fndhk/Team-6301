//파일 명: StageSelectManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    [Header("UI 연결")]
    public RectTransform contentPanel;
    public GameObject stageButtonPrefab;
    public Button nextButton;
    public Button prevButton;
    [Tooltip("캐릭터 선택 UI가 있는 패널 게임 오브젝트")]
    public GameObject characterSelectPanel;

    [Header("스테이지 데이터")]
    public List<StageData> stageDatas = new List<StageData>();

    [Header("캐릭터 변경 UI")]
    public Button characterChangeButton;
    public Image characterIconImage;

    private int currentStageIndex = 0;
    private float buttonWidth;
    private float buttonSpacing;
    private List<RectTransform> stageButtonRects = new List<RectTransform>();

    void Start()
    {
        if (SaveLoadManager.instance == null)
        {
            GameObject managerPrefab = Resources.Load<GameObject>("SaveLoadManager");
            if (managerPrefab != null) Instantiate(managerPrefab);
        }

        buttonSpacing = contentPanel.GetComponent<HorizontalLayoutGroup>().spacing;

        stageDatas = Resources.LoadAll<StageData>("StageData").ToList();
        stageDatas = stageDatas.OrderBy(data => data.stageIndex).ToList();

        Debug.Log($"{stageDatas.Count}개의 스테이지 데이터를 찾았습니다.");

        GameData gameData = SaveLoadManager.instance.gameData;
        if (gameData == null) gameData = new GameData();

        for (int i = 0; i < stageDatas.Count; i++)
        {
            GameObject buttonGO = Instantiate(stageButtonPrefab, contentPanel);
            stageButtonRects.Add(buttonGO.GetComponent<RectTransform>());

            StageButtonUI buttonUI = buttonGO.GetComponent<StageButtonUI>();
            StageData currentStageData = stageDatas[i]; // 현재 스테이지 데이터 가져오기

            // 버튼 배경 이미지 설정 로직
            if (buttonUI.buttonBackground != null) // 1. StageButtonUI에 Image 슬롯이 있는지 확인
            {
                if (currentStageData.stageButtonBackground != null) // 2. StageData에 이미지가 할당되어 있는지 확인
                {
                    // 3. 버튼의 배경 이미지를 StageData의 이미지로 교체
                    buttonUI.buttonBackground.sprite = currentStageData.stageButtonBackground;
                }
            }
            else
            {
                Debug.LogWarning($"StageButtonUI 프리팹에 'buttonBackground' Image가 연결되지 않았습니다.");
            }

            // 텍스트 및 잠금 설정 로직
            buttonUI.stageNameText.text = currentStageData.stageName;
            buttonUI.stageNoText.text = "Stage " + currentStageData.stageIndex;

            if (currentStageData.stageIndex > gameData.highestClearedStage + 1)
            {
                buttonUI.lockedOverlay.SetActive(true);
                buttonGO.GetComponent<Button>().interactable = false;
                buttonUI.highScoreText.text = "";
            }
            else
            {
                buttonUI.lockedOverlay.SetActive(false);
                int stageIndex = currentStageData.stageIndex;
                int highScore = 0;
                if (gameData.stageHighScores.ContainsKey(stageIndex))
                {
                    highScore = gameData.stageHighScores[stageIndex];
                }
                buttonUI.highScoreText.text = "Best: " + highScore.ToString("N0"); // 쉼표 포맷

                int index = i;
                buttonGO.GetComponent<Button>().onClick.AddListener(() => OnClickStageSelect(index));
            }
        }

        if (stageButtonRects.Count > 0)
        {
            LayoutElement layoutElement = stageButtonRects[0].GetComponent<LayoutElement>();
            if (layoutElement != null)
            {
                buttonWidth = layoutElement.preferredWidth;
            }
        }

        if (characterChangeButton != null)
        {
            UpdateCurrentCharacterUI();

            // (캐릭터 정보 업데이트 로직)
            string currentCharID = SaveLoadManager.instance.gameData.currentSelectedCharacterID;
            if (!string.IsNullOrEmpty(currentCharID))
            {
                GameSession.instance.selectedCharacter = GetCharacterByID(currentCharID);
            }
            else
            {
                CharacterData boomChar = GetCharacterByID("Char_Boom");
                if (boomChar != null)
                {
                    GameSession.instance.selectedCharacter = boomChar;
                    SaveLoadManager.instance.gameData.currentSelectedCharacterID = "Char_Boom";
                }
            }

            characterChangeButton.onClick.AddListener(OpenCharacterSelect);
        }

        UpdateArrowButtons();
        Debug.Log($"---[Start 완료]--- buttonWidth: {buttonWidth}, buttonSpacing: {buttonSpacing}");
    }
    void Update()
    {
        float targetX = -currentStageIndex * (buttonWidth + buttonSpacing);
        Vector2 targetPosition = new Vector2(targetX, contentPanel.anchoredPosition.y);
        contentPanel.anchoredPosition = Vector2.Lerp(contentPanel.anchoredPosition, targetPosition, Time.deltaTime * 10f);
    }

    public void OnClickNext()
    {
        if (currentStageIndex < stageDatas.Count - 1)
        {
            currentStageIndex++;
            UpdateArrowButtons();
        }
    }

    public void OnClickPrevious()
    {
        if (currentStageIndex > 0)
        {
            currentStageIndex--;
            UpdateArrowButtons();
        }
    }

    private void UpdateArrowButtons()
    {
        prevButton.interactable = currentStageIndex > 0;
        nextButton.interactable = currentStageIndex < stageDatas.Count - 1;
    }

    public void OnClickStageSelect(int index)
    {
        StageData selected = stageDatas[index];
        GameSession.instance.selectedStage = selected; // 선택된 스테이지는 항상 저장

        GameData gameData = SaveLoadManager.instance.gameData;

        //  컷신 시청 여부 확인
        bool alreadyWatched = gameData.watchedCutsceneStageIndices.Contains(selected.stageIndex);

        //  해당 스테이지에 진입 컷신이 있고, 아직 보지 않았다면
        if (selected.entryCutscene != null && !alreadyWatched)
        {
            Debug.Log($"<color=yellow>컷신 재생 필요:</color> {selected.stageName}. CutScene 씬으로 이동합니다.");
            GameSession.instance.cutsceneToPlay = selected.entryCutscene;
            GameSession.instance.isNewGameCutscene = false; // 새 게임 컷신이 아님
            SceneManager.LoadScene("CutScene");
        }
        else //  컷신이 없거나 이미 봤다면
        {
            if (selected.entryCutscene != null && alreadyWatched)
                Debug.Log($"<color=gray>컷신 이미 시청함:</color> {selected.stageName}. GameScene으로 바로 이동합니다.");
            else if (selected.entryCutscene == null)
                Debug.Log($"<color=gray>진입 컷신 없음:</color> {selected.stageName}. GameScene으로 바로 이동합니다.");

            SceneManager.LoadScene("GameScene");
        }
    }

    public void OnClickBackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void UpdateCurrentCharacterUI()
    {
        if (characterIconImage == null) return;
        if (SaveLoadManager.instance == null || SaveLoadManager.instance.gameData == null) return;

        string currentCharID = SaveLoadManager.instance.gameData.currentSelectedCharacterID;
        if (string.IsNullOrEmpty(currentCharID)) currentCharID = "Char_Boom";

        CharacterData currentChar = GetCharacterByID(currentCharID);

        if (currentChar != null && currentChar.characterIcon != null)
        {
            characterIconImage.sprite = currentChar.characterIcon;
        }
    }

    void OpenCharacterSelect()
    {
        if (characterSelectPanel != null)
        {
            // 이 패널을 활성화합니다.
            characterSelectPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("StageSelectManager에 characterSelectPanel이 연결되지 않았습니다!");
        }
    }

    CharacterData GetCharacterByID(string id)
    {
        if (GachaManager.instance == null || GachaManager.instance.allCharacters == null) return null;
        return GachaManager.instance.allCharacters.Find(c => c != null && c.characterID == id);
    }
}
