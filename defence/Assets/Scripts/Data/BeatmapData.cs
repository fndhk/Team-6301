//파일 명: BeatmapData.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Beatmap", menuName = "TowerDefense/Beatmap Data")]
public class BeatmapData : ScriptableObject
{
    // ------ 신규 추가: 개별 노트 리스트 ------
    [Header("신규 채보 시스템 (에디터 생성)")]
    public List<SingleNoteData> notes = new List<SingleNoteData>();

    [Header("채보 정보")]
    public float totalBeats = 0f;

    // ------ 기존 patterns는 주석 처리 (팀원 혼란 방지, 절대 삭제 금지!) ------
    [Header("구 채보 시스템 (사용 안 함)")]
    //public List<NotePatternData> patterns = new List<NotePatternData>();

    // ------ 신규 추가: 어떤 시스템을 사용할지 선택 ------
    [Tooltip("체크하면 notes 사용, 체크 해제하면 patterns 사용")]
    public bool useNewBeatmapSystem = true;
}