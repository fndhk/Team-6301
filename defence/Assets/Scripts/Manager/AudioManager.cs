// 파일 이름: AudioManager.cs
using UnityEngine;
using System.Collections;

public enum InstrumentType { Drum, Piano, Cymbal }

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private AudioSource[] allSources;
    private AudioSource drumSource, pianoSource, cymbalSource;
    private bool isMusicStarted = false;

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

        // 5. 음악은 바로 재생하지 않고 대기 (CountdownUI가 StartMusic을 호출할 때까지)
    }

    // CountdownUI에서 카운트다운이 끝나면 호출할 함수
    public void StartMusic()
    {
        if (isMusicStarted) return;

        isMusicStarted = true;

        float musicStartTime = Time.time;
    
        foreach (var source in allSources)
        {
            if (source.clip != null)
            {
                source.Play();
            }
        }
    
    // ------ 신규 추가: NoteSpawner에게 정확한 시작 시간 전달 ------
        if (NoteSpawner.instance != null)
        {
            NoteSpawner.instance.StartSpawningAtTime(musicStartTime);
        }

        Debug.Log($"<color=green>[AudioManager] 음악 시작: {musicStartTime:F4}초</color>");

    }

    // ------ 신규 추가: 음악 정지 함수 ------
    public void StopMusic()
    {
        if (!isMusicStarted) return; // 음악이 시작되지 않았으면 무시

        // 모든 오디오 소스 정지
        foreach (var source in allSources)
        {
            if (source != null && source.isPlaying)
            {
                source.Stop();
            }
        }

        isMusicStarted = false;
        Debug.Log("AudioManager: 음악 정지!");
    }

    // ------ 신규 추가: 음악 페이드아웃 함수 (부드러운 정지) ------
    public void FadeOutMusic(float duration = 1f)
    {
        if (!isMusicStarted) return;

        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startTime = Time.time;
        float[] startVolumes = new float[allSources.Length];

        // 현재 볼륨 저장
        for (int i = 0; i < allSources.Length; i++)
        {
            startVolumes[i] = allSources[i].volume;
        }

        // 페이드아웃
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;

            for (int i = 0; i < allSources.Length; i++)
            {
                if (allSources[i] != null)
                {
                    allSources[i].volume = Mathf.Lerp(startVolumes[i], 0f, t);
                }
            }

            yield return null;
        }

        // 완전히 정지
        StopMusic();

        // 볼륨 복원 (다음 재생을 위해)
        allSources[0].volume = 1f;
        drumSource.volume = 0f;
        pianoSource.volume = 0f;
        cymbalSource.volume = 0f;

        Debug.Log("AudioManager: 페이드아웃 완료!");
    }

    // 성공 판정 시, 해당 악기 소리를 잠깐 들려주는 함수
    public void PlayInstrumentSound(InstrumentType type)
    {
        switch (type)
        {
            case InstrumentType.Drum:
                StartCoroutine(FadeInAndOut(drumSource, 0.2f, 0.5f));
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
        source.volume = 1f;
        yield return new WaitForSeconds(sustainTime);

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
    public void PauseAllMusic()
    {
        foreach (var source in allSources)
        {
            if (source != null && source.isPlaying)
            {
                source.Pause();
            }
        }
        Debug.Log("AudioManager: 모든 음악 일시정지!");
    }

    public void UnpauseAllMusic()
    {
        foreach (var source in allSources)
        {
            if (source != null)
            {
                source.UnPause();
            }
        }
        Debug.Log("AudioManager: 모든 음악 재개!");
    }
}