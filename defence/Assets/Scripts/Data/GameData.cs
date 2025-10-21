using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string nickname;
    public int highestClearedStage;
    public string currentSelectedCharacterID; // 현재 선택된 캐릭터 ID
    public int gachaTickets; // 보유 티켓 수
    public SerializableDictionary<string, int> characterLevels; // 캐릭터ID, 레벨(0~3)
    public List<string> firstClearRewards; // 첫 클리어한 스테이지 목록
    // 스테이지별 최고 점수를 저장할 Dictionary
    public SerializableDictionary<int, int> stageHighScores;

    public List<string> items;
    public string lastSaveTime;

    // ▼▼▼ 영구 성장 시스템을 위한 변수들 ▼▼▼
    public int enhancementMaterials; // 보유한 강화 재료 개수
    public int permanentAtkBonus;    // 영구 공격력 보너스
    public int permanentCoreHpBonus; // 영구 코어 체력 보너스
    public float permanentAtkSpeedBonus; // 공속 보너스
    public int permanentMultiShotCount; // 타워 공속

    public int permanentAtkLevel;
    public int permanentAtkSpeedLevel;
    public int permanentCoreHpLevel;
    public int permanentMultiShotLevel; // 영구 연사 레벨 보너스 (레벨 1당 1발 추가)

    // 생성자: 새 게임 데이터를 만들 때 변수들을 초기화합니다.
    public GameData()
    {
        currentSelectedCharacterID = "Char_Boom"; // 기본 캐릭터
        gachaTickets = 0;
        characterLevels = new SerializableDictionary<string, int>();
        characterLevels.Add("Char_Boom", 1); // 기본 캐릭터는 1레벨로 시작
        firstClearRewards = new List<string>();

        this.nickname = "";
        this.highestClearedStage = 0;
        this.stageHighScores = new SerializableDictionary<int, int>();
        this.items = new List<string>();
        this.lastSaveTime = "";

        // ▼▼▼ 추가된 변수들의 초기값을 설정합니다. ▼▼▼
        this.enhancementMaterials = 5000;
        this.permanentAtkBonus = 0;
        this.permanentCoreHpBonus = 0;
        this.permanentAtkSpeedBonus = 0f;
        this.permanentMultiShotCount = 0;

        this.permanentAtkLevel = 0;
        this.permanentAtkSpeedLevel = 0;
        this.permanentCoreHpLevel = 0;
        this.permanentMultiShotLevel = 0; // 초기 레벨 0 (총 1발 발사)
    }
}

// (SerializableDictionary 부분은 그대로 둡니다)
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    // ... (기존 코드와 동일) ...
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
            throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

        for (int i = 0; i < keys.Count; i++)
            this.Add(keys[i], values[i]);
    }
}