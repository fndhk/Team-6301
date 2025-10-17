// ���� �̸�: GameManager.cs
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
        // GameScene�� ���۵� ������ ����ִ� �� ī��Ʈ�� 0���� �ʱ�ȭ
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

        // 1. ���� ���������� Ŭ���� ������ �����ɴϴ�.
        int reward = currentStage.clearReward;

        // 2. �÷��̾��� ���� ��ῡ ������ �����ݴϴ�.
        gameData.enhancementMaterials += reward;
        Debug.Log($"�������� Ŭ����! ��ȭ ��� {reward}�� ȹ��!");

        // ------ �ű� �߰�: ��ȭ UI ������Ʈ ------
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

    // --- UI ��ư�� ������ �Լ��� ---

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
            Debug.Log("��� ���������� Ŭ�����߽��ϴ�!");
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

            Debug.Log("���� ����!");
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

        // 음악 재개
        if (AudioManager.instance != null)
        {
           AudioManager.instance.ResumeMusic();
        }
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료 버튼");
        Application.Quit();
    }

    public void OnClickPause_Resume()
    {
        ResumeGame();
    }

    public void OnClickPause_Restart()
    {
        // 재시작 시에는 음악을 완전 정지(Stop)해도 좋음
        if (AudioManager.instance != null) AudioManager.instance.StopMusic();
        RestartGame(); // 기존 구현 호출
    }

    public void OnClickPause_StageSelect()
    {
        if (AudioManager.instance != null) AudioManager.instance.StopMusic();
        OnClickStageSelect(); // 기존 구현: StageSelect 씬 이동
    }

    // 설정창 열기(있는 경우) — 없으면 패널만 띄우면 됩니다.
    [SerializeField] private GameObject settingsPanel; // 인스펙터에 패널 연결
    public void OnClickPause_Settings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }
}