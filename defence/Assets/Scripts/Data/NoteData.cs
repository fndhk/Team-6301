// 파일 이름: NoteData.cs (내용은 NotePatternData로 변경)

using System;
using UnityEngine;

[Serializable]
public class NotePatternData
{
    [Tooltip("패턴이 시작될 비트 위치")]
    public float startBeat = 1f;

    [Tooltip("패턴이 끝나는 비트 위치 (0이면 곡 끝까지 반복)")]
    public float endBeat = 0f;

    [Tooltip("몇 비트마다 노트를 생성할지 간격 (예: 4면 4비트마다)")]
    public float repeatInterval = 4f;

    [Tooltip("노트가 생성될 레인 번호 (1, 2, 3)")]
    public int laneIndex = 1;
}