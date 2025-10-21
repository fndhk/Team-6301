//파일 명: ShopManager.cs
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
    [Header("UI 연결")]
    public TextMeshProUGUI materialsText;
    public TextMeshProUGUI atkLevelText, atkCostText, atkBenefitText;
    public TextMeshProUGUI speedLevelText, speedCostText, speedBenefitText;
    public TextMeshProUGUI coreHpLevelText, coreHpCostText, coreHpBenefitText;

    [Header("업그레이드 테이블 (레벨별로 직접 설정)")]
    public List<IntUpgradeStep> attackUpgradeSteps;
    public List<FloatUpgradeStep> attackSpeedUpgradeSteps;
    public List<IntUpgradeStep> coreHpUpgradeSteps;

    [Header("가챠 버튼")]
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
        materialsText.text = "소지금: " + gameData.enhancementMaterials;

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
            speedBenefitText.text = "(+ " + attackSpeedUpgradeSteps[speedLevel].benefit + " 연사)";
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
            Debug.Log("공격력: 이미 최대 레벨입니다.");
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
        else Debug.Log("재료가 부족합니다.");
    }

    public void OnClickUpgradeAttackSpeed()
    {
        Debug.Log("===== 공격 속도 강화 버튼 클릭! =====");

        int currentLevel = gameData.permanentMultiShotLevel;
        Debug.Log("현재 레벨: " + currentLevel);

        if (currentLevel >= attackSpeedUpgradeSteps.Count)
        {
            Debug.Log("공격속도: 이미 최대 레벨입니다.");
            return;
        }

        int cost = attackSpeedUpgradeSteps[currentLevel].cost;
        float benefit = attackSpeedUpgradeSteps[currentLevel].benefit;
        Debug.Log($"다음 레벨업 정보 -> 비용: {cost}, 증가량: {benefit}");

        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentAtkSpeedBonus += benefit;
            gameData.permanentMultiShotLevel++;

            Debug.LogWarning($"강화 완료! -> 새 레벨: {gameData.permanentMultiShotLevel}, 새 총 보너스: {gameData.permanentAtkSpeedBonus}");
            SaveChangesAndRefreshUI();
        }
        else Debug.Log("재료가 부족합니다.");
    }

    public void OnClickUpgradeCoreHp()
    {
        int currentLevel = gameData.permanentCoreHpLevel;
        if (currentLevel >= coreHpUpgradeSteps.Count)
        {
            Debug.Log("코어 HP: 이미 최대 레벨입니다.");
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
        else Debug.Log("재료가 부족합니다.");
    }

    void OnClickGacha()
    {
        SceneManager.LoadScene("GachaScene");
    }

    private void SaveChangesAndRefreshUI()
    {
        Debug.Log($"--- [1. 저장 시점] --- 공격 속도 보너스 '{gameData.permanentAtkSpeedBonus}' 값을 저장합니다.");
        SaveLoadManager.instance.SaveGame(currentSaveSlot);
        UpdateAllUI();
    }

    public void OnClickBackButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StageSelect");
    }
}