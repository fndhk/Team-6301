using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class DisplaySettings : MonoBehaviour
{
    [Header("설정 컨트롤 연결")]
    public TMP_Dropdown frameDropdown;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    [Header("상태 표시 텍스트 연결")] // ★★★ 새로 추가된 부분 ★★★
    public TextMeshProUGUI currentFrameText;
    public TextMeshProUGUI currentResolutionText;

    private int[] frameRates = { 60, 120, 144, 240, 360, -1 };
    private Vector2Int[] targetResolutions = new Vector2Int[]
    {
        new Vector2Int(1600, 900),
        new Vector2Int(1280, 720),
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
        new Vector2Int(3840, 2160)
    };

    void Start()
    {
        // 초기화 및 리스너 등록
        InitFrameDropdown();
        InitFullscreenToggle();
        InitResolutionDropdown();

        frameDropdown.onValueChanged.AddListener(OnFrameRateChanged);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
    }

    // --- 1. 프레임 설정 ---
    void InitFrameDropdown()
    {
        frameDropdown.ClearOptions();
        List<string> options = new List<string> { "60 FPS", "120 FPS", "144 FPS", "240 FPS", "360 FPS", "제한없음" };
        frameDropdown.AddOptions(options);

        int currentRate = Application.targetFrameRate;
        int index = System.Array.IndexOf(frameRates, currentRate);
        if (index != -1) frameDropdown.value = index;
        else frameDropdown.value = 5;

        frameDropdown.RefreshShownValue();
        UpdateFrameText(frameDropdown.value); // ★ 초기 텍스트 업데이트
    }

    public void OnFrameRateChanged(int index)
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRates[index];
        UpdateFrameText(index); // ★ 변경 시 텍스트 업데이트
    }

    // ★ 프레임 텍스트 업데이트 함수
    private void UpdateFrameText(int index)
    {
        if (frameRates[index] == -1) currentFrameText.text = "제한없음";
        else currentFrameText.text = $"{frameRates[index]} FPS";
    }

    // --- 2. 전체화면 설정 ---
    void InitFullscreenToggle()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
    }

    public void OnFullscreenToggleChanged(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    // --- 3. 해상도 설정 ---
    void InitResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < targetResolutions.Length; i++)
        {
            options.Add($"{targetResolutions[i].x} x {targetResolutions[i].y}");
            if (targetResolutions[i].x == Screen.width && targetResolutions[i].y == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        UpdateResolutionText(currentResolutionIndex); // ★ 초기 텍스트 업데이트
    }

    public void OnResolutionChanged(int index)
    {
        Vector2Int targetRes = targetResolutions[index];
        Screen.SetResolution(targetRes.x, targetRes.y, Screen.fullScreenMode);
        UpdateResolutionText(index); // ★ 변경 시 텍스트 업데이트
    }

    // ★ 해상도 텍스트 업데이트 함수
    private void UpdateResolutionText(int index)
    {
        currentResolutionText.text = $"{targetResolutions[index].x}x{targetResolutions[index].y}";
    }
}