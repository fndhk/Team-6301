// ���� �̸�: CoreFacility.cs (���� �Ϸ� ����)
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoreFacility : MonoBehaviour
{
    [Header("�ɷ�ġ")]
    public int baseMaxHealth = 1000;
    private int currentHealth;
    private int temporaryMaxHealthBonus = 0;

    public int CurrentMaxHealth => baseMaxHealth + temporaryMaxHealthBonus;

    [Header("UI ����")]
    public Slider healthSlider;

    void Start()
    {
        // ���� �߸��� �ڵ带 �����ϰ� �� �� �ٸ� ����ϴ� ����
        currentHealth = CurrentMaxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage, Enemy attacker)
    {
        // 1. FurShield ������Ʈ�� �ִ��� Ȯ��
        FurShield shield = GetComponent<FurShield>();

        // 2. ���尡 �����ϰ�, ������ ���� �ִٸ�
        if (shield != null && attacker != null)
        {
            // ���忡�� �ݰ��϶�� ����ϰ�, �������� ���� ����
            shield.OnCoreHit(attacker);
            return; // ������ ó�� ������ �ǳʶ�
        }

        // (���尡 ���� ��쿡�� �Ʒ� ������ ������ �����)
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

        int overhealAmount = 0;
        if (currentHealth > CurrentMaxHealth)
        {
            overhealAmount = currentHealth - CurrentMaxHealth;
            currentHealth = CurrentMaxHealth;
        }

        Debug.Log($"<color=green>�ھ� ü�� {amount} ȸ��! ({previousHealth} -> {currentHealth})</color>");
        UpdateHealthUI();

        if (overhealAmount > 0)
        {
            ApplyTemporaryMaxHealthBuff(overhealAmount, 10f);
        }
    }

    public void ApplyTemporaryMaxHealthBuff(int amount, float duration)
    {
        Debug.Log($"<color=cyan>������ �߻�! �ִ� ü���� {amount}��ŭ {duration}�ʰ� �����մϴ�.</color>");
        StartCoroutine(MaxHealthBuffCoroutine(amount, duration));
    }

    private IEnumerator MaxHealthBuffCoroutine(int amount, float duration)
    {
        temporaryMaxHealthBonus += amount;
        UpdateHealthUI();

        yield return new WaitForSeconds(duration);

        temporaryMaxHealthBonus -= amount;
        if (currentHealth > CurrentMaxHealth)
        {
            currentHealth = CurrentMaxHealth;
        }
        UpdateHealthUI();
        Debug.Log("<color=orange>�ִ� ü�� ���� ȿ�� ����.</color>");
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