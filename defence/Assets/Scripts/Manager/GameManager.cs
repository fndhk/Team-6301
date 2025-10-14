// 파일 이름: GameManager.cs
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

    private bool isPaused = false;
    private bool isGameEnded = false;

    void Awake()
    {
        // GameScene이 시작될 때마다 살아있는 적 카운트를 0으로 초기화
        Enemy.liveEnemyCount = 0;
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetScore();
        }
    }

    void Update()
    {
        // ESC 키를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
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

        // 1. 현재 스테이지의 클리어 보상을 가져옵니다.
        int reward = currentStage.clearReward;

        // 2. 플레이어의 보유 재료에 보상을 더해줍니다.
        gameData.enhancementMaterials += reward;
        Debug.Log($"스테이지 클리어! 강화 재료 {reward}개 획득!");

        // ------ 신규 추가: 재화 UI 업데이트 ------
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

        SaveLoadManager.instance.SaveGame(GameSession.instance.currentSaveSlot);

        stageClearPanel.SetActive(true);
        scoreText.text = "Score: " + finalScore;
        Time.timeScale = 0f;
    }

    // --- UI 버튼과 연결할 함수들 ---

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
            Debug.Log("모든 스테이지를 클리어했습니다!");
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

            Debug.Log("게임 오버!");
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
    }

    public void ResumeGame()
    {
        isPaused = false;
        menuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Debug.Log("게임을 종료합니다.");
        Application.Quit();
    }
}