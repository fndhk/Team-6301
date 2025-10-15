// ���� �̸�: RhythmManager.cs (��ü ��ü)
using UnityEngine;
using System;
using System.Collections; // <-- 'IEnumerator'�� ����ϱ� ���� �� ���� �߰��߽��ϴ�!

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager instance;

    private float bpm;
    public float beatInterval;
    public float nextBeatTime;
    private int beatCount = 0;
    public static event Action<int> OnBeat;
    private bool isRhythmStarted = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        Time.timeScale = 1f;
    }

    void Start()
    {
        if (GameSession.instance != null && GameSession.instance.selectedStage != null)
        {
            bpm = GameSession.instance.selectedStage.bpm;
            beatInterval = 60f / bpm;
           // nextBeatTime = Time.time + beatInterval;
        }
        else
        {
            Debug.LogError("RhythmManager: GameSession���� �������� ������ �������� ���߽��ϴ�! �⺻ BPM(120)���� �����մϴ�.");
            bpm = 120;
            beatInterval = 60f / bpm;
            nextBeatTime = Time.time + beatInterval;
        }
    }

    void Update()
    {
        if (Time.time >= nextBeatTime)
        {
            beatCount = (beatCount % 4) + 1;
            OnBeat?.Invoke(beatCount);
            nextBeatTime += beatInterval;
        }
    }
    public void StartRhythm()
    {
        if (isRhythmStarted) return; // �̹� ���������� ����

        isRhythmStarted = true;
        nextBeatTime = Time.time + beatInterval; // ���� �ð� �������� ù ��Ʈ ����

        Debug.Log("RhythmManager: ���� ����!");
    }
    public float GetNearestBeatTime(float attackTime)
    {
        if (!isRhythmStarted) return attackTime; // ������ ���۵��� �ʾ����� �״�� ��ȯ
        float timeSinceStart = attackTime - (nextBeatTime - beatInterval * beatCount);
        float beatsPassed = timeSinceStart / beatInterval;
        int nearestBeatIndex = Mathf.RoundToInt(beatsPassed);
        return (nextBeatTime - beatInterval * beatCount) + nearestBeatIndex * beatInterval;
    }

    // --- BPM ���� �������� ���� �ڵ� ---
    private float originalBpm;
    private bool isSpeedBuffActive = false;

    public void ApplySpeedBuff(float multiplier, float duration)
    {
        if (!isSpeedBuffActive)
        {
            StartCoroutine(SpeedBuffCoroutine(multiplier, duration));
        }
    }

    private IEnumerator SpeedBuffCoroutine(float multiplier, float duration)
    {
        isSpeedBuffActive = true;
        originalBpm = bpm;

        bpm *= multiplier;
        beatInterval = 60f / bpm;
        Debug.Log($"BPM ���� Ȱ��ȭ! ���ο� BPM: {bpm}");

        AudioManager.instance.ChangePitch(multiplier);
        NoteSpawner.instance.RecalculateNoteSpeed();

        yield return new WaitForSeconds(duration);

        bpm = originalBpm;
        beatInterval = 60f / bpm;
        AudioManager.instance.ChangePitch(1f);
        NoteSpawner.instance.RecalculateNoteSpeed();
        Debug.Log("BPM ���� ����. ���� BPM���� ����.");

        isSpeedBuffActive = false;
    }
}