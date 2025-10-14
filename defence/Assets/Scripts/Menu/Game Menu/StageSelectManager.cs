using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

public class StageSelectManager : MonoBehaviour
{
    [Header("UI 연결")]
    public RectTransform contentPanel;
    public GameObject stageButtonPrefab;
    public Button nextButton;
    public Button prevButton;
    public GameObject characterSelectPanel;

    [Header("스테이지 데이터")]
    public List<StageData> stageDatas = new List<StageData>();

    private int currentStageIndex = 0;
    private float buttonWidth;
    private float buttonSpacing;
    private List<RectTransform> stageButtonRects = new List<RectTransform>();

    void Start()
    {
        // 디버그용 임시 로더 (MainMenu 씬에서 시작하지 않을 때를 대비)
        if (SaveLoadManager.instance == null)
        {
            GameObject managerPrefab = Resources.Load<GameObject>("SaveLoadManager");
            if (managerPrefab != null) Instantiate(managerPrefab);
        }

        buttonSpacing = contentPanel.GetComponent<HorizontalLayoutGroup>().spacing;

        // StageData 파일들을 Resources/StageData 폴더에서 모두 불러옵니다.
        stageDatas = Resources.LoadAll<StageData>("StageData").ToList();
        stageDatas = stageDatas.OrderBy(data => data.stageIndex).ToList();

        Debug.Log($"{stageDatas.Count}개의 스테이지 데이터를 찾았습니다.");

        GameData gameData = SaveLoadManager.instance.gameData;
        if (gameData == null) gameData = new GameData();

        // 각 스테이지 데이터에 맞춰 버튼 생성
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

        // --- [수정된 부분] ---
        // for문이 끝난 후에 버튼 너비를 계산하고, 화살표 버튼을 업데이트합니다.
        if (stageButtonRects.Count > 0)
        {
            LayoutElement layoutElement = stageButtonRects[0].GetComponent<LayoutElement>();
            if (layoutElement != null)
            {
                buttonWidth = layoutElement.preferredWidth;
            }
        }

        UpdateArrowButtons();

        // ▼▼▼▼▼ 시작 값 확인용 로그 ▼▼▼▼▼
        Debug.Log($"---[Start 완료]--- buttonWidth: {buttonWidth}, buttonSpacing: {buttonSpacing}");
    }

    // --- [수정된 부분] ---
    // Update 함수는 Start 함수와 같은 레벨에 있어야 합니다.
    void Update()
    {
        float targetX = -currentStageIndex * (buttonWidth + buttonSpacing);
        Vector2 targetPosition = new Vector2(targetX, contentPanel.anchoredPosition.y);
        contentPanel.anchoredPosition = Vector2.Lerp(contentPanel.anchoredPosition, targetPosition, Time.deltaTime * 10f);

        // ▼▼▼▼▼ 매 프레임 상태 확인용 로그 (필요시 주석 해제) ▼▼▼▼▼
        // Debug.Log($"Update - currentStageIndex: {currentStageIndex}, targetX: {targetX}, currentX: {contentPanel.anchoredPosition.x}");
    }

    public void OnClickNext()
    {
        // ▼▼▼▼▼ 함수 호출 확인용 로그 ▼▼▼▼▼
        Debug.Log("---[OnClickNext 함수 호출됨]---");
        if (currentStageIndex < stageDatas.Count - 1)
        {
            currentStageIndex++;
            Debug.Log($"currentStageIndex가 {currentStageIndex}(으)로 변경됨");
            UpdateArrowButtons();
        }
    }

    public void OnClickPrevious()
    {
        // ▼▼▼▼▼ 함수 호출 확인용 로그 ▼▼▼▼▼
        Debug.Log("---[OnClickPrevious 함수 호출됨]---");
        if (currentStageIndex > 0)
        {
            currentStageIndex--;
            Debug.Log($"currentStageIndex가 {currentStageIndex}(으)로 변경됨");
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
        Debug.Log(stageDatas[index].stageName + " 선택됨! 캐릭터 선택창을 엽니다.");

        // GameSession에 선택된 스테이지 정보를 저장
        GameSession.instance.selectedStage = stageDatas[index];

        if (characterSelectPanel != null)
        {
            characterSelectPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("StageSelectManager에 Character Select Panel이 연결되지 않았습니다!");
        }
    }
    public void OnClickBackButton()
    {
        // MainMenu 씬을 불러옵니다.
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

}