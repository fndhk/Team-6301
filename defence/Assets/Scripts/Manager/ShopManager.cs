using UnityEngine;
using TMPro;
using System.Collections.Generic;

// 정수(int) 값을 가진 업그레이드(공격력, 코어HP)를 위한 구조체
[System.Serializable]
public struct IntUpgradeStep
{
    public int cost;    // 이 레벨로 업그레이드하는 데 필요한 비용
    public int benefit; // 이 레벨 달성 시 증가하는 능력치
}

// 소수(float) 값을 가진 업그레이드(공격속도)를 위한 구조체
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
    public TextMeshProUGUI atkLevelText, atkCostText, atkBenefitText; // benefit 텍스트 추가
    public TextMeshProUGUI speedLevelText, speedCostText, speedBenefitText;
    public TextMeshProUGUI coreHpLevelText, coreHpCostText, coreHpBenefitText;

    [Header("업그레이드 테이블 (레벨별로 직접 설정)")]
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

        // 공격력 UI 업데이트
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

        // 공격 속도 UI 업데이트
        int speedLevel = gameData.permanentMultiShotLevel;
        speedLevelText.text = "Lv. " + (speedLevel + 1); // 레벨 표시
        if (speedLevel < attackSpeedUpgradeSteps.Count)
        {
            speedCostText.text = "비용: " + attackSpeedUpgradeSteps[speedLevel].cost;
            // benefit 텍스트는 이제 정수이므로 *100을 제거합니다.
            speedBenefitText.text = "(+ " + attackSpeedUpgradeSteps[speedLevel].benefit + " 연사)";
        }
        else { speedCostText.text = "MAX"; speedBenefitText.text = "(MAX)"; }

        // 코어 HP UI 업데이트
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

    // --- 버튼 연결 함수들 (테이블 참조 방식으로 수정) ---

    public void OnClickUpgradeAttack()
    {
        int currentLevel = gameData.permanentAtkLevel;
        if (currentLevel >= attackUpgradeSteps.Count) { Debug.Log("공격력: 이미 최대 레벨입니다."); return; }

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

        if (currentLevel >= attackSpeedUpgradeSteps.Count) { Debug.Log("공격속도: 이미 최대 레벨입니다."); return; }

        // 참고: attackSpeedUpgradeSteps의 benefit은 이제 float이 아닌 int여야 합니다.
        // FloatUpgradeStep 구조체를 IntUpgradeStep으로 변경하거나, benefit을 int로 변환해야 합니다.
        // 여기서는 IntUpgradeStep을 사용한다고 가정합니다.
        int cost = attackSpeedUpgradeSteps[currentLevel].cost;
        float benefit = attackSpeedUpgradeSteps[currentLevel].benefit;
        Debug.Log($"다음 레벨업 정보 -> 비용: {cost}, 증가량: {benefit}");

        if (gameData.enhancementMaterials >= cost)
        {
            gameData.enhancementMaterials -= cost;
            gameData.permanentAtkSpeedBonus += benefit;
            // 보너스 값이 아닌 레벨 자체를 1 증가시킵니다.
            gameData.permanentMultiShotLevel++;

            Debug.LogWarning($"강화 완료! -> 새 레벨: {gameData.permanentAtkSpeedLevel}, 새 총 보너스: {gameData.permanentAtkSpeedBonus}");
            SaveChangesAndRefreshUI();
        }
        else Debug.Log("재료가 부족합니다.");
    }

    public void OnClickUpgradeCoreHp()
    {
        int currentLevel = gameData.permanentCoreHpLevel;
        if (currentLevel >= coreHpUpgradeSteps.Count) { Debug.Log("코어 HP: 이미 최대 레벨입니다."); return; }

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

    private void SaveChangesAndRefreshUI()
    {
        Debug.LogError($"--- [1. 저장 시점] --- 공격 속도 보너스 '{gameData.permanentAtkSpeedBonus}' 값을 저장합니다.");

        SaveLoadManager.instance.SaveGame(currentSaveSlot);
        UpdateAllUI();
    }

    public void OnClickBackButton()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("StageSelect");
    }
}