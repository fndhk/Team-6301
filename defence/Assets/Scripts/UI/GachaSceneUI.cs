// 파일 이름: GachaSceneUI.cs (전체 수정 버전)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic; // List 사용을 위해 추가
using System.Linq; // Mathf.Min 사용을 위해 추가

// 뽑기 결과와 상태를 함께 저장하기 위한 작은 구조체
public struct GachaDrawResult
{
    public CharacterData characterData;
    public string status;

    public GachaDrawResult(CharacterData data, string stat)
    {
        characterData = data;
        status = stat;
    }
}

public class GachaSceneUI : MonoBehaviour
{
    [Header("UI 요소 - 단일 뽑기")]
    public Button gachaButton;
    public TextMeshProUGUI ticketCountText;
    public Image resultCharacterImage;
    public TextMeshProUGUI resultNameText;
    public TextMeshProUGUI resultStatusText; //  "신규 단원" 등 상태 표시용
    public GameObject resultPanel;
    public Button closeButton;

    [Header("UI 요소 - 단체 뽑기")]
    public Button gachaAllButton; //  티켓 모두 사용 버튼
    public GameObject multiResultPanel; //  단체 결과 패널
    public Button closeMultiResultButton; //  단체 결과 패널 닫기 버튼
    public List<GachaResultSlotUI> multiResultSlots; //  단체 결과 슬롯 UI 리스트

    [Header("기타 UI")]
    public Button backButton;

    private GachaDrawResult pendingSingleResult; //  단일 뽑기 결과를 구조체로 저장

    void Start()
    {
        // ... (기존 Start 내용은 대부분 동일) ...
        if (SaveLoadManager.instance == null || GameSession.instance == null)
        {
            Debug.LogError("GachaSceneUI: 필수 매니저가 없습니다! MainMenu에서 시작해주세요.");
            return;
        }

        UpdateTicketUI();

        // 버튼 리스너 연결
        gachaButton.onClick.AddListener(OnClickGachaSingle);
        closeButton.onClick.AddListener(OnClickCloseResult);
        backButton.onClick.AddListener(OnClickBackButton);

        //  새로 추가된 버튼 리스너 연결
        gachaAllButton.onClick.AddListener(OnClickGachaAll);
        closeMultiResultButton.onClick.AddListener(OnClickCloseMultiResult);

        // 패널 초기 상태 설정
        resultPanel.SetActive(false);
        multiResultPanel.SetActive(false);
    }

    void UpdateTicketUI()
    {
        int tickets = SaveLoadManager.instance.gameData.gachaTickets;
        ticketCountText.text = $"Tickets: {tickets}";

        // 티켓 수에 따라 버튼 활성화/비활성화
        gachaButton.interactable = tickets > 0;
        gachaAllButton.interactable = tickets > 0;
    }

    // 1회 뽑기 버튼 클릭
    void OnClickGachaSingle()
    {
        if (SaveLoadManager.instance.gameData.gachaTickets <= 0) return;

        SaveLoadManager.instance.gameData.gachaTickets--;
        CharacterData result = GachaManager.instance.DrawCharacter();
        if (result == null) return;

        //  뽑기 전에 현재 상태를 확인
        string status = GetGachaStatus(result.characterID);

        //  뽑기 결과 처리 (레벨업 등)
        GachaManager.instance.ProcessGacha(result);

        //  결과와 상태를 함께 저장
        pendingSingleResult = new GachaDrawResult(result, status);

        // 동영상 재생 후 ShowResult 호출
        GachaVideoPlayer.instance.PlayGachaVideo(result.rarity, ShowSingleResult);

        UpdateTicketUI();
        SaveLoadManager.instance.SaveGame(GameSession.instance.currentSaveSlot);
    }

    //  모두 뽑기 버튼 클릭
    void OnClickGachaAll()
    {
        int tickets = SaveLoadManager.instance.gameData.gachaTickets;
        if (tickets <= 0) return;

        // 사용할 티켓 수 결정 (최대 10장)
        int drawCount = Mathf.Min(tickets, 10);

        // 티켓 소모
        SaveLoadManager.instance.gameData.gachaTickets -= drawCount;

        List<GachaDrawResult> results = new List<GachaDrawResult>();

        // 정해진 횟수만큼 뽑기 반복
        for (int i = 0; i < drawCount; i++)
        {
            CharacterData drawnChar = GachaManager.instance.DrawCharacter();
            if (drawnChar == null) continue;

            string status = GetGachaStatus(drawnChar.characterID);
            GachaManager.instance.ProcessGacha(drawnChar);
            results.Add(new GachaDrawResult(drawnChar, status));
        }

        // 결과 표시 코루틴 시작
        StartCoroutine(ShowMultiResultCoroutine(results));

        UpdateTicketUI();
        SaveLoadManager.instance.SaveGame(GameSession.instance.currentSaveSlot);
    }

    //  단일 뽑기 결과 표시 (콜백 함수)
    void ShowSingleResult()
    {
        resultPanel.SetActive(true);
        //  아이콘 대신 일러스트 표시
        resultCharacterImage.sprite = pendingSingleResult.characterData.characterIllustration;
        resultNameText.text = $"{pendingSingleResult.characterData.characterName}\n({pendingSingleResult.characterData.rarity})";
        //  상태 텍스트 표시
        resultStatusText.text = pendingSingleResult.status;

        // 레벨 표시 로직 (기존과 동일)
        int currentLevel = SaveLoadManager.instance.gameData.characterLevels[pendingSingleResult.characterData.characterID];
        if (currentLevel > 0)
        {
            resultNameText.text += $"\nLv.{currentLevel}";
        }
    }

    //  단체 뽑기 결과 표시 (코루틴)
    private IEnumerator<WaitForSeconds> ShowMultiResultCoroutine(List<GachaDrawResult> results)
    {
        multiResultPanel.SetActive(true);

        // 모든 슬롯을 일단 숨깁니다.
        foreach (var slot in multiResultSlots)
        {
            slot.gameObject.SetActive(false);
        }

        // 뽑은 개수만큼 슬롯을 활성화하고 '숨김' 상태로 만듭니다.
        for (int i = 0; i < results.Count; i++)
        {
            if (i < multiResultSlots.Count)
            {
                multiResultSlots[i].gameObject.SetActive(true);
                multiResultSlots[i].Hide(); // 이미지를 검게, 텍스트는 비움
            }
        }

        // (선택사항) 여기에 카드 뒤집는 애니메이션 같은 연출을 넣을 수 있습니다.
        yield return new WaitForSeconds(0.5f); // 잠깐 대기 후 결과 공개

        // 결과를 하나씩 공개합니다.
        for (int i = 0; i < results.Count; i++)
        {
            if (i < multiResultSlots.Count)
            {
                multiResultSlots[i].ShowResult(results[i].characterData, results[i].status);
                yield return new WaitForSeconds(0.3f); // 다음 카드를 공개하기까지의 시간
            }
        }
    }

    //  캐릭터 ID를 기반으로 현재 상태를 반환하는 함수
    private string GetGachaStatus(string charID)
    {
        var levels = SaveLoadManager.instance.gameData.characterLevels;

        if (!levels.ContainsKey(charID))
        {
            return "신규 단원";
        }
        else if (levels[charID] < 3)
        {
            return "단원 강화";
        }
        else // 3레벨 이상
        {
            return "소지금 +300";
        }
    }

    // --- UI 닫기 및 뒤로가기 함수들 ---
    public void OnClickCloseResult()
    {
        resultPanel.SetActive(false);
    }

    public void OnClickCloseMultiResult()
    {
        multiResultPanel.SetActive(false);
    }

    public void OnClickBackButton()
    {
        SceneManager.LoadScene("StageSelect");
    }
}