using UnityEngine;
using System; // DateTime을 사용하기 위해 필요합니다.
using System.IO; // 파일을 다루기 위해 필요합니다.
using System.Collections.Generic;

public class SaveLoadManager : MonoBehaviour
{
    // --- 싱글톤 패턴 구현 ---
    // 이 스크립트의 인스턴스를 저장할 static 변수
    public static SaveLoadManager instance;

    private void Awake()
    {
        // 씬에 이미 SaveLoadManager가 있는지 확인
        if (instance == null)
        {
            // 없다면, 이 인스턴스를 사용
            instance = this;
            // 씬이 바뀌어도 파괴되지 않도록 설정
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 이미 있다면, 새로 생긴 이 인스턴스는 파괴
            Destroy(gameObject);
        }
    }
    // -------------------------

    // 현재 게임의 데이터를 담을 변수
    public GameData gameData;
    private string saveFileName = "gameSave.json"; // 저장될 파일의 기본 이름

    // 게임 시작 시 호출되는 함수
    void Start()
    {
        // 게임이 시작될 때 기본 데이터로 초기화
        this.gameData = new GameData();
    }

    // --- 주요 기능 함수들 ---

    // 지정된 슬롯 번호에 현재 게임 데이터를 저장하는 함수
    public void SaveGame(int slotIndex)
    {
        // 1. 현재 날짜와 시간을 기록
        gameData.lastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // 2. GameData 객체를 JSON 문자열로 변환
        // true를 인자로 넘기면 보기 좋게 들여쓰기 된 형식으로 변환됩니다.
        string toJsonData = JsonUtility.ToJson(gameData, true);

        // 3. 파일 경로를 정하고 JSON 데이터를 파일로 저장
        string filePath = Path.Combine(Application.persistentDataPath, slotIndex + "_" + saveFileName);
        File.WriteAllText(filePath, toJsonData);

        Debug.Log($"Save Success: {filePath}");
    }

    // 지정된 슬롯 번호의 게임 데이터를 불러오는 함수
    public bool LoadGame(int slotIndex)
    {
        // 1. 파일 경로를 확인
        string filePath = Path.Combine(Application.persistentDataPath, slotIndex + "_" + saveFileName);

        // 2. 해당 경로에 파일이 존재하는지 확인
        if (File.Exists(filePath))
        {
            // 3. 파일을 읽어와서 JSON 데이터를 문자열로 저장
            string fromJsonData = File.ReadAllText(filePath);

            // 4. JSON 문자열을 GameData 객체로 변환하여 gameData 변수에 덮어쓰기
            gameData = JsonUtility.FromJson<GameData>(fromJsonData);

            Debug.Log($"Load Success: {filePath}");
            return true; // 불러오기 성공
        }
        else
        {
            Debug.LogWarning($"No Saved Files: {filePath}");
            return false; // 불러오기 실패
        }
    }

    // (불러오기 화면을 위해 추가) 지정된 슬롯의 데이터 요약 정보만 가져오는 함수
    public GameData LoadSaveSummary(int slotIndex)
    {
        string filePath = Path.Combine(Application.persistentDataPath, slotIndex + "_" + saveFileName);
        if (File.Exists(filePath))
        {
            string fromJsonData = File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameData>(fromJsonData);
        }
        return null; // 파일이 없으면 null 반환
    }
    // 지정된 슬롯 인덱스에 대한 저장 파일 경로를 반환하는 함수입니다.
    private string GetSaveFilePath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, slotIndex + "_" + saveFileName);
    }

    // (아래 함수 추가) 지정된 슬롯에 세이브 파일이 존재하는지 확인하는 함수입니다.
    public bool DoesSaveFileExist(int slotIndex)
    {
        string filePath = GetSaveFilePath(slotIndex);
        return File.Exists(filePath);
    }
    public void DeleteSaveFile(int slotIndex)
    {
        string filePath = GetSaveFilePath(slotIndex);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Save file deleted: {filePath}");
        }
    }
}