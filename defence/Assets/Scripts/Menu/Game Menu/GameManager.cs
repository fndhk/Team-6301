using UnityEngine;
using UnityEngine.SceneManagement; // �� ������ ���� �߰� (�ʿ��)

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject menuPanel;
    public GameObject gameOverPanel; // ���ӿ��� �г� ����

    private bool isPaused = false;

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