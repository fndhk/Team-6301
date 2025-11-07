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
        // 1. 게임 시작 시 저장된 볼륨 불러오기 (없으면 기본값 1.0)
        // PlayerPrefs는 간단한 설정값을 저장하기에 좋습니다.
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);

        // 2. 오디오 리스너 볼륨 설정
        AudioListener.volume = savedVolume;

        // 3. 슬라이더 UI 초기화
        masterVolumeSlider.value = savedVolume;
        UpdateVolumeText(savedVolume);

        // 4. 슬라이더 값 변경 이벤트 연결
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