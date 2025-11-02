using UnityEngine;
using UnityEngine.UI; // Unity UI 요소를 사용하기 위해 필요합니다.
using TMPro; // TextMeshPro를 사용하기 위해 필요합니다.
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요합니다.

public class NicknameSetupUI : MonoBehaviour
{
    // Unity 에디터에서 연결할 UI 요소들
    public TMP_InputField nicknameInputField; // 닉네임 입력창
    public Button confirmButton; // 확인 버튼

    void Start()
    {
        // 버튼에 클릭 이벤트 리스너 추가
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
    }

    // 확인 버튼이 클릭되었을 때 호출될 함수
    //파일 명: NicknameSetupUI.cs (OnConfirmButtonClick 함수 수정)

    private void OnConfirmButtonClick()
    {
        string nickname = nicknameInputField.text;

        if (string.IsNullOrWhiteSpace(nickname))
        {
            Debug.LogWarning("Insert your name");
            return;
        }

        SaveLoadManager.instance.gameData.nickname = nickname;

        // (기본 캐릭터 설정 로직은 그대로 유지)
        if (string.IsNullOrEmpty(SaveLoadManager.instance.gameData.currentSelectedCharacterID))
        {
            SaveLoadManager.instance.gameData.currentSelectedCharacterID = "Char_Boom";
            if (!SaveLoadManager.instance.gameData.characterLevels.ContainsKey("Char_Boom"))
            {
                SaveLoadManager.instance.gameData.characterLevels.Add("Char_Boom", 1);
            }
            Debug.Log("NicknameSetupUI: 기본 캐릭터(Char_Boom)로 설정 완료");
        }

        SaveLoadManager.instance.SaveGame(GameSession.instance.currentSaveSlot);

        // --- ( 수정된 부분) ---
        // 닉네임 저장 후, 재생할 컷신이 있는지(MainMenu에서 설정) 확인합니다.
        if (GameSession.instance.isNewGameCutscene && GameSession.instance.cutsceneToPlay != null)
        {
            //  첫 컷신이 있다면 "CutScene" 씬을 로드합니다.
            SceneManager.LoadScene("CutScene");
        }
        else
        {
            //  첫 컷신이 설정되어 있지 않다면, 바로 "StageSelect" 씬으로 갑니다.
            SceneManager.LoadScene("StageSelect");
        }
        // --- ( 수정 끝) ---
    }
}