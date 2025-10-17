// 파일 이름: CharacterSelectUI.cs (LockOverlay 처리 추가)
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("캐릭터 목록 UI")]
    public Transform iconContainer;
    public GameObject characterButtonPrefab;

    [Header("캐릭터 정보창 UI")]
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

    [Header("페이지 닫기 버튼")]
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

        // GachaManager에 등록된 모든 캐릭터를 가져와 정렬
        List<CharacterData> allCharacters = GachaManager.instance.allCharacters
            .Where(c => c != null)
            .OrderBy(c => c.characterID)
            .ToList();

        foreach (CharacterData charData in allCharacters)
        {
            GameObject buttonGO = Instantiate(characterButtonPrefab, iconContainer);

            // --- Icon 이미지 설정 ---
            Image iconImage = buttonGO.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = charData.characterIcon;
                iconImage.color = Color.white;
            }
            else
            {
                Debug.LogError("CharacterButtonPrefab에 'Icon' 자식 오브젝트가 없습니다!");
            }

            // --- LockOverlay 및 버튼 상호작용 설정 ---
            GameObject lockOverlay = buttonGO.transform.Find("LockOverlay")?.gameObject;
            Button button = buttonGO.GetComponent<Button>();

            // 보유 여부 확인 (레벨 1 이상)
            bool isOwned = data.characterLevels.ContainsKey(charData.characterID) && data.characterLevels[charData.characterID] > 0;

            if (isOwned)
            {
                // 보유한 캐릭터일 경우
                if (lockOverlay != null)
                {
                    lockOverlay.SetActive(false); // 잠금 오버레이 비활성화
                }
                button.interactable = true; // 버튼 클릭 가능
                button.onClick.AddListener(() => ShowCharacterInfo(charData));
            }
            else
            {
                // 미보유 캐릭터일 경우
                if (lockOverlay != null)
                {
                    lockOverlay.SetActive(true); // 잠금 오버레이 활성화
                }
                button.interactable = false; // 버튼 클릭 불가능
                // 미보유 캐릭터 아이콘을 약간 어둡게 처리 (선택 사항)
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

        Debug.Log($"{selectedCharacterForInfo.characterName}(으)로 캐릭터 선택됨.");

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