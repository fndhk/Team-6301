// 파일 이름: AudioManager.cs
using UnityEngine;
using System.Collections;

// 악기 종류를 구분하기 위한 enum
public enum InstrumentType { Drum, Piano, Cymbal }

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("오디오 소스")]
    //public AudioSource baseMusicSource;  // 기본 곡
    //public AudioSource drumSource;       // 북
    //public AudioSource pianoSource;      // 피아노
    //public AudioSource cymbalSource;     // 심벌즈

    private AudioSource[] allSources;
    private AudioSource drumSource, pianoSource, cymbalSource;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 1. 현재 스테이지 정보를 가져옵니다.
        StageData currentStage = GameSession.instance.selectedStage;
        if (currentStage == null)
        {
            Debug.LogError("AudioManager: 현재 스테이지 정보를 찾을 수 없습니다!");
            return;
        }

        // 2. 이 오브젝트에 붙어있는 모든 AudioSource 컴포넌트를 가져옵니다. (4개여야 함)
        allSources = GetComponents<AudioSource>();
        if (allSources.Length < 4)
        {
            Debug.LogError("AudioManager에 AudioSource 컴포넌트가 4개 필요합니다!");
            return;
        }

        // 3. StageData에서 오디오 클립을 가져와 각 AudioSource에 할당합니다.
        allSources[0].clip = currentStage.baseMusic;
        allSources[1].clip = currentStage.drumTrack;
        allSources[2].clip = currentStage.pianoTrack;
        allSources[3].clip = currentStage.cymbalTrack;

        // 4. 참조 변수 설정 및 초기화
        drumSource = allSources[1];
        pianoSource = allSources[2];
        cymbalSource = allSources[3];

        drumSource.volume = 0f;
        pianoSource.volume = 0f;
        cymbalSource.volume = 0f;
        allSources[0].volume = 1f;

        // 5. 클립이 할당된 소스만 동시에 재생 시작!
        foreach (var source in allSources)
        {
            if (source.clip != null) // 클립이 없으면 재생하지 않음
            {
                source.Play();
            }
        }
    }

    // 성공 판정 시, 해당 악기 소리를 잠깐 들려주는 함수
    public void PlayInstrumentSound(InstrumentType type)
    {
        switch (type)
        {
            case InstrumentType.Drum:
                StartCoroutine(FadeInAndOut(drumSource, 0.2f, 0.5f)); // 0.2초간 켰다가 0.5초에 걸쳐 원래대로
                break;
            case InstrumentType.Piano:
                StartCoroutine(FadeInAndOut(pianoSource, 0.2f, 0.5f));
                break;
            case InstrumentType.Cymbal:
                StartCoroutine(FadeInAndOut(cymbalSource, 0.2f, 0.5f));
                break;
        }
    }

    // 볼륨을 잠시 올렸다가 내리는 코루틴
    private IEnumerator FadeInAndOut(AudioSource source, float sustainTime, float fadeOutTime)
    {
        source.volume = 1f; // 소리를 최대로 킴
        yield return new WaitForSeconds(sustainTime); // 잠시 유지

        // 서서히 소리를 줄임
        float startVolume = source.volume;
        for (float t = 0; t < fadeOutTime; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(startVolume, 0, t / fadeOutTime);
            yield return null;
        }
        source.volume = 0f;
    }

    // 게임 내에서 음악 재생 속도(피치)를 변경하는 함수
    public void ChangePitch(float pitchMultiplier)
    {
        foreach (var source in allSources)
        {
            source.pitch = pitchMultiplier;
        }
    }
}