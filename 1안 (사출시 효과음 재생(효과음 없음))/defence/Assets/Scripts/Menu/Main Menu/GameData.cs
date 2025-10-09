using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string nickname;
    // public int playerLevel; // 이 줄을 삭제하거나 주석 처리합니다.
    public int highestClearedStage; // 새로 추가: 클리어한 최고 스테이지
    public int currentStage; // 이 줄은 이제 사용하지 않으므로 삭제해도 됩니다.

    // 새로 추가: 스테이지별 최고 점수를 저장할 Dictionary
    public SerializableDictionary<int, int> stageHighScores;

    public List<string> items;
    public string lastSaveTime;

    public GameData()
    {
        this.nickname = "";
        this.highestClearedStage = 0; // 0은 아직 아무 스테이지도 클리어하지 못했다는 의미
        this.stageHighScores = new SerializableDictionary<int, int>();
        this.items = new List<string>();
        this.lastSaveTime = "";
    }
}

// Dictionary는 Unity에서 직접 직렬화(저장)가 안되므로, 직렬화가 가능한 커스텀 클래스를 만들어줍니다.
// 아래 클래스를 GameData.cs 파일 하단이나 별도의 파일에 추가해주세요.
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
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