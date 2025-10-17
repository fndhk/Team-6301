//파일 명: BeatmapExporter.cs
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class BeatmapExportData
{
    public float bpm;
    public float totalBeats;
    public List<SingleNoteData> notes;
}

public static class BeatmapExporter
{
    public static void Export(List<SingleNoteData> notes, float bpm)
    {
        if (notes == null || notes.Count == 0)
        {
            Debug.LogWarning("배치된 노트가 없습니다!");
            return;
        }

        // 비트 순서대로 정렬
        notes.Sort((a, b) => a.beat.CompareTo(b.beat));

        // 마지막 노트의 비트를 전체 길이로 설정
        float maxBeat = notes[notes.Count - 1].beat;

        BeatmapExportData data = new BeatmapExportData
        {
            bpm = bpm,
            totalBeats = maxBeat,
            notes = notes
        };

        // JSON 변환
        string json = JsonUtility.ToJson(data, true);

        // 저장 경로 설정
        string folderPath = Path.Combine(Application.dataPath, "ExportedBeatmaps");

        // 폴더가 없으면 생성
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"폴더 생성: {folderPath}");
        }

        // 파일명 생성 (타임스탬프 포함)
        string fileName = $"Beatmap_{System.DateTime.Now:yyyyMMdd_HHmmss}.json";
        string fullPath = Path.Combine(folderPath, fileName);

        // 파일 저장
        File.WriteAllText(fullPath, json);

        Debug.Log($" 채보 저장 완료!");
        Debug.Log($" 경로: {fullPath}");
        Debug.Log($" 총 {notes.Count}개 노트, {maxBeat:F2} 비트");

#if UNITY_EDITOR
        // 에디터에서 Asset 새로고침
        UnityEditor.AssetDatabase.Refresh();

        // 저장 위치 열기
        UnityEditor.EditorUtility.RevealInFinder(fullPath);
#endif
    }
}
