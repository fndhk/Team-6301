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
        Enemy.liveEnemyCount = 0;
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetScore();
        }

        if (SaveLoadManager.instance != null && SaveLoadManager.instance.gameData != null)
        {
            int unlockedCount = SaveLoadManager.instance.gameData.unlockedTowerCount;
            for (int i = 0; i < allTowersInOrder.Count; i++)
            {
                if (allTowersInOrder[i] != null)
                {
                    // 해금된 타워는 레벨 1(활성), 아니면 레벨 0(비활성)으로 설정
                    allTowersInOrder[i].SetLevel(i < unlockedCount ? 1 : 0);
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
    {
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

        if (MaterialsUI.instance != null)
        {
            MaterialsUI.instance.OnMaterialsChanged();
        }

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

        // 2. 저장된 강화 레벨을 가져옵니다.
        int rewardLevel = gameData.clearRewardBonusLevel;

        // 3. 최종 보너스 배율을 계산합니다 (레벨당 0.2배 = 20%).
        float bonusMultiplier = 1f + (rewardLevel * 0.2f); // 기본 1배 + 보너스

        // 4. 최종 보상 = 기본 보상 * 최종 배율 (정수로 변환)
        int finalReward = Mathf.RoundToInt(baseReward * bonusMultiplier);

        // 5. 플레이어의 보유 재료에 최종 보상을 더해줍니다.
        gameData.enhancementMaterials += finalReward;
        Debug.Log($"스테이지 클리어! 기본 보상: {baseReward}, 보너스: +{(bonusMultiplier - 1f) * 100:F0}%, 최종 획득: {finalReward}개!");

        if (!data.firstClearRewards.Contains(stageID))
        {
            data.firstClearRewards.Add(stageID);
            data.gachaTickets += 3; // 첫 클리어 시 티켓 3개 지급
            Debug.Log("첫 클리어 보너스! 티켓 +3");
        }
        SaveLoadManager.instance.SaveGame(GameSession.instance.currentSaveSlot);

        stageClearPanel.SetActive(true);
        scoreText.text = "Score: " + finalScore;
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
        isPaused = true;
        menuPanel.SetActive(true);
        Time.timeScale = 0f;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PauseMusic();
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        menuPanel.SetActive(false);
        Time.timeScale = 1f;

        if (AudioManager.instance != null)
        {
           AudioManager.instance.ResumeMusic();
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
}