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
            buttonUI.stageNameText.text = stageDatas[i].stageName;
            buttonUI.stageNoText.text = "Stage " + stageDatas[i].stageIndex;

            if (stageDatas[i].stageIndex > gameData.highestClearedStage + 1)
            {
                buttonUI.lockedOverlay.SetActive(true);
                buttonGO.GetComponent<Button>().interactable = false;
                buttonUI.highScoreText.text = "";
            }
            else
            {
                buttonUI.lockedOverlay.SetActive(false);
                int stageIndex = stageDatas[i].stageIndex;
                int highScore = 0;
                if (gameData.stageHighScores.ContainsKey(stageIndex))
                {
                    highScore = gameData.stageHighScores[stageIndex];
                }
                buttonUI.highScoreText.text = "Best: " + highScore;

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

            // ▼▼▼ 게임 세션에 현재 선택된 캐릭터 정보도 업데이트 (SkillManager 오류 방지) ▼▼▼
            string currentCharID = SaveLoadManager.instance.gameData.currentSelectedCharacterID;
            if (!string.IsNullOrEmpty(currentCharID))
            {
                GameSession.instance.selectedCharacter = GetCharacterByID(currentCharID);
            }
            else // 혹시 모를 예외 처리 (기본 캐릭터 설정)
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
        // ▼▼▼ 핵심 수정 부분 ▼▼▼
        // 1. 클릭된 버튼의 index를 사용하여 스테이지 목록(stageDatas)에서 올바른 StageData를 찾습니다.
        StageData selected = stageDatas[index];

        // 2. 찾은 StageData를 DontDestroyOnLoad로 유지되는 GameSession의 selectedStage 변수에 저장합니다.
        GameSession.instance.selectedStage = selected;
        Debug.Log($"<color=cyan>스테이지 선택:</color> {selected.stageName}. GameSession에 저장 완료.");

        // 3. 이제 GameScene으로 넘어갑니다.
        SceneManager.LoadScene("GameScene");
        // ▲▲▲ 여기까지 수정 ▲▲▲
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
