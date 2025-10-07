using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가 (필요시)

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject menuPanel;
    public GameObject gameOverPanel; // 게임오버 패널 연결

    private bool isPaused = false;

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