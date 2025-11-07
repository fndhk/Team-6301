using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionMenuController : MonoBehaviour
{
    [Header("타이틀 연결")]
    public TextMeshProUGUI titleText;

    [Header("패널 연결")]
    public GameObject panelDisplay;
    public GameObject panelSound;
    public GameObject panelKey;

    // 시작할 때 화면설정 패널만 보여주고 나머지는 숨김
    void Start()
    {
        ShowDisplaySettings();
    }

    // 모든 패널을 일단 꺼버리는 함수 (중복 방지용)
    private void HideAllPanels()
    {
        panelDisplay.SetActive(false);
        panelSound.SetActive(false);
        panelKey.SetActive(false);
    }

    // --- 버튼에 연결할 함수들 ---

    public void ShowDisplaySettings()
    {
        HideAllPanels(); // 싹 다 끄고
        panelDisplay.SetActive(true); // 화면 패널만 켠다
        titleText.text = "그래픽 설정";
    }

    public void ShowSoundSettings()
    {
        HideAllPanels();
        panelSound.SetActive(true); // 사운드 패널만 켠다
        titleText.text = "사운드 설정";
    }

    public void ShowKeySettings()
    {
        HideAllPanels();
        panelKey.SetActive(true); // 키 패널만 켠다
        titleText.text = "키 설정";
    }
}