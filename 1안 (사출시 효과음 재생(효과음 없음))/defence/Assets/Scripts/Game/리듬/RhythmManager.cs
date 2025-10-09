// ���� �̸�: RhythmManager.cs (���� ����)
using UnityEngine;
using System;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager instance;

    private float bpm;
    public float beatInterval;
    public float nextBeatTime;
    private int beatCount = 0;
    public static event Action<int> OnBeat;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        Time.timeScale = 1f;
    }

    void Start()
    {
        // StageManager�� �ƴ� GameSession�� �����ϵ��� ����
        if (GameSession.instance != null && GameSession.instance.selectedStage != null)
        {
            // GameSession�� ����� ���õ� ��������(selectedStage)���� bpm�� �����ɴϴ�.
            bpm = GameSession.instance.selectedStage.bpm;
            beatInterval = 60f / bpm;
            nextBeatTime = Time.time + beatInterval;
        }
        else
        {
            // GameSession�� ã�� �� ���� ���� ���� �⺻��
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

    // JudgmentManager�� ȣ���� �Լ�
    public float GetNearestBeatTime(float attackTime)
    {
        // �� �Լ��� ������ StageManager�� ������ �����Ƿ� ������ �ʿ䰡 �����ϴ�.
        float timeSinceStart = attackTime - (nextBeatTime - beatInterval * beatCount);
        float beatsPassed = timeSinceStart / beatInterval;
        int nearestBeatIndex = Mathf.RoundToInt(beatsPassed);
        return (nextBeatTime - beatInterval * beatCount) + nearestBeatIndex * beatInterval;
    }
}