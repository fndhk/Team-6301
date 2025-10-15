//���� ��: BeatmapImporter.cs
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class BeatmapImporter : EditorWindow
{
    private TextAsset jsonFile;
    private string beatmapName = "NewBeatmap";

    [MenuItem("Tools/Beatmap Importer")]
    public static void ShowWindow()
    {
        GetWindow<BeatmapImporter>("Beatmap Importer");
    }

    void OnGUI()
    {
        GUILayout.Label("JSON to BeatmapData Converter", EditorStyles.boldLabel);
        GUILayout.Space(10);

        jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);
        
        GUILayout.Space(5);
        
        beatmapName = EditorGUILayout.TextField("Beatmap Name", beatmapName);
        
        GUILayout.Space(10);

        if (GUILayout.Button("Import Beatmap", GUILayout.Height(30)))
        {
            ImportBeatmap();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("��� ���:", EditorStyles.boldLabel);
        GUILayout.Label("1. JSON ������ Project â�� �巡��");
        GUILayout.Label("2. JSON File �ʵ忡 �Ҵ�");
        GUILayout.Label("3. Beatmap Name �Է�");
        GUILayout.Label("4. Import Beatmap ��ư Ŭ��");
        GUILayout.Label("5. Resources/Beatmaps ������ ������");
    }

    void ImportBeatmap()
    {
        if (jsonFile == null)
        {
            EditorUtility.DisplayDialog("����", "JSON ������ �������ּ���!", "Ȯ��");
            return;
        }

        if (string.IsNullOrWhiteSpace(beatmapName))
        {
            EditorUtility.DisplayDialog("����", "Beatmap �̸��� �Է����ּ���!", "Ȯ��");
            return;
        }

        try
        {
            // JSON �Ľ�
            BeatmapExportData exportData = JsonUtility.FromJson<BeatmapExportData>(jsonFile.text);

            // BeatmapData ����
            BeatmapData beatmapData = ScriptableObject.CreateInstance<BeatmapData>();
            beatmapData.notes = exportData.notes;
            beatmapData.totalBeats = exportData.totalBeats;
            beatmapData.useNewBeatmapSystem = true; // �ű� �ý��� ���

            // ���� ��� ����
            string folderPath = "Assets/Resources/Beatmaps";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string assetPath = $"{folderPath}/{beatmapName}.asset";

            // �̹� �����ϸ� ����� Ȯ��
            if (File.Exists(assetPath))
            {
                if (!EditorUtility.DisplayDialog("���", 
                    $"{beatmapName}.asset ������ �̹� �����մϴ�.\n����ðڽ��ϱ�?", 
                    "�����", "���"))
                {
                    return;
                }
            }

            // Asset ����
            AssetDatabase.CreateAsset(beatmapData, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // ���� �޽���
            EditorUtility.DisplayDialog("����!", 
                $" {beatmapName}.asset ���� �Ϸ�!\n" +
                $" ���: {assetPath}\n" +
                $" BPM: {exportData.bpm}\n" +
                $" �� {exportData.notes.Count}�� ��Ʈ, {exportData.totalBeats:F2} ��Ʈ", 
                "Ȯ��");

            // ������ ���� ����
            Selection.activeObject = beatmapData;
            EditorGUIUtility.PingObject(beatmapData);

            Debug.Log($"<color=green>Beatmap Import ����!</color> {assetPath}");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("����", $"Import ����!\n{e.Message}", "Ȯ��");
            Debug.LogError($"Beatmap Import ����: {e}");
        }
    }
}
[System.Serializable]
public class BeatmapExportData
{
    public float bpm;
    public float totalBeats;
    public List<SingleNoteData> notes;
}
#endif