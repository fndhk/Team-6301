// 파일 이름: StageData.cs
using System.Collections.Generic;
using UnityEngine;

// 이전에 EnemySpawner.cs에 만들었던 Wave 클래스를 여기에 다시 정의하거나,
// 하나의 파일에서 관리하는 것이 좋습니다. 지금은 그대로 사용합니다.

[CreateAssetMenu(fileName = "Stage_", menuName = "TowerDefense/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("스테이지 기본 정보")]
    public int stageIndex;
    public string stageName;
    [Header("음악 정보")] // ▼▼▼ 이 부분을 추가하거나 수정합니다 ▼▼▼
    public AudioClip baseMusic;
    public AudioClip drumTrack;
    public AudioClip pianoTrack;
    public AudioClip cymbalTrack;

    [Header("리듬 & 음악 정보")]
    public float bpm = 120f; //  스테이지의 기본 BPM
    public AudioClip stageMusic; //  스테이지에서 재생할 배경 음악

    [Header("등장 웨이브 정보")]
    public List<Wave> waves = new List<Wave>();
    [Header("채보 정보")]
    public BeatmapData beatmap;
}