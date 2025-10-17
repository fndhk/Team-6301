using UnityEngine;
using UnityEngine.UI;
using System.Collections; // 코루틴을 사용하기 위해 추가

public class CoreFacility : MonoBehaviour
{
    [Header("능력치")]
    public int baseMaxHealth = 1000; // ▼▼▼ 이름 변경: maxHealth -> baseMaxHealth
    private int currentHealth;
    private int temporaryMaxHealthBonus = 0; // 임시 최대 체력 보너스

    // 현재 적용된 최대 체력 (기본값 + 임시 보너스)
    public int CurrentMaxHealth => baseMaxHealth + temporaryMaxHealthBonus;

    [Header("UI 연결")]
    public Slider healthSlider;

    void Start()
    {
        currentHealth = CurrentMaxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            FindFirstObjectByType<GameManager>().GameOver();
        }
    }

    // ▼▼▼ 신규: 체력 회복 함수 ▼▼▼
    public void Heal(int amount)
    {
        int previousHealth = currentHealth;
        currentHealth += amount;

        // 회복량이 최대 체력을 초과했는지 계산
        int overhealAmount = 0;
        if (currentHealth > CurrentMaxHealth)
        {
            overhealAmount = currentHealth - CurrentMaxHealth;
            currentHealth = CurrentMaxHealth; // 현재 체력을 최대 체력으로 제한
        }

        Debug.Log($"<color=green>코어 체력 {amount} 회복! ({previousHealth} -> {currentHealth})</color>");
        UpdateHealthUI();

        // 오버힐이 발생했다면, 최대 체력 증가 효과 발동
        if (overhealAmount > 0)
        {
            ApplyTemporaryMaxHealthBuff(overhealAmount, 10f); // 10초간 지속
        }
    }

    // ▼▼▼ 신규: 임시 최대 체력 증가 함수 ▼▼▼
    public void ApplyTemporaryMaxHealthBuff(int amount, float duration)
    {
        Debug.Log($"<color=cyan>오버힐 발생! 최대 체력이 {amount}만큼 {duration}초간 증가합니다.</color>");
        StartCoroutine(MaxHealthBuffCoroutine(amount, duration));
    }

    private IEnumerator MaxHealthBuffCoroutine(int amount, float duration)
    {
        temporaryMaxHealthBonus += amount;
        UpdateHealthUI();

        yield return new WaitForSeconds(duration);

        temporaryMaxHealthBonus -= amount;
        // 버프가 끝났을 때 현재 체력이 새 최대 체력보다 높으면 조정
        if (currentHealth > CurrentMaxHealth)
        {
            currentHealth = CurrentMaxHealth;
        }
        UpdateHealthUI();
        Debug.Log("<color=orange>최대 체력 증가 효과 종료.</color>");
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
