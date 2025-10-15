//파일 명: BeatmapImporter.cs
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
        GUILayout.Label("사용 방법:", EditorStyles.boldLabel);
        GUILayout.Label("1. JSON 파일을 Project 창에 드래그");
        GUILayout.Label("2. JSON File 필드에 할당");
        GUILayout.Label("3. Beatmap Name 입력");
        GUILayout.Label("4. Import Beatmap 버튼 클릭");
        GUILayout.Label("5. Resources/Beatmaps 폴더에 생성됨");
    }

    void ImportBeatmap()
    {
        if (jsonFile == null)
        {
            EditorUtility.DisplayDialog("에러", "JSON 파일을 선택해주세요!", "확인");
            return;
        }

        if (string.IsNullOrWhiteSpace(beatmapName))
        {
            EditorUtility.DisplayDialog("에러", "Beatmap 이름을 입력해주세요!", "확인");
            return;
        }

        try
        {
            // JSON 파싱
            BeatmapExportData exportData = JsonUtility.FromJson<BeatmapExportData>(jsonFile.text);

            // BeatmapData 생성
            BeatmapData beatmapData = ScriptableObject.CreateInstance<BeatmapData>();
            beatmapData.notes = exportData.notes;
            beatmapData.totalBeats = exportData.totalBeats;
            beatmapData.useNewBeatmapSystem = true; // 신규 시스템 사용

            // 저장 경로 설정
            string folderPath = "Assets/Resources/Beatmaps";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string assetPath = $"{folderPath}/{beatmapName}.asset";

            // 이미 존재하면 덮어쓸지 확인
            if (File.Exists(assetPath))
            {
                if (!EditorUtility.DisplayDialog("경고", 
                    $"{beatmapName}.asset 파일이 이미 존재합니다.\n덮어쓰시겠습니까?", 
                    "덮어쓰기", "취소"))
                {
                    return;
                }
            }

            // Asset 생성
            AssetDatabase.CreateAsset(beatmapData, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 성공 메시지
            EditorUtility.DisplayDialog("성공!", 
                $" {beatmapName}.asset 생성 완료!\n" +
                $" 경로: {assetPath}\n" +
                $" BPM: {exportData.bpm}\n" +
                $" 총 {exportData.notes.Count}개 노트, {exportData.totalBeats:F2} 비트", 
                "확인");

            // 생성된 파일 선택
            Selection.activeObject = beatmapData;
            EditorGUIUtility.PingObject(beatmapData);

            Debug.Log($"<color=green>Beatmap Import 성공!</color> {assetPath}");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("에러", $"Import 실패!\n{e.Message}", "확인");
            Debug.LogError($"Beatmap Import 실패: {e}");
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