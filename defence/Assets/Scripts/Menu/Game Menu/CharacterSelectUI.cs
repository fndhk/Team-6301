// 파일 이름: CharacterSelectUI.cs (수동 배치 버전)
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// ▼▼▼ 스크립트 상단에 이 클래스를 추가합니다 ▼▼▼
[System.Serializable]
public class CharacterButtonSetup
{
    [Tooltip("이 버튼이 대표할 캐릭터 데이터를 연결하세요.")]
    public CharacterData characterData;
    [Tooltip("씬에 배치된 캐릭터 버튼 UI를 연결하세요.")]
    public Button characterButton;
    [Tooltip("버튼의 자식 오브젝트인 'Icon' 이미지를 연결하세요.")]
    public Image iconImage;
    [Tooltip("버튼의 자식 오브젝트인 'LockOverlay'를 연결하세요.")]
    public GameObject lockOverlay;
}


public class CharacterSelectUI : MonoBehaviour
{
    // ▼▼▼ 기존 변수들을 아래 리스트로 대체합니다 ▼▼▼
    [Header("캐릭터 버튼 수동 설정")]
    [Tooltip("씬에 수동으로 배치한 캐릭터 버튼들을 여기에 등록하세요.")]
    public List<CharacterButtonSetup> characterButtons = new List<CharacterButtonSetup>();


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

    // private List<GameObject> spawnedButtons = new List<GameObject>(); // 이 줄은 더 이상 필요 없으므로 삭제합니다.
    private CharacterData selectedCharacterForInfo;

    void OnEnable()
    {
        infoPanel.SetActive(false);
        closePageButton.onClick.RemoveAllListeners();
        closePageButton.onClick.AddListener(CloseCharacterSelectPage);
        // ▼▼▼ 함수 이름을 변경합니다 ▼▼"
        SetupManualCharacterButtons();
    }

    // ▼▼▼ GenerateCharacterButtons 함수를 아래 내용으로 전체 교체합니다 ▼▼▼
    void SetupManualCharacterButtons()
    {
        if (SaveLoadManager.instance == null || SaveLoadManager.instance.gameData == null)
        {
            Debug.LogError("CharacterSelectUI: SaveLoadManager의 데이터가 없습니다!");
            return;
        }

        GameData data = SaveLoadManager.instance.gameData;

        // 등록된 모든 버튼을 순회하며 상태를 설정합니다.
        foreach (var buttonSetup in characterButtons)
        {
            // 필수 요소가 연결되지 않았으면 건너뜁니다.
            if (buttonSetup.characterData == null || buttonSetup.characterButton == null || buttonSetup.iconImage == null || buttonSetup.lockOverlay == null)
            {
                Debug.LogWarning("CharacterButtons 리스트의 일부 항목이 제대로 설정되지 않았습니다. Inspector를 확인해주세요.");
                continue;
            }

            CharacterData charData = buttonSetup.characterData;

            // 아이콘 이미지 설정
            buttonSetup.iconImage.sprite = charData.characterIcon;

            // 리스너 초기화 (OnEnable마다 중복 추가 방지)
            buttonSetup.characterButton.onClick.RemoveAllListeners();

            // 보유 여부 확인 (레벨 1 이상)
            bool isOwned = data.characterLevels.ContainsKey(charData.characterID) && data.characterLevels[charData.characterID] > 0;

            if (isOwned)
            {
                // 보유한 캐릭터일 경우
                buttonSetup.lockOverlay.SetActive(false);
                buttonSetup.characterButton.interactable = true;
                buttonSetup.iconImage.color = Color.white; // 원래 색상으로
                // 버튼 클릭 시 ShowCharacterInfo 함수를 해당 캐릭터 데이터와 함께 호출하도록 설정
                buttonSetup.characterButton.onClick.AddListener(() => ShowCharacterInfo(charData));
            }
            else
            {
                // 미보유 캐릭터일 경우
                buttonSetup.lockOverlay.SetActive(true);
                buttonSetup.characterButton.interactable = false;
                buttonSetup.iconImage.color = new Color(0.5f, 0.5f, 0.5f, 0.8f); // 어둡게 처리
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
        // ▼▼▼ 캐릭터 레벨을 표시할 때도 보유 여부를 한 번 더 확인하는 것이 안전합니다. ▼▼▼
        if (SaveLoadManager.instance.gameData.characterLevels.ContainsKey(data.characterID))
        {
            levelText.text = "Lv. " + SaveLoadManager.instance.gameData.characterLevels[data.characterID];
        }
        else
        {
            levelText.text = "Lv. 1 (미보유)";
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