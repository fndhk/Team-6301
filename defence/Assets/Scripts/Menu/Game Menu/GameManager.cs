using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // �� ������ ���� �߰� (�ʿ��)

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject menuPanel;
    public GameObject gameOverPanel; // ���ӿ��� �г� ����

    [Header("Stage Clear UI")]
    public GameObject stageClearPanel; // ��� ���� Ŭ���� �г� ����
    public TextMeshProUGUI scoreText; // ���� �ؽ�Ʈ ���� (�ӽ�)
    public CoreFacility coreFacility;

    private bool isPaused = false;
    private bool isGameEnded = false; // ���� ���� ���� Ȯ�� ����

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
        // ESC Ű�� ������ ��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                // �̹� �Ͻ� ���� �����̸�, ���� �簳
                ResumeGame();
            }
            else
            {
                // �Ͻ� ���� ���°� �ƴϸ�, ���� ����
                PauseGame();
            }
        }

    }
    public void StageClear()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        int finalScore = 0;
        if (ScoreManager.instance != null && coreFacility != null)
        {
            float healthPercentage = coreFacility.GetCurrentHealthPercentage();
            finalScore = ScoreManager.instance.GetFinalScore(healthPercentage);
        }

        GameData gameData = SaveLoadManager.instance.gameData;
        StageData currentStage = GameSession.instance.selectedStage;

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
        // ���� ��(GameScene)�� �ٽ� �ε��ϸ�, GameSession�� ����� �������� ������ ����۵�
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickStageSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StageSelect");
    }

    public void OnClickNextStage()
    {
        Time.timeScale = 1f; // �ð� ���� ����

        // 1. ���� �������� ���� ��������
        StageData currentStage = GameSession.instance.selectedStage;

        // 2. Resources �������� ��� �������� �����͸� �ҷ��� ������� ����
        List<StageData> allStages = Resources.LoadAll<StageData>("StageData").OrderBy(data => data.stageIndex).ToList();

        // 3. ���� ���������� ���� �������� ã��
        int currentIndexInList = allStages.FindIndex(data => data == currentStage);
        int nextStageIndexInList = currentIndexInList + 1;

        // 4. ���� ���������� �����Ѵٸ�
        if (nextStageIndexInList < allStages.Count)
        {
            // GameSession�� ���� �������� ������ ���� ����
            GameSession.instance.selectedStage = allStages[nextStageIndexInList];

            // GameScene�� �ٽ� �ε�
            SceneManager.LoadScene("GameScene");
        }
        else // ���� ���������� ���ٸ� (������ �������� Ŭ����)
        {
            Debug.Log("��� ���������� Ŭ�����߽��ϴ�!");
            // �������� ���� ȭ������ �̵�
            SceneManager.LoadScene("StageSelect");
        }
    }
    public void GameOver()
    {
        // �̹� ������ �������� �ʴٸ�
        if (gameOverPanel.activeSelf == false)
        {
            Debug.Log("���� ����!");
            gameOverPanel.SetActive(true); // ���ӿ��� �г��� �Ҵ�
            Time.timeScale = 0f;         // ������ �ð��� �����
        }
    }
    public void RestartGame()
    {
        Time.timeScale = 1f; // �ð��� �ٽ� ������� �ǵ�����
        // ���� ���� �ٽ� �ε���
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ������ ������Ű�� �Լ�
    private void PauseGame()
    {
        isPaused = true;
        menuPanel.SetActive(true); // �޴� �г��� �Ҵ�
        Time.timeScale = 0f; // �ڡڡ� ������ �ð��� 0������� ����� ��� ���� ����� �ڡڡ�
    }

    // ������ �簳��Ű�� �Լ� (��ư���� ȣ���� ���̹Ƿ� public���� ����)
    public void ResumeGame()
    {
        isPaused = false;
        menuPanel.SetActive(false); // �޴� �г��� ����
        Time.timeScale = 1f; // ������ �ð��� �ٽ� 1������� �ǵ�����
    }

    // ������ �����ϴ� �Լ� (��ư���� ȣ���� ���̹Ƿ� public���� ����)
    public void QuitGame()
    {
        Debug.Log("������ �����մϴ�."); // �����Ϳ����� Ȯ�ο� �޽����� ��µ�
        Application.Quit(); // ����� ���ӿ����� ���α׷��� �����
    }
}