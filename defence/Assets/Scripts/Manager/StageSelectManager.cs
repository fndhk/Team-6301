//���� ��: StageSelectManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    [Header("UI ����")]
    public RectTransform contentPanel;
    public GameObject stageButtonPrefab;
    public Button nextButton;
    public Button prevButton;
    [Tooltip("ĳ���� ���� UI�� �ִ� �г� ���� ������Ʈ")]
    public GameObject characterSelectPanel;

    [Header("�������� ������")]
    public List<StageData> stageDatas = new List<StageData>();

    [Header("ĳ���� ���� UI")]
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

        Debug.Log($"{stageDatas.Count}���� �������� �����͸� ã�ҽ��ϴ�.");

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

            // ���� ���� ���ǿ� ���� ���õ� ĳ���� ������ ������Ʈ (SkillManager ���� ����) ����
            string currentCharID = SaveLoadManager.instance.gameData.currentSelectedCharacterID;
            if (!string.IsNullOrEmpty(currentCharID))
            {
                GameSession.instance.selectedCharacter = GetCharacterByID(currentCharID);
            }
            else // Ȥ�� �� ���� ó�� (�⺻ ĳ���� ����)
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
        Debug.Log($"---[Start �Ϸ�]--- buttonWidth: {buttonWidth}, buttonSpacing: {buttonSpacing}");
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
        // ���� �ٽ� ���� �κ� ����
        // 1. Ŭ���� ��ư�� index�� ����Ͽ� �������� ���(stageDatas)���� �ùٸ� StageData�� ã���ϴ�.
        StageData selected = stageDatas[index];

        // 2. ã�� StageData�� DontDestroyOnLoad�� �����Ǵ� GameSession�� selectedStage ������ �����մϴ�.
        GameSession.instance.selectedStage = selected;
        Debug.Log($"<color=cyan>�������� ����:</color> {selected.stageName}. GameSession�� ���� �Ϸ�.");

        // 3. ���� GameScene���� �Ѿ�ϴ�.
        SceneManager.LoadScene("GameScene");
        // ���� ������� ���� ����
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
            // �� �г��� Ȱ��ȭ�մϴ�.
            characterSelectPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("StageSelectManager�� characterSelectPanel�� ������� �ʾҽ��ϴ�!");
        }
    }

    CharacterData GetCharacterByID(string id)
    {
        if (GachaManager.instance == null || GachaManager.instance.allCharacters == null) return null;
        return GachaManager.instance.allCharacters.Find(c => c != null && c.characterID == id);
    }
}
