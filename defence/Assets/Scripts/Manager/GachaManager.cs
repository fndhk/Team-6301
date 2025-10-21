//파일 명: GachaManager.cs (전체 수정)

using UnityEngine;
using System.Collections.Generic;

public class GachaManager : MonoBehaviour
{
    public static GachaManager instance;

    [Header("캐릭터 풀")]
    public List<CharacterData> allCharacters;

    [Header("등급별 확률 (합계 100%)")]
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
            Debug.Log("GachaManager: DontDestroyOnLoad 설정 완료");

            // ------ 신규 추가: 확률 검증 ------
            ValidateRarityProbabilities();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ------ 신규 추가: 확률 검증 함수 ------
    private void ValidateRarityProbabilities()
    {
        float total = raritySSR + raritySR + rarityR;

        if (Mathf.Abs(total - 1f) > 0.01f)  // 오차 허용 1%
        {
            Debug.LogWarning($"GachaManager: 확률 합계가 100%가 아닙니다! (현재: {total * 100:F1}%) " +
                           $"SSR:{raritySSR * 100}% + SR:{raritySR * 100}% + R:{rarityR * 100}%");
        }
    }

    public CharacterData DrawCharacter()
    {
        float roll = Random.value;
        CharacterData.CharacterRarity targetRarity;

        // ------ 수정: 모든 변수 사용 ------
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
            // 확률 합이 100%가 아닐 경우 기본값
            Debug.LogWarning($"GachaManager: 확률 범위를 벗어났습니다. roll={roll:F3}");
            targetRarity = CharacterData.CharacterRarity.R;
        }

        List<CharacterData> pool = allCharacters.FindAll(c => c.rarity == targetRarity);

        if (pool.Count == 0)
        {
            Debug.LogError($"GachaManager: {targetRarity} 등급의 캐릭터가 없습니다!");
            return null;
        }

        CharacterData result = pool[Random.Range(0, pool.Count)];
        Debug.Log($"GachaManager: 뽑기 결과 - {result.characterName} ({result.rarity})");

        return result;
    }

    public void ProcessGacha(CharacterData drawnCharacter)
    {
        string charID = drawnCharacter.characterID;
        GameData data = SaveLoadManager.instance.gameData;

        if (!data.characterLevels.ContainsKey(charID))
        {
            data.characterLevels[charID] = 1;
            Debug.Log($"신규 캐릭터 획득: {drawnCharacter.characterName}");
        }
        else if (data.characterLevels[charID] < 3)
        {
            data.characterLevels[charID]++;
            Debug.Log($"캐릭터 강화! {drawnCharacter.characterName} Lv.{data.characterLevels[charID]}");
        }
        else
        {
            data.gachaTickets++;
            Debug.Log($"Max 레벨 캐릭터! 티켓 환불 +1 ({drawnCharacter.characterName})");
        }
    }
}