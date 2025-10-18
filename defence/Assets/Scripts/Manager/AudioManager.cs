// ���� �̸�: AudioManager.cs
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
        // 1. ���� �������� ������ �����ɴϴ�.
        StageData currentStage = GameSession.instance.selectedStage;
        if (currentStage == null)
        {
            Debug.LogError("AudioManager: ���� �������� ������ ã�� �� �����ϴ�!");
            return;
        }

        // 2. �� ������Ʈ�� �پ��ִ� ��� AudioSource ������Ʈ�� �����ɴϴ�. (4������ ��)
        allSources = GetComponents<AudioSource>();
        if (allSources.Length < 4)
        {
            Debug.LogError("AudioManager�� AudioSource ������Ʈ�� 4�� �ʿ��մϴ�!");
            return;
        }

        // 3. StageData���� ����� Ŭ���� ������ �� AudioSource�� �Ҵ��մϴ�.
        allSources[0].clip = currentStage.baseMusic;
        allSources[1].clip = currentStage.drumTrack;
        allSources[2].clip = currentStage.pianoTrack;
        allSources[3].clip = currentStage.cymbalTrack;

        // 4. ���� ���� ���� �� �ʱ�ȭ
        drumSource = allSources[1];
        pianoSource = allSources[2];
        cymbalSource = allSources[3];

        drumSource.volume = 0f;
        pianoSource.volume = 0f;
        cymbalSource.volume = 0f;
        allSources[0].volume = 1f;

        // 5. ������ �ٷ� ������� �ʰ� ��� (CountdownUI�� StartMusic�� ȣ���� ������)
    }

    // CountdownUI���� ī��Ʈ�ٿ��� ������ ȣ���� �Լ�
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
    
    // ------ �ű� �߰�: NoteSpawner���� ��Ȯ�� ���� �ð� ���� ------
        if (NoteSpawner.instance != null)
        {
            NoteSpawner.instance.StartSpawningAtTime(musicStartTime);
        }

        Debug.Log($"<color=green>[AudioManager] ���� ����: {musicStartTime:F4}��</color>");

    }

    // ------ �ű� �߰�: ���� ���� �Լ� ------
    public void StopMusic()
    {
        if (!isMusicStarted) return; // ������ ���۵��� �ʾ����� ����

        // ��� ����� �ҽ� ����
        foreach (var source in allSources)
        {
            if (source != null && source.isPlaying)
            {
                source.Stop();
            }
        }

        isMusicStarted = false;
        Debug.Log("AudioManager: ���� ����!");
    }

    // ------ �ű� �߰�: ���� ���̵�ƿ� �Լ� (�ε巯�� ����) ------
    public void FadeOutMusic(float duration = 1f)
    {
        if (!isMusicStarted) return;

        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startTime = Time.time;
        float[] startVolumes = new float[allSources.Length];

        // ���� ���� ����
        for (int i = 0; i < allSources.Length; i++)
        {
            startVolumes[i] = allSources[i].volume;
        }

        // ���̵�ƿ�
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

        // ������ ����
        StopMusic();

        // ���� ���� (���� ����� ����)
        allSources[0].volume = 1f;
        drumSource.volume = 0f;
        pianoSource.volume = 0f;
        cymbalSource.volume = 0f;

        Debug.Log("AudioManager: ���̵�ƿ� �Ϸ�!");
    }

    // ���� ���� ��, �ش� �Ǳ� �Ҹ��� ��� ����ִ� �Լ�
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

    // ������ ��� �÷ȴٰ� ������ �ڷ�ƾ
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

    // ���� ������ ���� ��� �ӵ�(��ġ)�� �����ϴ� �Լ�
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
        Debug.Log("AudioManager: ��� ���� �Ͻ�����!");
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
        Debug.Log("AudioManager: ��� ���� �簳!");
    }
}