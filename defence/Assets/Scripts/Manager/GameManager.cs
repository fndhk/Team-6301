// ï¿½ï¿½ï¿½ï¿½ ï¿½Ì¸ï¿½: GameManager.cs
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
        // GameSceneï¿½ï¿½ ï¿½ï¿½ï¿½Ûµï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½Ö´ï¿?ï¿½ï¿½ Ä«ï¿½ï¿½Æ®ï¿½ï¿½ 0ï¿½ï¿½ï¿½ï¿½ ï¿½Ê±ï¿½È­
        Enemy.liveEnemyCount = 0;
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetScore();
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

        // 1. ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ Å¬ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½É´Ï´ï¿½.
        int reward = currentStage.clearReward;

        // 2. ï¿½Ã·ï¿½ï¿½Ì¾ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½á¿?ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½Ý´Ï´ï¿½.
        gameData.enhancementMaterials += reward;
        Debug.Log($"ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ Å¬ï¿½ï¿½ï¿½ï¿½! ï¿½ï¿½È­ ï¿½ï¿½ï¿?{reward}ï¿½ï¿½ È¹ï¿½ï¿½!");

        // ------ ï¿½Å±ï¿½ ï¿½ß°ï¿½: ï¿½ï¿½È­ UI ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Æ® ------
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

        if (!data.firstClearRewards.Contains(stageID))
        {
            data.firstClearRewards.Add(stageID);
            data.gachaTickets += 3; // Ã¹ Å¬¸®¾î ½Ã Æ¼ÄÏ 3°³ Áö±Þ
            Debug.Log("Ã¹ Å¬¸®¾î º¸³Ê½º! Æ¼ÄÏ +3");
        }
        SaveLoadManager.instance.SaveGame(GameSession.instance.currentSaveSlot);

        stageClearPanel.SetActive(true);
        scoreText.text = "Score: " + finalScore;
        Time.timeScale = 0f;

    }

    // --- UI ï¿½ï¿½Æ°ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½Ô¼ï¿½ï¿½ï¿½ ---

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
            Debug.Log("ï¿½ï¿½ï¿?ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ Å¬ï¿½ï¿½ï¿½ï¿½ï¿½ß½ï¿½ï¿½Ï´ï¿½!");
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

            Debug.Log("ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½!");
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

        // ?Œì•… ?¬ê°œ
        if (AudioManager.instance != null)
        {
           AudioManager.instance.ResumeMusic();
        }
    }

    public void QuitGame()
    {
        Debug.Log("ê²Œìž„ ì¢…ë£Œ ë²„íŠ¼");
        Application.Quit();
    }

    public void OnClickPause_Resume()
    {
        ResumeGame();
    }

    public void OnClickPause_Restart()
    {
        // ?¬ì‹œ???œì—???Œì•…???„ì „ ?•ì?(Stop)?´ë„ ì¢‹ìŒ
        if (AudioManager.instance != null) AudioManager.instance.StopMusic();
        RestartGame(); // ê¸°ì¡´ êµ¬í˜„ ?¸ì¶œ
    }

    public void OnClickPause_StageSelect()
    {
        if (AudioManager.instance != null) AudioManager.instance.StopMusic();
        OnClickStageSelect(); // ê¸°ì¡´ êµ¬í˜„: StageSelect ???´ë™
    }

    // ?¤ì •ì°??´ê¸°(?ˆëŠ” ê²½ìš°) ???†ìœ¼ë©??¨ë„ë§??„ìš°ë©??©ë‹ˆ??
    [SerializeField] private GameObject settingsPanel; // ?¸ìŠ¤?™í„°???¨ë„ ?°ê²°
    public void OnClickPause_Settings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }
}