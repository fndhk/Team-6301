// ���� �̸�: CharacterSelectUI.cs (���� ��ġ ����)
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// ���� ��ũ��Ʈ ��ܿ� �� Ŭ������ �߰��մϴ� ����
[System.Serializable]
public class CharacterButtonSetup
{
    [Tooltip("�� ��ư�� ��ǥ�� ĳ���� �����͸� �����ϼ���.")]
    public CharacterData characterData;
    [Tooltip("���� ��ġ�� ĳ���� ��ư UI�� �����ϼ���.")]
    public Button characterButton;
    [Tooltip("��ư�� �ڽ� ������Ʈ�� 'Icon' �̹����� �����ϼ���.")]
    public Image iconImage;
    [Tooltip("��ư�� �ڽ� ������Ʈ�� 'LockOverlay'�� �����ϼ���.")]
    public GameObject lockOverlay;
}


public class CharacterSelectUI : MonoBehaviour
{
    // ���� ���� �������� �Ʒ� ����Ʈ�� ��ü�մϴ� ����
    [Header("ĳ���� ��ư ���� ����")]
    [Tooltip("���� �������� ��ġ�� ĳ���� ��ư���� ���⿡ ����ϼ���.")]
    public List<CharacterButtonSetup> characterButtons = new List<CharacterButtonSetup>();


    [Header("ĳ���� ����â UI")]
    public GameObject infoPanel;
    public Image characterIllustrationImage;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillDescriptionText;
    public Button selectButton;
    public Button closeInfoButton;

    [Header("������ �ݱ� ��ư")]
    public Button closePageButton;

    // private List<GameObject> spawnedButtons = new List<GameObject>(); // �� ���� �� �̻� �ʿ� �����Ƿ� �����մϴ�.
    private CharacterData selectedCharacterForInfo;

    void OnEnable()
    {
        infoPanel.SetActive(false);
        closePageButton.onClick.RemoveAllListeners();
        closePageButton.onClick.AddListener(CloseCharacterSelectPage);
        // ���� �Լ� �̸��� �����մϴ� ���"
        SetupManualCharacterButtons();
    }

    // ���� GenerateCharacterButtons �Լ��� �Ʒ� �������� ��ü ��ü�մϴ� ����
    void SetupManualCharacterButtons()
    {
        if (SaveLoadManager.instance == null || SaveLoadManager.instance.gameData == null)
        {
            Debug.LogError("CharacterSelectUI: SaveLoadManager�� �����Ͱ� �����ϴ�!");
            return;
        }

        GameData data = SaveLoadManager.instance.gameData;

        // ��ϵ� ��� ��ư�� ��ȸ�ϸ� ���¸� �����մϴ�.
        foreach (var buttonSetup in characterButtons)
        {
            // �ʼ� ��Ұ� ������� �ʾ����� �ǳʶݴϴ�.
            if (buttonSetup.characterData == null || buttonSetup.characterButton == null || buttonSetup.iconImage == null || buttonSetup.lockOverlay == null)
            {
                Debug.LogWarning("CharacterButtons ����Ʈ�� �Ϻ� �׸��� ����� �������� �ʾҽ��ϴ�. Inspector�� Ȯ�����ּ���.");
                continue;
            }

            CharacterData charData = buttonSetup.characterData;

            // ������ �̹��� ����
            buttonSetup.iconImage.sprite = charData.characterIcon;

            // ������ �ʱ�ȭ (OnEnable���� �ߺ� �߰� ����)
            buttonSetup.characterButton.onClick.RemoveAllListeners();

            // ���� ���� Ȯ�� (���� 1 �̻�)
            bool isOwned = data.characterLevels.ContainsKey(charData.characterID) && data.characterLevels[charData.characterID] > 0;

            if (isOwned)
            {
                // ������ ĳ������ ���
                buttonSetup.lockOverlay.SetActive(false);
                buttonSetup.characterButton.interactable = true;
                buttonSetup.iconImage.color = Color.white; // ���� ��������
                // ��ư Ŭ�� �� ShowCharacterInfo �Լ��� �ش� ĳ���� �����Ϳ� �Բ� ȣ���ϵ��� ����
                buttonSetup.characterButton.onClick.AddListener(() => ShowCharacterInfo(charData));
            }
            else
            {
                // �̺��� ĳ������ ���
                buttonSetup.lockOverlay.SetActive(true);
                buttonSetup.characterButton.interactable = false;
                buttonSetup.iconImage.color = new Color(0.5f, 0.5f, 0.5f, 0.8f); // ��Ӱ� ó��
            }
        }
    }


    void ShowCharacterInfo(CharacterData data)
    {
        selectedCharacterForInfo = data;
        infoPanel.SetActive(true);

        characterIllustrationImage.sprite = data.characterIllustration;
        characterNameText.text = data.characterName;
        rarityText.text = data.rarity.ToString();
        // ���� ĳ���� ������ ǥ���� ���� ���� ���θ� �� �� �� Ȯ���ϴ� ���� �����մϴ�. ����
        if (SaveLoadManager.instance.gameData.characterLevels.ContainsKey(data.characterID))
        {
            levelText.text = "Lv. " + SaveLoadManager.instance.gameData.characterLevels[data.characterID];
        }
        else
        {
            levelText.text = "Lv. 1 (�̺���)";
        }

        descriptionText.text = data.characterDescription;
        skillNameText.text = data.skillName;
        skillDescriptionText.text = data.skillDescription;

        selectButton.onClick.RemoveAllListeners();
        closeInfoButton.onClick.RemoveAllListeners();

        selectButton.onClick.AddListener(OnClickSelect);
        closeInfoButton.onClick.AddListener(() => infoPanel.SetActive(false));

        if (SaveLoadManager.instance.gameData.currentSelectedCharacterID == data.characterID)
        {
            selectButton.interactable = false;
        }
        else
        {
            selectButton.interactable = true;
        }
    }

    void OnClickSelect()
    {
        if (selectedCharacterForInfo == null) return;

        SaveLoadManager.instance.gameData.currentSelectedCharacterID = selectedCharacterForInfo.characterID;
        GameSession.instance.selectedCharacter = selectedCharacterForInfo;

        SaveLoadManager.instance.SaveGame(GameSession.instance.currentSaveSlot);

        Debug.Log($"{selectedCharacterForInfo.characterName}(��)�� ĳ���� ���õ�.");

        CloseCharacterSelectPage();
    }

    void CloseCharacterSelectPage()
    {
        StageSelectManager ssm = FindFirstObjectByType<StageSelectManager>();
        if (ssm != null)
        {
            ssm.UpdateCurrentCharacterUI();
        }

        gameObject.SetActive(false);
    }
}