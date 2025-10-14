// 파일 이름: CharacterSelectUI.cs (새 파일)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class CharacterSelectUI : MonoBehaviour
{
    [Tooltip("인스펙터에서 3개의 캐릭터 데이터 에셋을 연결")]
    public List<CharacterData> characterDatas;

    [Header("UI 연결")]
    public Button[] characterButtons;
    public TextMeshProUGUI descriptionText;
    public Button confirmButton;

    private int selectedIndex = -1;

    void Start()
    {
        // 버튼에 기능 연결
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i; // 중요: 클로저 문제 방지
            characterButtons[i].onClick.AddListener(() => OnClickCharacterButton(index));

            // 버튼 이미지 설정
            if (i < characterDatas.Count && characterDatas[i].characterIcon != null)
            {
                characterButtons[i].image.sprite = characterDatas[i].characterIcon;
            }
        }
        confirmButton.onClick.AddListener(StartGame);

        // 처음에는 아무것도 선택되지 않은 상태로 시작
        OnClickCharacterButton(0); // 또는 첫번째 캐릭터를 기본 선택
    }

    private void OnClickCharacterButton(int index)
    {
        selectedIndex = index;
        descriptionText.text = $"{characterDatas[index].characterName}\n\n{characterDatas[index].characterDescription}";

        // (선택사항) 선택된 버튼을 시각적으로 강조하는 효과 추가
    }

    private void StartGame()
    {
        if (selectedIndex != -1)
        {
            // GameSession에 선택된 캐릭터 정보를 저장
            GameSession.instance.selectedCharacter = characterDatas[selectedIndex];
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.LogWarning("캐릭터를 선택해주세요!");
        }
    }
}