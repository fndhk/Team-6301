// 파일 이름: BeatmapData.cs
using System.Collections.Generic;
using UnityEngine;
using static NoteSpawner;

[CreateAssetMenu(fileName = "New Beatmap", menuName = "TowerDefense/Beatmap Data")]
public class BeatmapData : ScriptableObject
{
    public List<NotePatternData> patterns = new List<NotePatternData>();
}