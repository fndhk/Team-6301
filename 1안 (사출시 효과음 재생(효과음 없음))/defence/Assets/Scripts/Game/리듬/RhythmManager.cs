// 파일 이름: RhythmManager.cs (최종 수정)
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
        // StageManager가 아닌 GameSession을 참조하도록 수정
        if (GameSession.instance != null && GameSession.instance.selectedStage != null)
        {
            // GameSession에 저장된 선택된 스테이지(selectedStage)에서 bpm을 가져옵니다.
            bpm = GameSession.instance.selectedStage.bpm;
            beatInterval = 60f / bpm;
            nextBeatTime = Time.time + beatInterval;
        }
        else
        {
            // GameSession을 찾을 수 없을 때의 비상용 기본값
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

    // JudgmentManager가 호출할 함수
    public float GetNearestBeatTime(float attackTime)
    {
        // 이 함수의 로직은 StageManager와 관련이 없으므로 수정할 필요가 없습니다.
        float timeSinceStart = attackTime - (nextBeatTime - beatInterval * beatCount);
        float beatsPassed = timeSinceStart / beatInterval;
        int nearestBeatIndex = Mathf.RoundToInt(beatsPassed);
        return (nextBeatTime - beatInterval * beatCount) + nearestBeatIndex * beatInterval;
    }
}