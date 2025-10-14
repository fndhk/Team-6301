// 파일 이름: VisualMetronomeController.cs (수정된 최종 버전)
using UnityEngine;

public class VisualMetronomeController : MonoBehaviour
{
    [Header("UI 연결")]
    public RectTransform beatIndicator;
    public RectTransform[] barPositions;

    private int fromBarIndex = 0;
    private int toBarIndex = 1;
    private int direction = 1;

    private void OnEnable() { RhythmManager.OnBeat += HandleBeat; }
    private void OnDisable() { RhythmManager.OnBeat -= HandleBeat; }

    private void HandleBeat(int beatNumber)
    {
        // 현재 위치를 시작점으로 설정
        fromBarIndex = toBarIndex;

        // 방향 전환 체크
        if (fromBarIndex >= barPositions.Length - 1) direction = -1;
        else if (fromBarIndex <= 0) direction = 1;

        // 다음 목표 지점 설정
        toBarIndex = fromBarIndex + direction;
    }

    void Update()
    {
        if (RhythmManager.instance == null || barPositions.Length < 2) return;

        float lastBeatTime = RhythmManager.instance.nextBeatTime - RhythmManager.instance.beatInterval;
        float progress = (Time.time - lastBeatTime) / RhythmManager.instance.beatInterval;

        // Lerp의 진행도가 1을 넘어서면 다음 비트로 넘어간 것이므로,
        // 다음 비트의 움직임이 어색하지 않도록 progress를 1로 고정
        progress = Mathf.Clamp01(progress);

        // 시작점과 끝점 위치
        Vector3 startPos = barPositions[fromBarIndex].position;
        Vector3 endPos = barPositions[toBarIndex].position;

        // 부드럽게 이동
        beatIndicator.position = Vector3.Lerp(startPos, endPos, progress);
    }
}