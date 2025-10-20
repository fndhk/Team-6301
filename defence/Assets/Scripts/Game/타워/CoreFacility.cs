// 파일 이름: CoreFacility.cs (수정 완료 버전)
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoreFacility : MonoBehaviour
{
    [Header("능력치")]
    public int baseMaxHealth = 1000;
    private int currentHealth;
    private int temporaryMaxHealthBonus = 0;

    public int CurrentMaxHealth => baseMaxHealth + temporaryMaxHealthBonus;

    [Header("UI 연결")]
    public Slider healthSlider;

    void Start()
    {
        // ▼▼▼ 잘못된 코드를 삭제하고 이 두 줄만 남깁니다 ▼▼▼
        currentHealth = CurrentMaxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage, Enemy attacker)
    {
        // 1. FurShield 컴포넌트가 있는지 확인
        FurShield shield = GetComponent<FurShield>();

        // 2. 쉴드가 존재하고, 공격한 적이 있다면
        if (shield != null && attacker != null)
        {
            // 쉴드에게 반격하라고 명령하고, 데미지는 받지 않음
            shield.OnCoreHit(attacker);
            return; // 데미지 처리 로직을 건너뜀
        }

        // (쉴드가 없을 경우에만 아래 데미지 로직이 실행됨)
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            FindFirstObjectByType<GameManager>().GameOver();
        }
    }

    public void Heal(int amount)
    {
        int previousHealth = currentHealth;
        currentHealth += amount;

        // 현재 체력이 최대 체력을 넘지 않도록 제한합니다.
        if (currentHealth > CurrentMaxHealth)
        {
            currentHealth = CurrentMaxHealth;
        }

        Debug.Log($"<color=green>코어 체력 {amount} 회복! ({previousHealth} -> {currentHealth})</color>");
        UpdateHealthUI();
    }

    
    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            float healthPercentage = (float)currentHealth / CurrentMaxHealth;
            healthSlider.value = healthPercentage;
        }
    }

    public float GetCurrentHealthPercentage()
    {
        return (float)currentHealth / CurrentMaxHealth;
    }
}