using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundSettings : MonoBehaviour
{
    [Header("UI 연결")]
    public Slider masterVolumeSlider;
    public TextMeshProUGUI masterVolumeText; // 현재 볼륨 수치 표시용 (선택사항)

    void Start()
    {
        // 1. AudioManager가 이미 설정한 현재 볼륨 값을 가져옵니다.
        float currentVolume = AudioListener.volume;

        // 2. 슬라이더 UI만 현재 볼륨에 맞춥니다.
        masterVolumeSlider.value = currentVolume;
        UpdateVolumeText(currentVolume);

        // 3. 슬라이더 값 변경 이벤트 연결
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
    }

    // 슬라이더 값이 바뀔 때마다 호출될 함수
    public void OnMasterVolumeChanged(float volume)
    {
        // 게임 전체 볼륨 조절
        AudioListener.volume = volume;

        // 변경된 볼륨 저장 (게임을 껐다 켜도 유지되도록)
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();

        // 텍스트 업데이트
        UpdateVolumeText(volume);
    }

    private void UpdateVolumeText(float volume)
    {
        if (masterVolumeText != null)
        {
            // 0.0 ~ 1.0 값을 0% ~ 100% 로 변환하여 표시
            int volumePercent = Mathf.RoundToInt(volume * 100);
            masterVolumeText.text = $"{volumePercent}";
        }
    }
}