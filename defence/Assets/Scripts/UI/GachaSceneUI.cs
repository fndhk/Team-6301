//파일 명: GachaSceneUI.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GachaSceneUI : MonoBehaviour
{
    [Header("UI 요소")]
    public Button gachaButton;
    public TextMeshProUGUI ticketCountText;
    public Image resultCharacterImage;
    public TextMeshProUGUI resultNameText;
    public GameObject resultPanel;

    [Header("버튼들 - Inspector에서 연결")]
    public Button closeButton;
    public Button backButton;

    private CharacterData pendingResult;

    void Start()
    {
        if (SaveLoadManager.instance == null)
        {
            Debug.LogError("GachaSceneUI: SaveLoadManager가 없습니다! MainMenu에서 시작해주세요.");
            return;
        }

        if (GameSession.instance == null)
        {
            Debug.LogError("GachaSceneUI: GameSession이 없습니다! MainMenu에서 시작해주세요.");
            return;
        }

        UpdateTicketUI();

        if (gachaButton != null)
        {
            gachaButton.onClick.RemoveAllListeners();
            gachaButton.onClick.AddListener(OnClickGacha);
            Debug.Log("GachaButton 연결 완료");
        }
        else
        {
            Debug.LogError("GachaButton이 null입니다!");
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnClickCloseResult);
            Debug.Log("CloseButton 연결 완료");
        }
        else
        {
            Debug.LogWarning("CloseButton이 연결되지 않았습니다.");
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnClickBackButton);
            Debug.Log("BackButton 연결 완료");
        }
        else
        {
            Debug.LogWarning("BackButton이 연결되지 않았습니다.");
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
            Debug.LogError("GachaSceneUI: SaveLoadManager 또는 GameData가 null입니다!");
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
        Debug.Log("=== OnClickGacha 호출됨 ===");

        if (SaveLoadManager.instance == null || SaveLoadManager.instance.gameData == null)
        {
            Debug.LogError("GachaSceneUI: SaveLoadManager가 없습니다!");
            return;
        }

        if (GachaManager.instance == null)
        {
            Debug.LogError("GachaSceneUI: GachaManager가 없습니다!");
            return;
        }

        GameData data = SaveLoadManager.instance.gameData;
        if (data.gachaTickets <= 0)
        {
            Debug.Log("티켓이 부족합니다!");
            return;
        }

        data.gachaTickets--;
        CharacterData result = GachaManager.instance.DrawCharacter();

        if (result == null)
        {
            Debug.LogError("GachaSceneUI: 뽑기 결과가 null입니다!");
            UpdateTicketUI();
            return;
        }

        Debug.Log($"뽑기 결과: {result.characterName} ({result.rarity})");

        GachaManager.instance.ProcessGacha(result);
        pendingResult = result;

        if (GachaVideoPlayer.instance != null)
        {
            GachaVideoPlayer.instance.PlayGachaVideo(result.rarity, ShowResult);
        }
        else
        {
            Debug.LogWarning("GachaSceneUI: GachaVideoPlayer가 없어서 동영상을 건너뜁니다.");
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
        Debug.Log("=== ShowResult 호출됨 ===");

        if (pendingResult == null)
        {
            Debug.LogWarning("pendingResult가 null입니다!");
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
        Debug.Log("=== OnClickCloseResult 호출됨 ===");
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    public void OnClickBackButton()
    {
        Debug.Log("=== OnClickBackButton 호출됨 ===");
        SceneManager.LoadScene("StageSelect");
    }
}