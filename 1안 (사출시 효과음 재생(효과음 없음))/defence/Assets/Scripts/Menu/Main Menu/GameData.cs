using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string nickname;
    // public int playerLevel; // �� ���� �����ϰų� �ּ� ó���մϴ�.
    public int highestClearedStage; // ���� �߰�: Ŭ������ �ְ� ��������
    public int currentStage; // �� ���� ���� ������� �����Ƿ� �����ص� �˴ϴ�.

    // ���� �߰�: ���������� �ְ� ������ ������ Dictionary
    public SerializableDictionary<int, int> stageHighScores;

    public List<string> items;
    public string lastSaveTime;

    public GameData()
    {
        this.nickname = "";
        this.highestClearedStage = 0; // 0�� ���� �ƹ� ���������� Ŭ�������� ���ߴٴ� �ǹ�
        this.stageHighScores = new SerializableDictionary<int, int>();
        this.items = new List<string>();
        this.lastSaveTime = "";
    }
}

// Dictionary�� Unity���� ���� ����ȭ(����)�� �ȵǹǷ�, ����ȭ�� ������ Ŀ���� Ŭ������ ������ݴϴ�.
// �Ʒ� Ŭ������ GameData.cs ���� �ϴ��̳� ������ ���Ͽ� �߰����ּ���.
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