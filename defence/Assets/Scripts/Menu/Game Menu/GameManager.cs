using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가 (필요시)

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject menuPanel;
    public GameObject gameOverPanel; // 게임오버 패널 연결

    [Header("Stage Clear UI")]
    public GameObject stageClearPanel; // 방금 만든 클리어 패널 연결
    public TextMeshProUGUI scoreText; // 점수 텍스트 연결 (임시)
    public CoreFacility coreFacility;

    private bool isPaused = false;
    private bool isGameEnded = false; // 게임 종료 여부 확인 변수

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
                // 이미 일시 정지 상태이면, 게임 재개
                ResumeGame();
            }
            else
            {
                // 일시 정지 상태가 아니면, 게임 정지
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

    // --- UI 버튼과 연결할 함수들 ---

    public void OnClickRetry()
    {
        Time.timeScale = 1f;
        // 현재 씬(GameScene)을 다시 로드하면, GameSession에 저장된 스테이지 정보로 재시작됨
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickStageSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StageSelect");
    }

    public void OnClickNextStage()
    {
        Time.timeScale = 1f; // 시간 정지 해제

        // 1. 현재 스테이지 정보 가져오기
        StageData currentStage = GameSession.instance.selectedStage;

        // 2. Resources 폴더에서 모든 스테이지 데이터를 불러와 순서대로 정렬
        List<StageData> allStages = Resources.LoadAll<StageData>("StageData").OrderBy(data => data.stageIndex).ToList();

        // 3. 현재 스테이지의 다음 스테이지 찾기
        int currentIndexInList = allStages.FindIndex(data => data == currentStage);
        int nextStageIndexInList = currentIndexInList + 1;

        // 4. 다음 스테이지가 존재한다면
        if (nextStageIndexInList < allStages.Count)
        {
            // GameSession에 다음 스테이지 정보를 새로 설정
            GameSession.instance.selectedStage = allStages[nextStageIndexInList];

            // GameScene을 다시 로드
            SceneManager.LoadScene("GameScene");
        }
        else // 다음 스테이지가 없다면 (마지막 스테이지 클리어)
        {
            Debug.Log("모든 스테이지를 클리어했습니다!");
            // 스테이지 선택 화면으로 이동
            SceneManager.LoadScene("StageSelect");
        }
    }
    public void GameOver()
    {
        // 이미 게임이 끝나있지 않다면
        if (gameOverPanel.activeSelf == false)
        {
            Debug.Log("게임 오버!");
            gameOverPanel.SetActive(true); // 게임오버 패널을 켠다
            Time.timeScale = 0f;         // 게임의 시간을 멈춘다
        }
    }
    public void RestartGame()
    {
        Time.timeScale = 1f; // 시간을 다시 원래대로 되돌리고
        // 현재 씬을 다시 로드함
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 게임을 정지시키는 함수
    private void PauseGame()
    {
        isPaused = true;
        menuPanel.SetActive(true); // 메뉴 패널을 켠다
        Time.timeScale = 0f; // ★★★ 게임의 시간을 0배속으로 만들어 모든 것을 멈춘다 ★★★
    }

    // 게임을 재개시키는 함수 (버튼에서 호출할 것이므로 public으로 선언)
    public void ResumeGame()
    {
        isPaused = false;
        menuPanel.SetActive(false); // 메뉴 패널을 끈다
        Time.timeScale = 1f; // 게임의 시간을 다시 1배속으로 되돌린다
    }

    // 게임을 종료하는 함수 (버튼에서 호출할 것이므로 public으로 선언)
    public void QuitGame()
    {
        Debug.Log("게임을 종료합니다."); // 에디터에서는 확인용 메시지만 출력됨
        Application.Quit(); // 빌드된 게임에서는 프로그램이 종료됨
    }
}