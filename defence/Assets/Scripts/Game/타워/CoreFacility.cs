using UnityEngine;
using UnityEngine.UI;
using System.Collections; // �ڷ�ƾ�� ����ϱ� ���� �߰�

public class CoreFacility : MonoBehaviour
{
    [Header("�ɷ�ġ")]
    public int baseMaxHealth = 1000; // ���� �̸� ����: maxHealth -> baseMaxHealth
    private int currentHealth;
    private int temporaryMaxHealthBonus = 0; // �ӽ� �ִ� ü�� ���ʽ�

    // ���� ����� �ִ� ü�� (�⺻�� + �ӽ� ���ʽ�)
    public int CurrentMaxHealth => baseMaxHealth + temporaryMaxHealthBonus;

    [Header("UI ����")]
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

    // ���� �ű�: ü�� ȸ�� �Լ� ����
    public void Heal(int amount)
    {
        int previousHealth = currentHealth;
        currentHealth += amount;

        // ȸ������ �ִ� ü���� �ʰ��ߴ��� ���
        int overhealAmount = 0;
        if (currentHealth > CurrentMaxHealth)
        {
            overhealAmount = currentHealth - CurrentMaxHealth;
            currentHealth = CurrentMaxHealth; // ���� ü���� �ִ� ü������ ����
        }

        Debug.Log($"<color=green>�ھ� ü�� {amount} ȸ��! ({previousHealth} -> {currentHealth})</color>");
        UpdateHealthUI();

        // �������� �߻��ߴٸ�, �ִ� ü�� ���� ȿ�� �ߵ�
        if (overhealAmount > 0)
        {
            ApplyTemporaryMaxHealthBuff(overhealAmount, 10f); // 10�ʰ� ����
        }
    }

    // ���� �ű�: �ӽ� �ִ� ü�� ���� �Լ� ����
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
        // ������ ������ �� ���� ü���� �� �ִ� ü�º��� ������ ����
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
