// 파일 이름: GameData.cs (전체 교체)
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string nickname;
    public int highestClearedStage;

    // 스테이지별 최고 점수를 저장할 Dictionary
    public SerializableDictionary<int, int> stageHighScores;

    public List<string> items;
    public string lastSaveTime;

    // ▼▼▼ 영구 성장 시스템을 위한 변수들 ▼▼▼
    public int enhancementMaterials; // 보유한 강화 재료 개수
    public int permanentAtkBonus;    // 영구 공격력 보너스
    public int permanentCoreHpBonus; // 영구 코어 체력 보너스
    public float permanentAtkSpeedBonus; // 공격 속도 보너스

    // 생성자: 새 게임 데이터를 만들 때 변수들을 초기화합니다.
    public GameData()
    {
        this.nickname = "";
        this.highestClearedStage = 0;
        this.stageHighScores = new SerializableDictionary<int, int>();
        this.items = new List<string>();
        this.lastSaveTime = "";

        // ▼▼▼ 추가된 변수들의 초기값을 설정합니다. ▼▼▼
        this.enhancementMaterials = 0;
        this.permanentAtkBonus = 0;
        this.permanentCoreHpBonus = 0;
        this.permanentAtkSpeedBonus = 0f;
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