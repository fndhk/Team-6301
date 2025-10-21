//파일 명: SingleNoteData.cs
using System;
using UnityEngine;

[Serializable]
public class SingleNoteData
{
    public float beat;      // 정확한 비트 위치
    public int laneIndex;   // 1~4 레인
}