using UnityEngine;
using TMPro;

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
        currentSaveSlot = GameSession.instance.currentSaveSlot;
        gameData = SaveLoadManager.instance.gameData;
        UpdateAllUI();
    }

    void UpdateAllUI()
    {
        materialsText.text = "cost: " + gameData.enhancementMaterials;

        int currentAtkLevel = gameData.permanentAtkBonus;
        atkLevelText.text = "Lv. " + (currentAtkLevel + 1);
        atkCostText.text = "" + GetAttackUpgradeCost(currentAtkLevel);

        int currentSpeedLevel = Mathf.FloorToInt(gameData.permanentAtkSpeedBonus * 100);
        speedLevelText.text = "Lv. " + (currentSpeedLevel + 1) + " (+" + currentSpeedLevel + "%)";
        speedCostText.text = "" + GetAttackSpeedUpgradeCost(currentSpeedLevel);

        int currentCoreHpLevel = gameData.permanentCoreHpBonus / 10;
        coreHpLevelText.text = "Lv. " + (currentCoreHpLevel + 1);
        coreHpCostText.text = "" + GetCoreHpUpgradeCost(currentCoreHpLevel);
    }

    // --- 비용 계산 함수들 ---

    // 공격력 강화 비용 (예: 기본 100, 레벨당 50씩 증가)
    private int GetAttackUpgradeCost(int currentLevel)
    {
        return 100 + currentLevel * 50;
    }

    // 공격 속도 강화 비용 (예: 기본 150, 레벨당 75씩 증가)
    private int GetAttackSpeedUpgradeCost(int currentLevel)
    {
        return 150 + currentLevel * 75;
    }

    // 코어 HP 강화 비용 (예: 기본 120, 레벨당 40씩 증가)
    private int GetCoreHpUpgradeCost(int currentLevel)
    {
        return 120 + currentLevel * 40;
    }

    // --- 버튼 연결 함수들 ---

    public void OnClickUpgradeAttack()
    {
        int currentLevel = gameData.permanentAtkBonus;
        int cost = GetAttackUpgradeCost(currentLevel);
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentAtkBonus += 1; // 레벨 1 증가
            SaveChangesAndRefreshUI();
        }
        else Debug.Log("재료가 부족합니다.");
    }

    public void OnClickUpgradeAttackSpeed()
    {
        int currentLevel = Mathf.FloorToInt(gameData.permanentAtkSpeedBonus * 100);
        int cost = GetAttackSpeedUpgradeCost(currentLevel);
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentAtkSpeedBonus += 0.01f; // 1% 증가
            SaveChangesAndRefreshUI();
        }
        else Debug.Log("재료가 부족합니다.");
    }

    public void OnClickUpgradeCoreHp()
    {
        int currentLevel = gameData.permanentCoreHpBonus / 10;
        int cost = GetCoreHpUpgradeCost(currentLevel);
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentCoreHpBonus += 10; // 10 HP 증가
            SaveChangesAndRefreshUI();
        }
        else Debug.Log("재료가 부족합니다.");
    }

    private void SaveChangesAndRefreshUI()
    {
        SaveLoadManager.instance.SaveGame(currentSaveSlot);
        UpdateAllUI();
    }

    public void OnClickBackButton()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("StageSelect");
    }
}