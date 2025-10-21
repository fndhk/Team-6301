// 파일 이름: Barricade.cs
using UnityEngine;

public class Barricade : MonoBehaviour
{
    [Header("능력치")]
    public int maxHealth = 300;
    public int currentHealth;

    [Header("데미지 반사")]
    [Tooltip("적이 이 바리케이드를 공격할 때마다 입는 데미지")]
    public int thornsDamage = 5;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // 적(Enemy)이 이 함수를 호출할 것입니다.
    public void TakeDamage(int damage, Enemy attacker)
    {
        currentHealth -= damage;

        // 데미지 반사 (가시 효과)
        if (attacker != null && thornsDamage > 0)
        {
            // 적의 TakeDamage 함수를 역으로 호출
            attacker.TakeDamage(thornsDamage, transform);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // TODO: 여기에 파괴 이펙트나 사운드를 추가할 수 있습니다.
        Destroy(gameObject);
    }
}