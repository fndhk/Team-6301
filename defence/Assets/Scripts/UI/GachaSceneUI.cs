//���� ��: GachaSceneUI.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GachaSceneUI : MonoBehaviour
{
    [Header("UI ���")]
    public Button gachaButton;
    public TextMeshProUGUI ticketCountText;
    public Image resultCharacterImage;
    public TextMeshProUGUI resultNameText;
    public GameObject resultPanel;

    [Header("��ư�� - Inspector���� ����")]
    public Button closeButton;
    public Button backButton;

    private CharacterData pendingResult;

    void Start()
    {
        if (SaveLoadManager.instance == null)
        {
            Debug.LogError("GachaSceneUI: SaveLoadManager�� �����ϴ�! MainMenu���� �������ּ���.");
            return;
        }

        if (GameSession.instance == null)
        {
            Debug.LogError("GachaSceneUI: GameSession�� �����ϴ�! MainMenu���� �������ּ���.");
            return;
        }

        UpdateTicketUI();

        if (gachaButton != null)
        {
            gachaButton.onClick.RemoveAllListeners();
            gachaButton.onClick.AddListener(OnClickGacha);
            Debug.Log("GachaButton ���� �Ϸ�");
        }
        else
        {
            Debug.LogError("GachaButton�� null�Դϴ�!");
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnClickCloseResult);
            Debug.Log("CloseButton ���� �Ϸ�");
        }
        else
        {
            Debug.LogWarning("CloseButton�� ������� �ʾҽ��ϴ�.");
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnClickBackButton);
            Debug.Log("BackButton ���� �Ϸ�");
        }
        else
        {
            Debug.LogWarning("BackButton�� ������� �ʾҽ��ϴ�.");
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    void UpdateTicketUI()
    {
        if (SaveLoadManager.instance == null || SaveLoadManager.instance.gameData == null)
        {
            Debug.LogError("GachaSceneUI: SaveLoadManager �Ǵ� GameData�� null�Դϴ�!");
            if (ticketCountText != null)
            {
                ticketCountText.text = "Tickets: ???";
            }
            if (gachaButton != null)
            {
                gachaButton.interactable = false;
            }
            return;
        }

        int tickets = SaveLoadManager.instance.gameData.gachaTickets;
        ticketCountText.text = $"Tickets: {tickets}";
        gachaButton.interactable = tickets > 0;
    }

    void OnClickGacha()
    {
        Debug.Log("=== OnClickGacha ȣ��� ===");

        if (SaveLoadManager.instance == null || SaveLoadManager.instance.gameData == null)
        {
            Debug.LogError("GachaSceneUI: SaveLoadManager�� �����ϴ�!");
            return;
        }

        if (GachaManager.instance == null)
        {
            Debug.LogError("GachaSceneUI: GachaManager�� �����ϴ�!");
            return;
        }

        GameData data = SaveLoadManager.instance.gameData;
        if (data.gachaTickets <= 0)
        {
            Debug.Log("Ƽ���� �����մϴ�!");
            return;
        }

        data.gachaTickets--;
        CharacterData result = GachaManager.instance.DrawCharacter();

        if (result == null)
        {
            Debug.LogError("GachaSceneUI: �̱� ����� null�Դϴ�!");
            UpdateTicketUI();
            return;
        }

        Debug.Log($"�̱� ���: {result.characterName} ({result.rarity})");

        GachaManager.instance.ProcessGacha(result);
        pendingResult = result;

        if (GachaVideoPlayer.instance != null)
        {
            GachaVideoPlayer.instance.PlayGachaVideo(result.rarity, ShowResult);
        }
        else
        {
            Debug.LogWarning("GachaSceneUI: GachaVideoPlayer�� ��� �������� �ǳʶݴϴ�.");
            ShowResult();
        }

        UpdateTicketUI();

        if (GameSession.instance != null)
        {
            SaveLoadManager.instance.SaveGame(GameSession.instance.currentSaveSlot);
        }
    }

    void ShowResult()
    {
        Debug.Log("=== ShowResult ȣ��� ===");

        if (pendingResult == null)
        {
            Debug.LogWarning("pendingResult�� null�Դϴ�!");
            return;
        }

        resultPanel.SetActive(true);
        resultCharacterImage.sprite = pendingResult.characterIcon;
        resultNameText.text = $"{pendingResult.characterName}\n({pendingResult.rarity})";

        if (SaveLoadManager.instance != null &&
            SaveLoadManager.instance.gameData != null &&
            SaveLoadManager.instance.gameData.characterLevels.ContainsKey(pendingResult.characterID))
        {
            int currentLevel = SaveLoadManager.instance.gameData.characterLevels[pendingResult.characterID];
            if (currentLevel > 1)
            {
                resultNameText.text += $"\nLv.{currentLevel}";
            }
        }

        pendingResult = null;
    }

    public void OnClickCloseResult()
    {
        Debug.Log("=== OnClickCloseResult ȣ��� ===");
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    public void OnClickBackButton()
    {
        Debug.Log("=== OnClickBackButton ȣ��� ===");
        SceneManager.LoadScene("StageSelect");
    }
}