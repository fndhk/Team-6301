using UnityEngine;
using TMPro;
using System.Collections.Generic;

// ����(int) ���� ���� ���׷��̵�(���ݷ�, �ھ�HP)�� ���� ����ü
[System.Serializable]
public struct IntUpgradeStep
{
    public int cost;    // �� ������ ���׷��̵��ϴ� �� �ʿ��� ���
    public int benefit; // �� ���� �޼� �� �����ϴ� �ɷ�ġ
}

// �Ҽ�(float) ���� ���� ���׷��̵�(���ݼӵ�)�� ���� ����ü
[System.Serializable]
public struct FloatUpgradeStep
{
    public int cost;
    public float benefit;
}
public class ShopManager : MonoBehaviour
{
    [Header("UI ����")]
    public TextMeshProUGUI materialsText;
    public TextMeshProUGUI atkLevelText, atkCostText, atkBenefitText; // benefit �ؽ�Ʈ �߰�
    public TextMeshProUGUI speedLevelText, speedCostText, speedBenefitText;
    public TextMeshProUGUI coreHpLevelText, coreHpCostText, coreHpBenefitText;

    [Header("���׷��̵� ���̺� (�������� ���� ����)")]
    public List<IntUpgradeStep> attackUpgradeSteps;
    public List<FloatUpgradeStep> attackSpeedUpgradeSteps;
    public List<IntUpgradeStep> coreHpUpgradeSteps;

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
        materialsText.text = "Cost: " + gameData.enhancementMaterials;

        // ���ݷ� UI ������Ʈ
        int atkLevel = gameData.permanentAtkLevel;
        atkLevelText.text = "Lv. " + (atkLevel + 1);
        if (atkLevel < attackUpgradeSteps.Count)
        {
            atkCostText.text = "" + attackUpgradeSteps[atkLevel].cost;
            atkBenefitText.text = "(+ " + attackUpgradeSteps[atkLevel].benefit + ")";
        }
        else
        {
            atkCostText.text = "MAX";
            atkBenefitText.text = "(MAX)";
        }

        // ���� �ӵ� UI ������Ʈ
        int speedLevel = gameData.permanentMultiShotLevel;
        speedLevelText.text = "Lv. " + (speedLevel + 1); // ���� ǥ��
        if (speedLevel < attackSpeedUpgradeSteps.Count)
        {
            speedCostText.text = "���: " + attackSpeedUpgradeSteps[speedLevel].cost;
            // benefit �ؽ�Ʈ�� ���� �����̹Ƿ� *100�� �����մϴ�.
            speedBenefitText.text = "(+ " + attackSpeedUpgradeSteps[speedLevel].benefit + " ����)";
        }
        else { speedCostText.text = "MAX"; speedBenefitText.text = "(MAX)"; }

        // �ھ� HP UI ������Ʈ
        int coreHpLevel = gameData.permanentCoreHpLevel;
        coreHpLevelText.text = "Lv. " + (coreHpLevel + 1);
        if (coreHpLevel < coreHpUpgradeSteps.Count)
        {
            coreHpCostText.text = "" + coreHpUpgradeSteps[coreHpLevel].cost;
            coreHpBenefitText.text = "(+ " + coreHpUpgradeSteps[coreHpLevel].benefit + " HP)";
        }
        else
        {
            coreHpCostText.text = "MAX";
            coreHpBenefitText.text = "(MAX)";
        }
    }

    // --- ��ư ���� �Լ��� (���̺� ���� ������� ����) ---

    public void OnClickUpgradeAttack()
    {
        int currentLevel = gameData.permanentAtkLevel;
        if (currentLevel >= attackUpgradeSteps.Count) { Debug.Log("���ݷ�: �̹� �ִ� �����Դϴ�."); return; }

        int cost = attackUpgradeSteps[currentLevel].cost;
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentAtkBonus += attackUpgradeSteps[currentLevel].benefit;
            gameData.permanentAtkLevel++;
            SaveChangesAndRefreshUI();
        }
        else Debug.Log("��ᰡ �����մϴ�.");
    }

    public void OnClickUpgradeAttackSpeed()
    {
        Debug.Log("===== ���� �ӵ� ��ȭ ��ư Ŭ��! =====");

        int currentLevel = gameData.permanentMultiShotLevel;
        Debug.Log("���� ����: " + currentLevel);

        if (currentLevel >= attackSpeedUpgradeSteps.Count) { Debug.Log("���ݼӵ�: �̹� �ִ� �����Դϴ�."); return; }

        // ����: attackSpeedUpgradeSteps�� benefit�� ���� float�� �ƴ� int���� �մϴ�.
        // FloatUpgradeStep ����ü�� IntUpgradeStep���� �����ϰų�, benefit�� int�� ��ȯ�ؾ� �մϴ�.
        // ���⼭�� IntUpgradeStep�� ����Ѵٰ� �����մϴ�.
        int cost = attackSpeedUpgradeSteps[currentLevel].cost;
        float benefit = attackSpeedUpgradeSteps[currentLevel].benefit;
        Debug.Log($"���� ������ ���� -> ���: {cost}, ������: {benefit}");

        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentAtkSpeedBonus += benefit;
            // ���ʽ� ���� �ƴ� ���� ��ü�� 1 ������ŵ�ϴ�.
            gameData.permanentMultiShotLevel++;

            Debug.LogWarning($"��ȭ �Ϸ�! -> �� ����: {gameData.permanentAtkSpeedLevel}, �� �� ���ʽ�: {gameData.permanentAtkSpeedBonus}");
            SaveChangesAndRefreshUI();
        }
        else Debug.Log("��ᰡ �����մϴ�.");
    }

    public void OnClickUpgradeCoreHp()
    {
        int currentLevel = gameData.permanentCoreHpLevel;
        if (currentLevel >= coreHpUpgradeSteps.Count) { Debug.Log("�ھ� HP: �̹� �ִ� �����Դϴ�."); return; }

        int cost = coreHpUpgradeSteps[currentLevel].cost;
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentCoreHpBonus += coreHpUpgradeSteps[currentLevel].benefit;
            gameData.permanentCoreHpLevel++;
            SaveChangesAndRefreshUI();
        }
        else Debug.Log("��ᰡ �����մϴ�.");
    }

    private void SaveChangesAndRefreshUI()
    {
        Debug.LogError($"--- [1. ���� ����] --- ���� �ӵ� ���ʽ� '{gameData.permanentAtkSpeedBonus}' ���� �����մϴ�.");

        SaveLoadManager.instance.SaveGame(currentSaveSlot);
        UpdateAllUI();
    }

    public void OnClickBackButton()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("StageSelect");
    }
}