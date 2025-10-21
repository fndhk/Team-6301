//파일 명: GachaVideoPlayer.cs
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class GachaVideoPlayer : MonoBehaviour
{
    public static GachaVideoPlayer instance;

    [Header("Video Player 설정")]
    public VideoPlayer videoPlayer;
    public RawImage videoDisplay;
    public GameObject videoPanel;

    [Header("등급별 동영상")]
    public VideoClip videoR;
    public VideoClip videoSR;
    public VideoClip videoSSR;

    private System.Action onVideoComplete;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }

        if (videoPanel != null)
        {
            videoPanel.SetActive(false);
        }
    }

    public void PlayGachaVideo(CharacterData.CharacterRarity rarity, System.Action onComplete)
    {
        VideoClip clipToPlay = null;

        switch (rarity)
        {
            case CharacterData.CharacterRarity.R:
                clipToPlay = videoR;
                break;
            case CharacterData.CharacterRarity.SR:
                clipToPlay = videoSR;
                break;
            case CharacterData.CharacterRarity.SSR:
                clipToPlay = videoSSR;
                break;
        }

        if (clipToPlay == null)
        {
            Debug.LogWarning($"GachaVideoPlayer: {rarity} 등급의 동영상이 없습니다!");
            onComplete?.Invoke();
            return;
        }

        onVideoComplete = onComplete;
        StartCoroutine(PlayVideoCoroutine(clipToPlay));
    }

    private IEnumerator PlayVideoCoroutine(VideoClip clip)
    {
        videoPanel.SetActive(true);

        videoPlayer.clip = clip;
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        videoPanel.SetActive(false);
        onVideoComplete?.Invoke();
        onVideoComplete = null;
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}