// ���� �̸�: GameData.cs (��ü ��ü)
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string nickname;
    public int highestClearedStage;

    // ���������� �ְ� ������ ������ Dictionary
    public SerializableDictionary<int, int> stageHighScores;

    public List<string> items;
    public string lastSaveTime;

    // ���� ���� ���� �ý����� ���� ������ ����
    public int enhancementMaterials; // ������ ��ȭ ��� ����
    public int permanentAtkBonus;    // ���� ���ݷ� ���ʽ�
    public int permanentCoreHpBonus; // ���� �ھ� ü�� ���ʽ�
    public float permanentAtkSpeedBonus; // ���� �ӵ� ���ʽ�

    // ������: �� ���� �����͸� ���� �� �������� �ʱ�ȭ�մϴ�.
    public GameData()
    {
        this.nickname = "";
        this.highestClearedStage = 0;
        this.stageHighScores = new SerializableDictionary<int, int>();
        this.items = new List<string>();
        this.lastSaveTime = "";

        // ���� �߰��� �������� �ʱⰪ�� �����մϴ�. ����
        this.enhancementMaterials = 0;
        this.permanentAtkBonus = 0;
        this.permanentCoreHpBonus = 0;
        this.permanentAtkSpeedBonus = 0f;
    }
}

// (SerializableDictionary �κ��� �״�� �Ӵϴ�)
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    // ... (���� �ڵ�� ����) ...
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