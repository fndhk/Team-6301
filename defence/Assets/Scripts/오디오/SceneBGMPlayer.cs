// 파일명: SceneBGMPlayer.cs
// 메뉴 씬(MainMenu, StoreScene, GachaScene, StageSelect)에서 배경음악을 재생하는 스크립트
using UnityEngine;

public class SceneBGMPlayer : MonoBehaviour
{
    [Header("배경음악 설정")]
    [Tooltip("이 씬에서 재생할 배경음악 클립")]
    public AudioClip bgmClip;

    [Range(0f, 1f)]
    [Tooltip("배경음악 볼륨 (0~1)")]
    public float volume = 0.5f;

    [Tooltip("루프 재생 여부")]
    public bool loop = true;

    [Tooltip("씬 시작 시 자동 재생")]
    public bool playOnStart = true;

    private AudioSource audioSource;

    void Awake()
    {
        // AudioSource 컴포넌트 가져오기 또는 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // AudioSource 설정
        audioSource.clip = bgmClip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        if (playOnStart && bgmClip != null)
        {
            PlayBGM();
        }
    }

    /// <summary>
    /// BGM 재생
    /// </summary>
    public void PlayBGM()
    {
        if (audioSource != null && bgmClip != null)
        {
            audioSource.Play();
            Debug.Log($"SceneBGMPlayer: {bgmClip.name} 재생 시작");
        }
        else
        {
            Debug.LogWarning("SceneBGMPlayer: bgmClip이 할당되지 않았습니다!");
        }
    }

    /// <summary>
    /// BGM 정지
    /// </summary>
    public void StopBGM()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("SceneBGMPlayer: BGM 정지");
        }
    }

    /// <summary>
    /// BGM 일시정지
    /// </summary>
    public void PauseBGM()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    /// <summary>
    /// BGM 재개
    /// </summary>
    public void ResumeBGM()
    {
        if (audioSource != null)
        {
            audioSource.UnPause();
        }
    }

    /// <summary>
    /// 볼륨 변경
    /// </summary>
    /// <param name="newVolume">새 볼륨 (0~1)</param>
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    void OnDestroy()
    {
        // 씬 전환 시 BGM 정지
        StopBGM();
    }
}
