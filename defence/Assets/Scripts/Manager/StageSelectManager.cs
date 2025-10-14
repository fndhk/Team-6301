using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

public class StageSelectManager : MonoBehaviour
{
    [Header("UI ����")]
    public RectTransform contentPanel;
    public GameObject stageButtonPrefab;
    public Button nextButton;
    public Button prevButton;
    public GameObject characterSelectPanel;

    [Header("�������� ������")]
    public List<StageData> stageDatas = new List<StageData>();

    private int currentStageIndex = 0;
    private float buttonWidth;
    private float buttonSpacing;
    private List<RectTransform> stageButtonRects = new List<RectTransform>();

    void Start()
    {
        // ����׿� �ӽ� �δ� (MainMenu ������ �������� ���� ���� ���)
        if (SaveLoadManager.instance == null)
        {
            GameObject managerPrefab = Resources.Load<GameObject>("SaveLoadManager");
            if (managerPrefab != null) Instantiate(managerPrefab);
        }

        buttonSpacing = contentPanel.GetComponent<HorizontalLayoutGroup>().spacing;

        // StageData ���ϵ��� Resources/StageData �������� ��� �ҷ��ɴϴ�.
        stageDatas = Resources.LoadAll<StageData>("StageData").ToList();
        stageDatas = stageDatas.OrderBy(data => data.stageIndex).ToList();

        Debug.Log($"{stageDatas.Count}���� �������� �����͸� ã�ҽ��ϴ�.");

        GameData gameData = SaveLoadManager.instance.gameData;
        if (gameData == null) gameData = new GameData();

        // �� �������� �����Ϳ� ���� ��ư ����
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

        // --- [������ �κ�] ---
        // for���� ���� �Ŀ� ��ư �ʺ� ����ϰ�, ȭ��ǥ ��ư�� ������Ʈ�մϴ�.
        if (stageButtonRects.Count > 0)
        {
            LayoutElement layoutElement = stageButtonRects[0].GetComponent<LayoutElement>();
            if (layoutElement != null)
            {
                buttonWidth = layoutElement.preferredWidth;
            }
        }

        UpdateArrowButtons();

        // ������ ���� �� Ȯ�ο� �α� ������
        Debug.Log($"---[Start �Ϸ�]--- buttonWidth: {buttonWidth}, buttonSpacing: {buttonSpacing}");
    }

    // --- [������ �κ�] ---
    // Update �Լ��� Start �Լ��� ���� ������ �־�� �մϴ�.
    void Update()
    {
        float targetX = -currentStageIndex * (buttonWidth + buttonSpacing);
        Vector2 targetPosition = new Vector2(targetX, contentPanel.anchoredPosition.y);
        contentPanel.anchoredPosition = Vector2.Lerp(contentPanel.anchoredPosition, targetPosition, Time.deltaTime * 10f);

        // ������ �� ������ ���� Ȯ�ο� �α� (�ʿ�� �ּ� ����) ������
        // Debug.Log($"Update - currentStageIndex: {currentStageIndex}, targetX: {targetX}, currentX: {contentPanel.anchoredPosition.x}");
    }

    public void OnClickNext()
    {
        // ������ �Լ� ȣ�� Ȯ�ο� �α� ������
        Debug.Log("---[OnClickNext �Լ� ȣ���]---");
        if (currentStageIndex < stageDatas.Count - 1)
        {
            currentStageIndex++;
            Debug.Log($"currentStageIndex�� {currentStageIndex}(��)�� �����");
            UpdateArrowButtons();
        }
    }

    public void OnClickPrevious()
    {
        // ������ �Լ� ȣ�� Ȯ�ο� �α� ������
        Debug.Log("---[OnClickPrevious �Լ� ȣ���]---");
        if (currentStageIndex > 0)
        {
            currentStageIndex--;
            Debug.Log($"currentStageIndex�� {currentStageIndex}(��)�� �����");
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
        Debug.Log(stageDatas[index].stageName + " ���õ�! ĳ���� ����â�� ���ϴ�.");

        // GameSession�� ���õ� �������� ������ ����
        GameSession.instance.selectedStage = stageDatas[index];

        if (characterSelectPanel != null)
        {
            characterSelectPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("StageSelectManager�� Character Select Panel�� ������� �ʾҽ��ϴ�!");
        }
    }
    public void OnClickBackButton()
    {
        // MainMenu ���� �ҷ��ɴϴ�.
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

}