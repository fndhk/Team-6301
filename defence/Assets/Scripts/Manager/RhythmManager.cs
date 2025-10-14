// 파일 이름: RhythmManager.cs (전체 교체)
using UnityEngine;
using System;
using System.Collections; // <-- 'IEnumerator'를 사용하기 위해 이 줄을 추가했습니다!

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
            Debug.LogError("RhythmManager: GameSession에서 스테이지 정보를 가져오지 못했습니다! 기본 BPM(120)으로 실행합니다.");
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
        if (isRhythmStarted) return; // 이미 시작했으면 무시

        isRhythmStarted = true;
        nextBeatTime = Time.time + beatInterval; // 현재 시간 기준으로 첫 비트 설정

        Debug.Log("RhythmManager: 리듬 시작!");
    }
    public float GetNearestBeatTime(float attackTime)
    {
        if (!isRhythmStarted) return attackTime; // 리듬이 시작되지 않았으면 그대로 반환
        float timeSinceStart = attackTime - (nextBeatTime - beatInterval * beatCount);
        float beatsPassed = timeSinceStart / beatInterval;
        int nearestBeatIndex = Mathf.RoundToInt(beatsPassed);
        return (nextBeatTime - beatInterval * beatCount) + nearestBeatIndex * beatInterval;
    }

    // --- BPM 조절 아이템을 위한 코드 ---
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
        Debug.Log($"BPM 버프 활성화! 새로운 BPM: {bpm}");

        AudioManager.instance.ChangePitch(multiplier);
        NoteSpawner.instance.RecalculateNoteSpeed();

        yield return new WaitForSeconds(duration);

        bpm = originalBpm;
        beatInterval = 60f / bpm;
        AudioManager.instance.ChangePitch(1f);
        NoteSpawner.instance.RecalculateNoteSpeed();
        Debug.Log("BPM 버프 종료. 원래 BPM으로 복구.");

        isSpeedBuffActive = false;
    }
}