using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text; // StringBuilder�� ����ϱ� ���� �ʿ��մϴ�.

public class InGameUI : MonoBehaviour
{
    // --- UI ��� ���� ������ ---
    [Header("Player Info UI")]
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI itemsText;

    [Header("Item Input UI")]
    public TMP_InputField itemInputField; // ������ �̸��� �Է¹��� InputField

    [Header("Save UI")]
    public GameObject saveSlotPanel; // ���̺� ���� ������ ���� �г� UI
    public TextMeshProUGUI[] saveSlotInfoTexts;
    [Header("Overwrite Confirmation UI")]
    public GameObject confirmOverwritePanel; // ����� Ȯ�� �г�
    public TextMeshProUGUI confirmOverwriteText; // ����� �ȳ� �ؽ�Ʈ
    private int selectedSlotIndex = -1; // ����ڰ� ������ ���� ��ȣ�� �ӽ� ������ ����

    // ��ũ��Ʈ�� Ȱ��ȭ�� �� ó�� �� �� ȣ��Ǵ� �Լ��Դϴ�.
    void Start()
    {
        // ���� ���۵� �� ���� ���� �����͸� ������� UI�� ������Ʈ�մϴ�.
        UpdateAllUI();
    }

    // UI�� ��� �ؽ�Ʈ ������ ���� GameData �������� ���ΰ�ħ�ϴ� �Լ��Դϴ�.
    public void UpdateAllUI()
    {
        GameData data = SaveLoadManager.instance.gameData;

        nicknameText.text = "Player: " + data.nickname;
        stageText.text = "Stage: " + data.currentStage;

        // ������ ����� UI�� ǥ���ϱ� ���� ���ڿ��� ����ϴ�.
        StringBuilder itemsStringBuilder = new StringBuilder("Items:\n");
        if (data.items.Count > 0)
        {
            foreach (string item in data.items)
            {
                // �� �������� �� �پ� �߰��մϴ�.
                itemsStringBuilder.Append("- ").Append(item).Append("\n");
            }
        }
        else
        {
            itemsStringBuilder.Append("You have no items.");
        }
        itemsText.text = itemsStringBuilder.ToString();
    }

    // --- �Ʒ����ʹ� UI ��ư�� ����� �Լ����Դϴ�. ---

    #region Player_Data_Controls
    // ������ ��ư�� Ŭ������ �� ȣ��˴ϴ�.
    public void OnClickLevelUp()
    {
        UpdateAllUI();
    }

    // �����ٿ� ��ư�� Ŭ������ �� ȣ��˴ϴ�.
    public void OnClickLevelDown()
    {
       
    }

    // ���� �������� ��ư�� Ŭ������ �� ȣ��˴ϴ�.
    public void OnClickStageNext()
    {
        // ���������� 10�� �Ѿ�� �ʵ��� �����մϴ�.
        if (SaveLoadManager.instance.gameData.currentStage < 10)
        {
            SaveLoadManager.instance.gameData.currentStage++;
            UpdateAllUI();
        }
    }

    // ���� �������� ��ư�� Ŭ������ �� ȣ��˴ϴ�.
    public void OnClickStagePrev()
    {
        // ���������� 1���� �۾����� �ʵ��� �����մϴ�.
        if (SaveLoadManager.instance.gameData.currentStage > 1)
        {
            SaveLoadManager.instance.gameData.currentStage--;
            UpdateAllUI();
        }
    }
    #endregion

    #region Item_Controls
    // '������ �߰�' Ȯ�� ��ư�� Ŭ������ �� ȣ��˴ϴ�.
    public void OnClickConfirmAddItem()
    {
        string newItemName = itemInputField.text;

        // �Է�â�� ������� ���� ���� �������� �߰��մϴ�.
        if (!string.IsNullOrWhiteSpace(newItemName))
        {
            SaveLoadManager.instance.gameData.items.Add(newItemName);

            // ���� �Է��� ���� �Է�â�� �����ϰ� ���ϴ�.
            itemInputField.text = "";

            // �� �������� �ݿ��ǵ��� UI�� ���ΰ�ħ�մϴ�.
            UpdateAllUI();
        }
    }
    #endregion

    #region Scene_And_Save_Controls
    // '���� �޴���' ��ư�� Ŭ������ �� ȣ��˴ϴ�.
    public void OnClickReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // '�����ϱ�' ��ư�� Ŭ������ �� ȣ��˴ϴ�.
    private void UpdateSaveSlotsUI()
    {
        for (int i = 0; i < saveSlotInfoTexts.Length; i++)
        {
            int slotIndex = i + 1;
            GameData summaryData = SaveLoadManager.instance.LoadSaveSummary(slotIndex);

            if (summaryData != null)
            {
                // �����Ͱ� ������ ��� ������ ǥ���մϴ�.
                saveSlotInfoTexts[i].text = $"Player: {summaryData.nickname}\n" +
                                            $"Last Save: {summaryData.lastSaveTime}";
            }
            else
            {
                // �����Ͱ� ������ "�������"�� ǥ���մϴ�.
                saveSlotInfoTexts[i].text = "- Empty -";
            }
        }
    }
    public void OnClickOpenSavePanel()
    {
        UpdateSaveSlotsUI();
        saveSlotPanel.SetActive(true); // ���̺� �г��� ȭ�鿡 ǥ���մϴ�.
    }

    // ���̺� �г��� '�ݱ�' ��ư�� Ŭ������ �� ȣ��˴ϴ�.
    public void OnClickCloseSavePanel()
    {
        saveSlotPanel.SetActive(false); // ���̺� �г��� ����ϴ�.
    }

    // 1~5�� ���̺� ���� ��ư �� �ϳ��� Ŭ������ �� ȣ��˴ϴ�.
    // 1~5�� ���̺� ���� ��ư �� �ϳ��� Ŭ������ �� ȣ��˴ϴ�.
    public void OnClickSelectSaveSlot(int slotIndex)
    {
        // --- �Ʒ� Debug.Log �� �ٸ� �߰����ּ��� ---
        Debug.Log($"--- OnClickSelectSaveSlot called by '{this.gameObject.name}' with index: {slotIndex} ---");

        // ���� �ش� ���Կ� �̹� ������ �����Ѵٸ�,
        if (SaveLoadManager.instance.DoesSaveFileExist(slotIndex))
        {
            // (���� ���� �ڵ�� ����)
            this.selectedSlotIndex = slotIndex;
            confirmOverwriteText.text = $"Aleady data in SLOT {slotIndex}\nOverWrite?";
            confirmOverwritePanel.SetActive(true);
            saveSlotPanel.SetActive(false);
        }
        // ������ �������� �ʴ´ٸ�,
        else
        {
            Debug.Log($"Saving game to new slot {slotIndex}...");
            SaveLoadManager.instance.SaveGame(slotIndex);
            saveSlotPanel.SetActive(false);
        }
    }
    public void OnClickConfirmOverwriteYes()
    {
        // �ӽ� �����ص� ���� ��ȣ�� ������ �����մϴ�.
        Debug.Log($"Overwriting and saving game to slot {selectedSlotIndex}...");
        SaveLoadManager.instance.SaveGame(this.selectedSlotIndex);
        // Ȯ�� â�� �ݽ��ϴ�.
        confirmOverwritePanel.SetActive(false);
        // �ӽ� ���� ��ȣ�� �ʱ�ȭ�մϴ�.
        this.selectedSlotIndex = -1;
    }

    // ����� Ȯ�� â���� "�ƴϿ�"�� ������ �� ȣ��˴ϴ�.
    public void OnClickConfirmOverwriteNo()
    {
        // Ȯ�� â�� �ݰ�,
        confirmOverwritePanel.SetActive(false);
        this.selectedSlotIndex = -1;
        // �ٽ� ���� ���� â�� �����ݴϴ�. (����� ���Ǽ�)
        saveSlotPanel.SetActive(true);
    }
    
    #endregion
}