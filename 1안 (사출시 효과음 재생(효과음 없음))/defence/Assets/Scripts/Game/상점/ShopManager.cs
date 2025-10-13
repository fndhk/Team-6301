using UnityEngine;
using TMPro; // TextMeshPro�� ����ϱ� ���� �߰�

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
        // ���� ���̺� ������ ���� �����͸� �ҷ���
        currentSaveSlot = GameSession.instance.currentSaveSlot;
        gameData = SaveLoadManager.instance.gameData;

        // ��� UI�� �ֽ� ������ ������Ʈ
        UpdateAllUI();
    }

    // ��� UI �ؽ�Ʈ�� ���� �����Ϳ� �°� ���ΰ�ħ�ϴ� �Լ�
    void UpdateAllUI()
    {
        materialsText.text = "���� ���: " + gameData.enhancementMaterials;

        atkLevelText.text = "Lv. " + (gameData.permanentAtkBonus + 1);
        atkCostText.text = "���: " + GetUpgradeCost(gameData.permanentAtkBonus);

        // ���� �ӵ��� %�� ǥ�� (��: 0.1f -> 10%)
        speedLevelText.text = "Lv. " + ((gameData.permanentAtkSpeedBonus * 10) + 1) + " (+" + (gameData.permanentAtkSpeedBonus * 100) + "%)";
        speedCostText.text = "���: " + GetUpgradeCost((int)(gameData.permanentAtkSpeedBonus * 10));

        coreHpLevelText.text = "Lv. " + (gameData.permanentCoreHpBonus / 10 + 1); // ����: ü�� 10�� 1����
        coreHpCostText.text = "���: " + GetUpgradeCost(gameData.permanentCoreHpBonus / 10);
    }

    // ������ ���� ����� ����ϴ� �Լ� (����)
    private int GetUpgradeCost(int currentLevel)
    {
        // ��: 100 + (���� * 50)
        return 100 + currentLevel * 50;
    }

    // --- ��ư�� ������ �Լ��� ---

    public void OnClickUpgradeAttack()
    {
        int cost = GetUpgradeCost(gameData.permanentAtkBonus);
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentAtkBonus += 1; // ���ݷ� 1 ����
            SaveChangesAndRefreshUI();
        }
        else
        {
            Debug.Log("��ᰡ �����մϴ�.");
        }
    }

    public void OnClickUpgradeAttackSpeed()
    {
        int cost = GetUpgradeCost((int)(gameData.permanentAtkSpeedBonus * 10));
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentAtkSpeedBonus += 0.01f; // ���� �ӵ� 1% ����
            SaveChangesAndRefreshUI();
        }
        else
        {
            Debug.Log("��ᰡ �����մϴ�.");
        }
    }

    public void OnClickUpgradeCoreHp()
    {
        int cost = GetUpgradeCost(gameData.permanentCoreHpBonus / 10);
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentCoreHpBonus += 10; // ü�� 10 ����
            SaveChangesAndRefreshUI();
        }
        else
        {
            Debug.Log("��ᰡ �����մϴ�.");
        }
    }

    private void SaveChangesAndRefreshUI()
    {
        // ����� �����͸� ���Ͽ� ����
        SaveLoadManager.instance.SaveGame(currentSaveSlot);
        // UI ���ΰ�ħ
        UpdateAllUI();
    }
}