// ���� �̸�: AudioManager.cs
using UnityEngine;
using System.Collections;

// �Ǳ� ������ �����ϱ� ���� enum
public enum InstrumentType { Drum, Piano, Cymbal }

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("����� �ҽ�")]
    //public AudioSource baseMusicSource;  // �⺻ ��
    //public AudioSource drumSource;       // ��
    //public AudioSource pianoSource;      // �ǾƳ�
    //public AudioSource cymbalSource;     // �ɹ���

    private AudioSource[] allSources;
    private AudioSource drumSource, pianoSource, cymbalSource;

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

        // 5. Ŭ���� �Ҵ�� �ҽ��� ���ÿ� ��� ����!
        foreach (var source in allSources)
        {
            if (source.clip != null) // Ŭ���� ������ ������� ����
            {
                source.Play();
            }
        }
    }

    // ���� ���� ��, �ش� �Ǳ� �Ҹ��� ��� ����ִ� �Լ�
    public void PlayInstrumentSound(InstrumentType type)
    {
        switch (type)
        {
            case InstrumentType.Drum:
                StartCoroutine(FadeInAndOut(drumSource, 0.2f, 0.5f)); // 0.2�ʰ� �״ٰ� 0.5�ʿ� ���� �������
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
        source.volume = 1f; // �Ҹ��� �ִ�� Ŵ
        yield return new WaitForSeconds(sustainTime); // ��� ����

        // ������ �Ҹ��� ����
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
}