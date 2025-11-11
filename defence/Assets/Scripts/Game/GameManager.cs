using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject menuPanel;
    public GameObject gameOverPanel;
    public static GameManager instance;
    [Header("Stage Clear UI")]
    public GameObject stageClearPanel;
    public TextMeshProUGUI scoreText;
    public CoreFacility coreFacility;

    [Header("타워 설정")]
    public List<BaseTower> allTowersInOrder;

    private bool isPaused = false;
    private bool isGameEnded = false;

    void Awake()
    {
        // ---  싱글톤 인스턴스 설정 (가장 중요!) ---
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // 이미 인스턴스가 있다면 이 오브젝트는 파괴
            Destroy(gameObject);
            return; // Awake의 나머지 코드를 실행하지 않음
        }
        // ---  (여기까지 추가) ---

        Enemy.liveEnemyCount = 0;
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetScore();
        }

        if (SaveLoadManager.instance != null && SaveLoadManager.instance.gameData != null)
        {
            // 1. 저장된 데이터에서 해금된 타워 개수를 불러옵니다. (예: 상점에서 구매했다면 '2')
            int unlockedCount = SaveLoadManager.instance.gameData.unlockedTowerCount;

            Debug.Log($"[GameManager] 불러온 해금된 타워 수: {unlockedCount}");

            // 2. Inspector에 연결된 모든 타워를 순회합니다.
            for (int i = 0; i < allTowersInOrder.Count; i++)
            {
                if (allTowersInOrder[i] != null)
                {
                    // 3. 타워의 순번(i)이 해금된 개수(unlockedCount)보다 작은지 확인합니다.
                    // 예: i=0 (타워1) -> 0 < 2 (True) -> 레벨 1로 활성화
                    // 예: i=1 (타워2) -> 1 < 2 (True) -> 레벨 1로 활성화
                    // 예: i=2 (타워3) -> 2 < 2 (False) -> 레벨 0으로 비활성화
                    if (i < unlockedCount)
                    {
                        allTowersInOrder[i].SetLevel(1); // 1레벨(활성화)로 설정
                    }
                    else
                    {
                        allTowersInOrder[i].SetLevel(0); // 0레벨(비활성화)로 설정
                    }
                }
            }
        }
        else
        {
            Debug.LogError("[GameManager] SaveLoadManager 또는 GameData를 찾을 수 없어 타워 레벨을 설정할 수 없습니다!");
        }
    }

        void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CountdownUI.instance != null && CountdownUI.instance.isCountingDown)
            {
                return; // 카운트다운 중에는 아무것도 하지 않음
            }

            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }


    }

    public void StageClear()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.FadeOutMusic(1f);
        }

        int finalScore = 0;
        if (ScoreManager.instance != null && coreFacility != null)
        {
            float healthPercentage = coreFacility.GetCurrentHealthPercentage();
            finalScore = ScoreManager.instance.GetFinalScore(healthPercentage);
        }

        GameData gameData = SaveLoadManager.instance.gameData;
        StageData currentStage = GameSession.instance.selectedStage;

        int reward = currentStage.clearReward;

        gameData.enhancementMaterials += reward;
        Debug.Log($"{reward}");



        if (gameData.stageHighScores.ContainsKey(currentStage.stageIndex))
        {
            if (finalScore > gameData.stageHighScores[currentStage.stageIndex])
            {
                gameData.stageHighScores[currentStage.stageIndex] = finalScore;
            }
        }
        else
        {
            gameData.stageHighScores.Add(currentStage.stageIndex, finalScore);
        }

        if (currentStage.stageIndex > gameData.highestClearedStage)
        {
            gameData.highestClearedStage = currentStage.stageIndex;
        }

        StageData clearedStage = GameSession.instance.selectedStage;
        string stageID = clearedStage.stageIndex.ToString();
        GameData data = SaveLoadManager.instance.gameData;

        // 1. 현재 스테이지의 기본 보상을 가져옵니다.
        int baseReward = currentStage.clearReward;

        // 2. 플레이어 강화 레벨을 가져옵니다.
        int rewardLevel = gameData.clearRewardBonusLevel;

        // 3. 보상 보너스 배율을 계산합니다 (레벨당 0.2배 = 20%).
        float bonusMultiplier = 1f + (rewardLevel * 0.2f); // 기본 1배 + 보너스

        // 4. 최종 보상 = 기본 보상 * 보상 배율 (정수로 반올림)
        int finalReward = Mathf.RoundToInt(baseReward * bonusMultiplier);

        // 5. 플레이어의 강화 재료에 최종 보상을 추가합니다.
        gameData.enhancementMaterials += finalReward;
        Debug.Log($"스테이지 클리어! 기본 보상: {baseReward}, 보너스: +{(bonusMultiplier - 1f) * 100:F0}%, 최종 획득: {finalReward}개!");

        if (!data.firstClearRewards.Contains(stageID))
        {
            data.firstClearRewards.Add(stageID);
            data.gachaTickets += 1; // 첫 클리어 시 티켓 1개 지급
            Debug.Log("첫 클리어 보너스! 티켓 +1");
        }
        SaveLoadManager.instance.SaveGame(GameSession.instance.currentSaveSlot);

        stageClearPanel.SetActive(true);
        scoreText.text = "Score: " + finalScore.ToString("N0");
        Time.timeScale = 0f;

    }


    public void OnClickRetry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickStageSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StageSelect");
    }

    public void OnClickNextStage()
    {
        Time.timeScale = 1f;

        StageData currentStage = GameSession.instance.selectedStage;
        List<StageData> allStages = Resources.LoadAll<StageData>("StageData").OrderBy(data => data.stageIndex).ToList();

        int currentIndexInList = allStages.FindIndex(data => data == currentStage);
        int nextStageIndexInList = currentIndexInList + 1;

        if (nextStageIndexInList < allStages.Count)
        {
            GameSession.instance.selectedStage = allStages[nextStageIndexInList];
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            SceneManager.LoadScene("StageSelect");
        }
    }

    public void GameOver()
    {
        if (gameOverPanel.activeSelf == false)
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.StopMusic();
            }

            Debug.Log("Die!");
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void PauseGame()
    {
        SetPaused(true);
        menuPanel.SetActive(true);
        Time.timeScale = 0f;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PauseMusic();
        }
    }

    public void ResumeGame()
    {
        // 1. ESC 패널(menuPanel)을 닫습니다.
        menuPanel.SetActive(false);

        // 5. CountdownUI에게 "재개 카운트다운"을 시작하라고 알립니다.
        if (CountdownUI.instance != null)
        {
            // ---  (핵심 수정)  ---
            // 코루틴을 부르기 전에, 비활성화되어 있을 수 있는
            // CountdownUI 오브젝트를 먼저 활성화(SetActive(true))시킵니다.
            CountdownUI.instance.gameObject.SetActive(true);

            // 이제 활성화되었으니 코루틴을 시작합니다.
            CountdownUI.instance.StartResumeCountdown();
        }
        else
        {
            // 6. (안전장치)
            Debug.LogError("CountdownUI 인스턴스를 찾을 수 없어 즉시 재개합니다.");
            SetPaused(false);
            Time.timeScale = 1f;
            if (AudioManager.instance != null)
            {
                AudioManager.instance.ResumeMusic();
            }
        }
    }


    

    public void QuitGame()
    {
        Debug.Log("quitgame");
        Application.Quit();
    }

    public void OnClickPause_Resume()
    {
        ResumeGame();
    }

    public void OnClickPause_Restart()
    {
        if (AudioManager.instance != null) AudioManager.instance.StopMusic();
        RestartGame();
    }

    public void OnClickPause_StageSelect()
    {
        if (AudioManager.instance != null) AudioManager.instance.StopMusic();
        OnClickStageSelect();
    }

    [SerializeField] private GameObject settingsPanel;
    public void OnClickPause_Settings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }
    /// <summary>
    /// 게임의 일시정지 상태를 설정합니다. (CountdownUI에서 이 함수를 호출)
    /// </summary>
    /// <param name="state">true = 일시정지, false = 재개</param>
    public void SetPaused(bool state)
    {
        isPaused = state;
        Debug.Log($"<color=magenta>GameManager: isPaused 상태가 {state}로 변경됨</color>");
    }
}
