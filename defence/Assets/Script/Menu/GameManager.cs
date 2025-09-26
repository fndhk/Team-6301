using UnityEngine;
using UnityEngine.SceneManagement; // �� ������ ���� �߰� (�ʿ��)

public class GameManager : MonoBehaviour
{
    public GameObject menuPanel; // ������ �޴� �г� UI
    private bool isPaused = false; // ������ �Ͻ� ���� �������� Ȯ���ϴ� ����

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