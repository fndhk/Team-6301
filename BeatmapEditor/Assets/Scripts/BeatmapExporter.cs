//���� ��: BeatmapExporter.cs
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
            Debug.LogWarning("��ġ�� ��Ʈ�� �����ϴ�!");
            return;
        }

        // ��Ʈ ������� ����
        notes.Sort((a, b) => a.beat.CompareTo(b.beat));

        // ������ ��Ʈ�� ��Ʈ�� ��ü ���̷� ����
        float maxBeat = notes[notes.Count - 1].beat;

        BeatmapExportData data = new BeatmapExportData
        {
            bpm = bpm,
            totalBeats = maxBeat,
            notes = notes
        };

        // JSON ��ȯ
        string json = JsonUtility.ToJson(data, true);

        // ���� ��� ����
        string folderPath = Path.Combine(Application.dataPath, "ExportedBeatmaps");

        // ������ ������ ����
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"���� ����: {folderPath}");
        }

        // ���ϸ� ���� (Ÿ�ӽ����� ����)
        string fileName = $"Beatmap_{System.DateTime.Now:yyyyMMdd_HHmmss}.json";
        string fullPath = Path.Combine(folderPath, fileName);

        // ���� ����
        File.WriteAllText(fullPath, json);

        Debug.Log($" ä�� ���� �Ϸ�!");
        Debug.Log($" ���: {fullPath}");
        Debug.Log($" �� {notes.Count}�� ��Ʈ, {maxBeat:F2} ��Ʈ");

#if UNITY_EDITOR
        // �����Ϳ��� Asset ���ΰ�ħ
        UnityEditor.AssetDatabase.Refresh();

        // ���� ��ġ ����
        UnityEditor.EditorUtility.RevealInFinder(fullPath);
#endif
    }
}
