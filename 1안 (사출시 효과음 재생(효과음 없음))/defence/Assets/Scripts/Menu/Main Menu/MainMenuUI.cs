using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject loadGamePanel;
    public GameObject confirmDeletePanel;

    [Header("Slot UI Elements")]
    public TextMeshProUGUI[] slotInfoTexts;
    public UnityEngine.UI.Button[] slotLoadButtons;

    private int slotIndexToDelete = -1;

    // ���� �г��� '�ҷ�����' ������� '�� ����' ������� �����ϱ� ���� ����
    private enum SlotPanelMode { Load, NewGame }
    private SlotPanelMode currentMode;

    // "�� ���� ����" ��ư�� ������ ȣ��� �Լ�
    public void OnClickStartNewGame()
    {
        // 1. ���� ��带 '�� ����'���� �����մϴ�.
        currentMode = SlotPanelMode.NewGame;

        // 2. ���� ���� �г��� ���ϴ�.
        UpdateLoadSlotsUI(); // UI�� �ֽ� ������ ������Ʈ
        loadGamePanel.SetActive(true);
    }

    // "�ҷ�����" ��ư�� ������ ȣ��� �Լ�
    public void OnClickOpenLoadPanel()
    {
        // 1. ���� ��带 '�ҷ�����'�� �����մϴ�.
        currentMode = SlotPanelMode.Load;

        // 2. ���� ���� �г��� ���ϴ�.
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
        for (int i = 0; i < 3; i++)
        {
            int slotIndex = i + 1;
            GameData summaryData = SaveLoadManager.instance.LoadSaveSummary(slotIndex);

            if (summaryData != null)
            {
                slotInfoTexts[i].text = $"Name: {summaryData.nickname}\n" +
                                        $"Stage: {summaryData.highestClearedStage}\n" +
                                        $"Last Play: {summaryData.lastSaveTime}";

                // '�ҷ�����' ����� ���� �����Ͱ� �ִ� ������ Ȱ��ȭ�մϴ�.
                // '�� ����' ��忡���� ��� ������ ������ �� �־�� �ϹǷ� �׻� Ȱ��ȭ�մϴ�.
                slotLoadButtons[i].interactable = (currentMode == SlotPanelMode.NewGame) ? true : true;
            }
            else
            {
                slotInfoTexts[i].text = "- Empty -";

                // '�ҷ�����' ��忡���� ����ִ� ������ ��Ȱ��ȭ�մϴ�.
                slotLoadButtons[i].interactable = (currentMode == SlotPanelMode.NewGame) ? true : false;
            }
        }
    }

    // ���� ��ư�� Ŭ������ �� ȣ��� �Լ� (���� ū ��ȭ)
    public void OnClickLoadFromSlot(int slotIndex)
    {
        // --- ���� ��尡 '�ҷ�����'�� ��� ---
        if (currentMode == SlotPanelMode.Load)
        {
            if (SaveLoadManager.instance.LoadGame(slotIndex))
            {
                GameSession.instance.currentSaveSlot = slotIndex;
                SceneManager.LoadScene("StageSelect");
            }
            else
            {
                Debug.LogError($"���� {slotIndex}���� ������ �ҷ����µ� �����߽��ϴ�!");
            }
        }
        // --- ���� ��尡 '�� ����'�� ��� ---
        else if (currentMode == SlotPanelMode.NewGame)
        {
            // TODO: ���� �� ���Կ� �̹� �����Ͱ� �ִٸ� "����ðڽ��ϱ�?" Ȯ�� â�� ���� ������ �߰��ϸ� �� �����ϴ�.
            // ������ �ٷ� ������� �����մϴ�.

            GameSession.instance.currentSaveSlot = slotIndex;
            SaveLoadManager.instance.gameData = new GameData(); // �� ���� �����͸� �����մϴ�.
            SceneManager.LoadScene("NicknameSetup");

            loadGamePanel.SetActive(false); // �۾��� �������� �г��� �ݽ��ϴ�.
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