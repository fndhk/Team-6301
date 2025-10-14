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
    private void OnConfirmButtonClick()
    {
        string nickname = nicknameInputField.text;

        // 닉네임이 비어있거나 공백만 있는지 확인
        if (string.IsNullOrWhiteSpace(nickname))
        {
            Debug.LogWarning("Insert your name");
            // 여기에 사용자에게 알림을 주는 UI를 추가할 수도 있습니다.
            return; // 함수 종료
        }

        // SaveLoadManager를 통해 현재 게임 데이터에 닉네임 저장
        SaveLoadManager.instance.gameData.nickname = nickname;

        // 데이터가 준비되었으니 InGame 씬으로 이동
        //SceneManager.LoadScene("InGame");
        SceneManager.LoadScene("StageSelect");
    }
}