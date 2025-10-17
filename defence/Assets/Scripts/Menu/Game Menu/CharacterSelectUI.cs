// ���� �̸�: CharacterSelectUI.cs (LockOverlay ó�� �߰�)
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("ĳ���� ��� UI")]
    public Transform iconContainer;
    public GameObject characterButtonPrefab;

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

    private List<GameObject> spawnedButtons = new List<GameObject>();
    private CharacterData selectedCharacterForInfo;

    void OnEnable()
    {
        infoPanel.SetActive(false);
        closePageButton.onClick.RemoveAllListeners();
        closePageButton.onClick.AddListener(CloseCharacterSelectPage);
        GenerateCharacterButtons();
    }

    void GenerateCharacterButtons()
    {
        foreach (var btn in spawnedButtons)
        {
            Destroy(btn);
        }
        spawnedButtons.Clear();

        GameData data = SaveLoadManager.instance.gameData;

        // GachaManager�� ��ϵ� ��� ĳ���͸� ������ ����
        List<CharacterData> allCharacters = GachaManager.instance.allCharacters
            .Where(c => c != null)
            .OrderBy(c => c.characterID)
            .ToList();

        foreach (CharacterData charData in allCharacters)
        {
            GameObject buttonGO = Instantiate(characterButtonPrefab, iconContainer);

            // --- Icon �̹��� ���� ---
            Image iconImage = buttonGO.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = charData.characterIcon;
                iconImage.color = Color.white;
            }
            else
            {
                Debug.LogError("CharacterButtonPrefab�� 'Icon' �ڽ� ������Ʈ�� �����ϴ�!");
            }

            // --- LockOverlay �� ��ư ��ȣ�ۿ� ���� ---
            GameObject lockOverlay = buttonGO.transform.Find("LockOverlay")?.gameObject;
            Button button = buttonGO.GetComponent<Button>();

            // ���� ���� Ȯ�� (���� 1 �̻�)
            bool isOwned = data.characterLevels.ContainsKey(charData.characterID) && data.characterLevels[charData.characterID] > 0;

            if (isOwned)
            {
                // ������ ĳ������ ���
                if (lockOverlay != null)
                {
                    lockOverlay.SetActive(false); // ��� �������� ��Ȱ��ȭ
                }
                button.interactable = true; // ��ư Ŭ�� ����
                button.onClick.AddListener(() => ShowCharacterInfo(charData));
            }
            else
            {
                // �̺��� ĳ������ ���
                if (lockOverlay != null)
                {
                    lockOverlay.SetActive(true); // ��� �������� Ȱ��ȭ
                }
                button.interactable = false; // ��ư Ŭ�� �Ұ���
                // �̺��� ĳ���� �������� �ణ ��Ӱ� ó�� (���� ����)
                if (iconImage != null) iconImage.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            }

            spawnedButtons.Add(buttonGO);
        }
    }

    void ShowCharacterInfo(CharacterData data)
    {
        selectedCharacterForInfo = data;
        infoPanel.SetActive(true);

        characterIllustrationImage.sprite = data.characterIllustration;
        characterNameText.text = data.characterName;
        rarityText.text = data.rarity.ToString();
        levelText.text = "Lv. " + SaveLoadManager.instance.gameData.characterLevels[data.characterID];
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