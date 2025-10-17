//���� ��: GachaManager.cs (��ü ����)

using UnityEngine;
using System.Collections.Generic;

public class GachaManager : MonoBehaviour
{
    public static GachaManager instance;

    [Header("ĳ���� Ǯ")]
    public List<CharacterData> allCharacters;

    [Header("��޺� Ȯ�� (�հ� 100%)")]
    [Range(0f, 1f)]
    [SerializeField] private float raritySSR = 0.05f;  // 5%
    [Range(0f, 1f)]
    [SerializeField] private float raritySR = 0.25f;   // 25%
    [Range(0f, 1f)]
    [SerializeField] private float rarityR = 0.70f;    // 70%

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GachaManager: DontDestroyOnLoad ���� �Ϸ�");

            // ------ �ű� �߰�: Ȯ�� ���� ------
            ValidateRarityProbabilities();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ------ �ű� �߰�: Ȯ�� ���� �Լ� ------
    private void ValidateRarityProbabilities()
    {
        float total = raritySSR + raritySR + rarityR;

        if (Mathf.Abs(total - 1f) > 0.01f)  // ���� ��� 1%
        {
            Debug.LogWarning($"GachaManager: Ȯ�� �հ谡 100%�� �ƴմϴ�! (����: {total * 100:F1}%) " +
                           $"SSR:{raritySSR * 100}% + SR:{raritySR * 100}% + R:{rarityR * 100}%");
        }
    }

    public CharacterData DrawCharacter()
    {
        float roll = Random.value;
        CharacterData.CharacterRarity targetRarity;

        // ------ ����: ��� ���� ��� ------
        float cumulativeSSR = raritySSR;
        float cumulativeSR = raritySSR + raritySR;
        float cumulativeR = raritySSR + raritySR + rarityR;

        if (roll < cumulativeSSR)
        {
            targetRarity = CharacterData.CharacterRarity.SSR;
        }
        else if (roll < cumulativeSR)
        {
            targetRarity = CharacterData.CharacterRarity.SR;
        }
        else if (roll < cumulativeR)
        {
            targetRarity = CharacterData.CharacterRarity.R;
        }
        else
        {
            // Ȯ�� ���� 100%�� �ƴ� ��� �⺻��
            Debug.LogWarning($"GachaManager: Ȯ�� ������ ������ϴ�. roll={roll:F3}");
            targetRarity = CharacterData.CharacterRarity.R;
        }

        List<CharacterData> pool = allCharacters.FindAll(c => c.rarity == targetRarity);

        if (pool.Count == 0)
        {
            Debug.LogError($"GachaManager: {targetRarity} ����� ĳ���Ͱ� �����ϴ�!");
            return null;
        }

        CharacterData result = pool[Random.Range(0, pool.Count)];
        Debug.Log($"GachaManager: �̱� ��� - {result.characterName} ({result.rarity})");

        return result;
    }

    public void ProcessGacha(CharacterData drawnCharacter)
    {
        string charID = drawnCharacter.characterID;
        GameData data = SaveLoadManager.instance.gameData;

        if (!data.characterLevels.ContainsKey(charID))
        {
            data.characterLevels[charID] = 1;
            Debug.Log($"�ű� ĳ���� ȹ��: {drawnCharacter.characterName}");
        }
        else if (data.characterLevels[charID] < 3)
        {
            data.characterLevels[charID]++;
            Debug.Log($"ĳ���� ��ȭ! {drawnCharacter.characterName} Lv.{data.characterLevels[charID]}");
        }
        else
        {
            data.gachaTickets++;
            Debug.Log($"Max ���� ĳ����! Ƽ�� ȯ�� +1 ({drawnCharacter.characterName})");
        }
    }
}