// 파일 이름: GachaResultSlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GachaResultSlotUI : MonoBehaviour
{
    [Header("UI 연결 (수동 배치용)")]
    public Image characterImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statusText;

    // 결과를 표시하는 함수
    public void ShowResult(CharacterData data, string status)
    {
        if (data != null)
        {
            characterImage.sprite = data.characterIllustration;
            characterImage.color = Color.white;
            nameText.text = $"{data.characterName} ({data.rarity})";
            statusText.text = status;
        }
    }

    // 결과를 숨기고 검은색으로 만드는 함수
    public void Hide()
    {
        characterImage.sprite = null;
        characterImage.color = Color.black;
        nameText.text = "";
        statusText.text = "";
    }
}