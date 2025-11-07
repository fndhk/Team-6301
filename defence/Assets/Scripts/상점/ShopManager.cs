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
    public TextMeshProUGUI unlockTowerLevelText, unlockTowerCostText, unlockTowerBenefitText;
    public TextMeshProUGUI quickSlotLevelText, quickSlotCostText, quickSlotBenefitText;
    public TextMeshProUGUI rewardLevelText, rewardCostText, rewardBenefitText;

    [Header("업그레이드 테이블 (레벨별로 직접 설정)")]
    public List<IntUpgradeStep> attackUpgradeSteps;
    public List<FloatUpgradeStep> attackSpeedUpgradeSteps;
    public List<IntUpgradeStep> coreHpUpgradeSteps;
    [Tooltip("Size를 2로 설정하세요. Element 0=타워2 해금 비용, Element 1=타워3 해금 비용")]
    public List<IntUpgradeStep> towerUnlockSteps;
    public List<IntUpgradeStep> quickSlotUpgradeSteps;
    public List<IntUpgradeStep> clearRewardUpgradeSteps;

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
        atkLevelText.text = "Lv." + (atkLevel + 1);
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
        speedLevelText.text = "Lv." + (speedLevel + 1);
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
        coreHpLevelText.text = "Lv." + (coreHpLevel + 1);
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

        int unlockedCount = gameData.unlockedTowerCount;
        if (unlockedCount >= 3) // 이미 모든 타워 해금
        {
            unlockTowerLevelText.text = "보유 : " + (unlockedCount);
            unlockTowerCostText.text = "MAX";
            unlockTowerBenefitText.text = "(해금 완료)";
        }
        else // 다음 타워 해금 정보 표시
        {
            unlockTowerLevelText.text = "보유 : " + (unlockedCount);
            int costIndex = unlockedCount - 1; // 해금할 타워 번호 - 1 = 비용 테이블 인덱스
            if (costIndex < towerUnlockSteps.Count)
            {
                unlockTowerCostText.text = "" + towerUnlockSteps[costIndex].cost;
                unlockTowerBenefitText.text = "(타워 + 1)";
            }
            else
            {
                // 비용 테이블 설정 오류
                unlockTowerCostText.text = "Error";
                unlockTowerBenefitText.text = "(Check Table)";
            }
        }

        int quickSlotLevel = gameData.quickSlotUpgradeLevel;
        quickSlotLevelText.text = "Lv." + (quickSlotLevel + 1);
        if (quickSlotLevel < quickSlotUpgradeSteps.Count)
        {
            quickSlotCostText.text = "" + quickSlotUpgradeSteps[quickSlotLevel].cost;
            quickSlotBenefitText.text = "Lv.:" + (quickSlotLevel + 1);
        }
        else
        {
            quickSlotCostText.text = "MAX";
            quickSlotBenefitText.text = "Lv.:" + (quickSlotLevel + 1);
        }

        int rewardLevel = gameData.clearRewardBonusLevel;
        float currentBonus = rewardLevel * 0.2f; // 현재 보너스 배율 계산
        rewardLevelText.text = "Lv." + (rewardLevel + 1);
        rewardBenefitText.text = "(+" + (currentBonus * 100).ToString("F0") + "%)"; // %로 표시
        if (rewardLevel < clearRewardUpgradeSteps.Count)
        {
            rewardCostText.text = "" + clearRewardUpgradeSteps[rewardLevel].cost;
        }
        else
        {
            rewardCostText.text = "MAX";
            rewardBenefitText.text = "(+" + (currentBonus * 100).ToString("F0") + "%)"; // 최대 레벨일 때 Benefit 텍스트도 변경
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
    public void OnClickUpgradeQuickslots()
    {
        int currentLevel = gameData.quickSlotUpgradeLevel;
        if (currentLevel >= quickSlotUpgradeSteps.Count)
        {
            Debug.Log("퀵슬롯: 이미 최대 레벨입니다.");
            return;
        }

        int cost = quickSlotUpgradeSteps[currentLevel].cost;
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.quickSlotUpgradeLevel++; // 통합 레벨 1 증가
            Debug.Log($"퀵슬롯 강화 완료! 새 레벨: {gameData.quickSlotUpgradeLevel}");
            SaveChangesAndRefreshUI();
        }
        else
        {
            Debug.Log("재료가 부족합니다.");
        }
    }

    void OnClickGacha()
    {
        SceneManager.LoadScene("GachaScene");
    }

    public void OnClickUnlockTower()
    {
        int currentUnlockedCount = gameData.unlockedTowerCount;
        if (currentUnlockedCount >= 3)
        {
            Debug.Log("모든 타워가 이미 해금되었습니다.");
            return;
        }

        int costIndex = currentUnlockedCount - 1;
        if (costIndex >= towerUnlockSteps.Count)
        {
            Debug.LogError("타워 해금 비용 테이블 설정 오류!");
            return;
        }

        int cost = towerUnlockSteps[costIndex].cost;
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.unlockedTowerCount++; // 해금된 타워 수 1 증가
            Debug.Log($"타워 {gameData.unlockedTowerCount} 해금 완료!");
            SaveChangesAndRefreshUI();
        }
        else
        {
            Debug.Log("재료가 부족합니다.");
        }
    }

    public void OnClickUpgradeClearReward()
    {
        int currentLevel = gameData.clearRewardBonusLevel;
        if (currentLevel >= clearRewardUpgradeSteps.Count)
        {
            Debug.Log("클리어 골드: 이미 최대 레벨입니다.");
            return;
        }

        int cost = clearRewardUpgradeSteps[currentLevel].cost;
        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.clearRewardBonusLevel++; // 레벨 1 증가
            Debug.Log($"클리어 골드 강화 완료! 새 레벨: {gameData.clearRewardBonusLevel}");
            SaveChangesAndRefreshUI();
        }
        else
        {
            Debug.Log("재료가 부족합니다.");
        }
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