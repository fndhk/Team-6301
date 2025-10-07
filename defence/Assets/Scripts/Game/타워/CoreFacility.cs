using UnityEngine;
using UnityEngine.UI; // UI(Text)�� ����ϱ� ���� �߰�

public class CoreFacility : MonoBehaviour
{
    [Header("�ɷ�ġ")]
    public int maxHealth = 1000;
    private int currentHealth;

    [Header("UI ����")]
    public Slider healthSlider; // ü�¹� �����̴��� ������ ����

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // �ܺ�(�ַ� ��)���� ȣ���� �������� �޴� �Լ�
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // ü���� 0���� �Ʒ��� �������� �ʵ��� ����
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        // ü���� 0 ���ϰ� �Ǹ� ���ӿ��� ó��
        if (currentHealth <= 0)
        {
            // GameManager�� ã�� GameOver �Լ��� ȣ��
            FindObjectOfType<GameManager>().GameOver();
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            // ���� ü���� �ִ� ü������ ������ 0�� 1 ������ ���� ���� ���
            // (float)�� �ٿ��ִ� ����: �������� ������ ����� 0 �Ǵ� 1�� ������ ���� ����
            float healthPercentage = (float)currentHealth / maxHealth;

            // �����̴��� value ���� ���� ü�� ������ ����
            healthSlider.value = healthPercentage;
        }
    }
}