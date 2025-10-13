using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class ShopManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI materialsText;
    public TextMeshProUGUI atkLevelText;
    public TextMeshProUGUI atkCostText;
    public TextMeshProUGUI speedLevelText;
    public TextMeshProUGUI speedCostText;
    public TextMeshProUGUI coreHpLevelText;
    public TextMeshProUGUI coreHpCostText;

    private GameData gameData;
    private int currentSaveSlot;

    void Start()
    {
        // 현재 세이브 슬롯의 게임 데이터를 불러옴
        currentSaveSlot = GameSession.instance.currentSaveSlot;
        gameData = SaveLoadManager.instance.gameData;

        // 모든 UI를 최신 정보로 업데이트
        UpdateAllUI();
    }

    // 모든 UI 텍스트를 현재 데이터에 맞게 새로고침하는 함수
    void UpdateAllUI()
    {
        materialsText.text = "보유 재료: " + gameData.enhancementMaterials;

        atkLevelText.text = "Lv. " + (gameData.permanentAtkBonus + 1);
        atkCostText.text = "비용: " + GetUpgradeCost(gameData.permanentAtkBonus);

        // 공격 속도는 %로 표시 (예: 0.1f -> 10%)
        speedLevelText.text = "Lv. " + ((gameData.permanentAtkSpeedBonus * 10) + 1) + " (+" + (gameData.permanentAtkSpeedBonus * 100) + "%)";
        speedCostText.text = "비용: " + GetUpgradeCost((int)(gameData.permanentAtkSpeedBonus * 10));

        coreHpLevelText.text = "Lv. " + (gameData.permanentCoreHpBonus / 10 + 1); // 예시: 체력 10당 1레벨
        coreHpCostText.text = "비용: " + GetUpgradeCost(gameData.permanentCoreHpBonus / 10);
    }

    // 레벨에 따라 비용을 계산하는 함수 (예시)
    private int GetUpgradeCost(int currentLevel)
    {
        // 예: 100 + (레벨 * 50)
        return 100 + currentLevel * 50;
    }

    // --- 버튼에 연결할 함수들 ---

    public void OnClickUpgradeAttack()
    {
        int cost = GetUpgradeCost(gameData.permanentAtkBonus);
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentAtkBonus += 1; // 공격력 1 증가
            SaveChangesAndRefreshUI();
        }
        else
        {
            Debug.Log("재료가 부족합니다.");
        }
    }

    public void OnClickUpgradeAttackSpeed()
    {
        int cost = GetUpgradeCost((int)(gameData.permanentAtkSpeedBonus * 10));
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentAtkSpeedBonus += 0.01f; // 공격 속도 1% 증가
            SaveChangesAndRefreshUI();
        }
        else
        {
            Debug.Log("재료가 부족합니다.");
        }
    }

    public void OnClickUpgradeCoreHp()
    {
        int cost = GetUpgradeCost(gameData.permanentCoreHpBonus / 10);
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentCoreHpBonus += 10; // 체력 10 증가
            SaveChangesAndRefreshUI();
        }
        else
        {
            Debug.Log("재료가 부족합니다.");
        }
    }

    private void SaveChangesAndRefreshUI()
    {
        // 변경된 데이터를 파일에 저장
        SaveLoadManager.instance.SaveGame(currentSaveSlot);
        // UI 새로고침
        UpdateAllUI();
    }
}