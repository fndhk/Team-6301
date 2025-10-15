using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI ����")]
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

    // --- ��� ��� �Լ��� ---

    // ���ݷ� ��ȭ ��� (��: �⺻ 100, ������ 50�� ����)
    private int GetAttackUpgradeCost(int currentLevel)
    {
        return 100 + currentLevel * 50;
    }

    // ���� �ӵ� ��ȭ ��� (��: �⺻ 150, ������ 75�� ����)
    private int GetAttackSpeedUpgradeCost(int currentLevel)
    {
        return 150 + currentLevel * 75;
    }

    // �ھ� HP ��ȭ ��� (��: �⺻ 120, ������ 40�� ����)
    private int GetCoreHpUpgradeCost(int currentLevel)
    {
        return 120 + currentLevel * 40;
    }

    // --- ��ư ���� �Լ��� ---

    public void OnClickUpgradeAttack()
    {
        int currentLevel = gameData.permanentAtkBonus;
        int cost = GetAttackUpgradeCost(currentLevel);
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentAtkBonus += 1; // ���� 1 ����
            SaveChangesAndRefreshUI();
        }
        else Debug.Log("��ᰡ �����մϴ�.");
    }

    public void OnClickUpgradeAttackSpeed()
    {
        int currentLevel = Mathf.FloorToInt(gameData.permanentAtkSpeedBonus * 100);
        int cost = GetAttackSpeedUpgradeCost(currentLevel);
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentAtkSpeedBonus += 0.01f; // 1% ����
            SaveChangesAndRefreshUI();
        }
        else Debug.Log("��ᰡ �����մϴ�.");
    }

    public void OnClickUpgradeCoreHp()
    {
        int currentLevel = gameData.permanentCoreHpBonus / 10;
        int cost = GetCoreHpUpgradeCost(currentLevel);
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentCoreHpBonus += 10; // 10 HP ����
            SaveChangesAndRefreshUI();
        }
        else Debug.Log("��ᰡ �����մϴ�.");
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