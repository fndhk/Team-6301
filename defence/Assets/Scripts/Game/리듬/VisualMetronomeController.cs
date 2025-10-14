// ���� �̸�: VisualMetronomeController.cs (������ ���� ����)
using UnityEngine;

public class VisualMetronomeController : MonoBehaviour
{
    [Header("UI ����")]
    public RectTransform beatIndicator;
    public RectTransform[] barPositions;

    private int fromBarIndex = 0;
    private int toBarIndex = 1;
    private int direction = 1;

    private void OnEnable() { RhythmManager.OnBeat += HandleBeat; }
    private void OnDisable() { RhythmManager.OnBeat -= HandleBeat; }

    private void HandleBeat(int beatNumber)
    {
        // ���� ��ġ�� ���������� ����
        fromBarIndex = toBarIndex;

        // ���� ��ȯ üũ
        if (fromBarIndex >= barPositions.Length - 1) direction = -1;
        else if (fromBarIndex <= 0) direction = 1;

        // ���� ��ǥ ���� ����
        toBarIndex = fromBarIndex + direction;
    }

    void Update()
    {
        if (RhythmManager.instance == null || barPositions.Length < 2) return;

        float lastBeatTime = RhythmManager.instance.nextBeatTime - RhythmManager.instance.beatInterval;
        float progress = (Time.time - lastBeatTime) / RhythmManager.instance.beatInterval;

        // Lerp�� ���൵�� 1�� �Ѿ�� ���� ��Ʈ�� �Ѿ ���̹Ƿ�,
        // ���� ��Ʈ�� �������� ������� �ʵ��� progress�� 1�� ����
        progress = Mathf.Clamp01(progress);

        // �������� ���� ��ġ
        Vector3 startPos = barPositions[fromBarIndex].position;
        Vector3 endPos = barPositions[toBarIndex].position;

        // �ε巴�� �̵�
        beatIndicator.position = Vector3.Lerp(startPos, endPos, progress);
    }
}