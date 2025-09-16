using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshProUGUI�� ����ϱ� ���� �ʿ��մϴ�.

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject loadGamePanel; // �ҷ����� ���Ե��� �ִ� �г�
    public GameObject confirmDeletePanel;
    [Header("Slot UI Elements")]
    // �� ������ ������ ǥ���� Text ������Ʈ �迭�Դϴ�.
    public TextMeshProUGUI[] slotInfoTexts;
    // �� ���Կ� �ش��ϴ� �ҷ����� ��ư �迭�Դϴ�.
    public UnityEngine.UI.Button[] slotLoadButtons;
    private int slotIndexToDelete = -1;
    // "ó������" ��ư�� ������ ȣ��� �Լ�
    public void OnClickStartNewGame()
    {
        SaveLoadManager.instance.gameData = new GameData();
        SceneManager.LoadScene("NicknameSetup");
    }

    // "�ҷ�����" ��ư�� ������ ȣ��� �Լ�
    public void OnClickOpenLoadPanel()
    {
        UpdateLoadSlotsUI();
        loadGamePanel.SetActive(true);
    }

    // �ҷ����� �г��� "�ݱ�" ��ư�� ������ ȣ��� �Լ�
    public void OnClickCloseLoadPanel()
    {
        loadGamePanel.SetActive(false);
    }

    // �� ������ UI ������ ������Ʈ�ϴ� �Լ�
    private void UpdateLoadSlotsUI()
    {
        // 1������ 5�� ���Ա��� �ݺ��մϴ�.
        for (int i = 0; i < 3; i++)
        {
            int slotIndex = i + 1;
            // �ش� ���Կ� ����� ������ ��� ������ �ҷ��ɴϴ�.
            GameData summaryData = SaveLoadManager.instance.LoadSaveSummary(slotIndex);

            // ����� �����Ͱ� �ִٸ�,
            if (summaryData != null)
            {
                // �ؽ�Ʈ�� �г���, ����, ��������, ���� �ð� ������ ǥ���մϴ�.
                slotInfoTexts[i].text = $"name: {summaryData.nickname}\n" +
                                        $"Level: {summaryData.playerLevel}\n" +
                                        $"Stage: {summaryData.currentStage}\n" +
                                        $"Last Play: {summaryData.lastSaveTime}";
                // �ҷ����� ��ư�� Ȱ��ȭ�մϴ�.
                slotLoadButtons[i].interactable = true;
            }
            // ����� �����Ͱ� ���ٸ�,
            else
            {
                // "�������" �̶�� ǥ���ϰ�,
                slotInfoTexts[i].text = "- Empty -";
                // ��ư�� ��Ȱ��ȭ�մϴ�.
                slotLoadButtons[i].interactable = false;
            }
        }
    }

    // ������ �ҷ����� ��ư�� Ŭ������ �� ȣ��� �Լ�
    public void OnClickLoadFromSlot(int slotIndex)
    {
        // SaveLoadManager�� ���� �ش� ������ ���� �����͸� ������ �ҷ��ɴϴ�.
        if (SaveLoadManager.instance.LoadGame(slotIndex))
        {
            // �ҷ����⿡ �����ߴٸ� InGame ������ �̵��մϴ�.
            SceneManager.LoadScene("InGame");
        }
        else
        {
            // ������ ��� �ҷ����⿡ �����ߴٸ�, ���� �޽����� ǥ���մϴ�.
            Debug.LogError($"Failed to load game from slot {slotIndex}!");
        }
    }
    // '����' ��ư�� Ŭ������ �� (���� Ȯ�� â ����)
    public void OnClickDeleteSlot(int slotIndex)
    {
        this.slotIndexToDelete = slotIndex;
        confirmDeletePanel.SetActive(true);
    }

    // ���� Ȯ�� â���� "��"�� ������ ��
    public void OnClickConfirmDeleteYes()
    {
        SaveLoadManager.instance.DeleteSaveFile(slotIndexToDelete);
        confirmDeletePanel.SetActive(false);
        UpdateLoadSlotsUI(); // ���� UI ���ΰ�ħ
    }

    // ���� Ȯ�� â���� "�ƴϿ�"�� ������ ��
    public void OnClickConfirmDeleteNo()
    {
        this.slotIndexToDelete = -1;
        confirmDeletePanel.SetActive(false);
    }

    // "���� ����" ��ư�� ������ ȣ��� �Լ�
    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}