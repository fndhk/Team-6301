//���� ��: ShopManager.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public struct IntUpgradeStep
{
    public int cost;
    public int benefit;
}

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
    public TextMeshProUGUI atkLevelText, atkCostText, atkBenefitText;
    public TextMeshProUGUI speedLevelText, speedCostText, speedBenefitText;
    public TextMeshProUGUI coreHpLevelText, coreHpCostText, coreHpBenefitText;

    [Header("���׷��̵� ���̺� (�������� ���� ����)")]
    public List<IntUpgradeStep> attackUpgradeSteps;
    public List<FloatUpgradeStep> attackSpeedUpgradeSteps;
    public List<IntUpgradeStep> coreHpUpgradeSteps;

    [Header("��í ��ư")]
    public Button gachaButton;

    private GameData gameData;
    private int currentSaveSlot;

    void Start()
    {
        currentSaveSlot = GameSession.instance.currentSaveSlot;
        gameData = SaveLoadManager.instance.gameData;

        if (gachaButton != null)
        {
            gachaButton.onClick.AddListener(OnClickGacha);
        }

        UpdateAllUI();
    }

    void UpdateAllUI()
    {
        materialsText.text = "������: " + gameData.enhancementMaterials;

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

        int speedLevel = gameData.permanentMultiShotLevel;
        speedLevelText.text = "Lv. " + (speedLevel + 1);
        if (speedLevel < attackSpeedUpgradeSteps.Count)
        {
            speedCostText.text = "" + attackSpeedUpgradeSteps[speedLevel].cost;
            speedBenefitText.text = "(+ " + attackSpeedUpgradeSteps[speedLevel].benefit + " ����)";
        }
        else
        {
            speedCostText.text = "MAX";
            speedBenefitText.text = "(MAX)";
        }

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

    public void OnClickUpgradeAttack()
    {
        int currentLevel = gameData.permanentAtkLevel;
        if (currentLevel >= attackUpgradeSteps.Count)
        {
            Debug.Log("���ݷ�: �̹� �ִ� �����Դϴ�.");
            return;
        }

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

        if (currentLevel >= attackSpeedUpgradeSteps.Count)
        {
            Debug.Log("���ݼӵ�: �̹� �ִ� �����Դϴ�.");
            return;
        }

        int cost = attackSpeedUpgradeSteps[currentLevel].cost;
        float benefit = attackSpeedUpgradeSteps[currentLevel].benefit;
        Debug.Log($"���� ������ ���� -> ���: {cost}, ������: {benefit}");

        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentAtkSpeedBonus += benefit;
            gameData.permanentMultiShotLevel++;

            Debug.LogWarning($"��ȭ �Ϸ�! -> �� ����: {gameData.permanentMultiShotLevel}, �� �� ���ʽ�: {gameData.permanentAtkSpeedBonus}");
            SaveChangesAndRefreshUI();
        }
        else Debug.Log("��ᰡ �����մϴ�.");
    }

    public void OnClickUpgradeCoreHp()
    {
        int currentLevel = gameData.permanentCoreHpLevel;
        if (currentLevel >= coreHpUpgradeSteps.Count)
        {
            Debug.Log("�ھ� HP: �̹� �ִ� �����Դϴ�.");
            return;
        }

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

    void OnClickGacha()
    {
        SceneManager.LoadScene("GachaScene");
    }

    private void SaveChangesAndRefreshUI()
    {
        Debug.Log($"--- [1. ���� ����] --- ���� �ӵ� ���ʽ� '{gameData.permanentAtkSpeedBonus}' ���� �����մϴ�.");
        SaveLoadManager.instance.SaveGame(currentSaveSlot);
        UpdateAllUI();
    }

    public void OnClickBackButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StageSelect");
    }
}