using UnityEngine;
using UnityEngine.UI; // UI(Text)를 사용하기 위해 추가

public class CoreFacility : MonoBehaviour
{
    [Header("능력치")]
    public int maxHealth = 1000;
    private int currentHealth;

    [Header("UI 연결")]
    public Slider healthSlider; // 체력바 슬라이더를 연결할 변수

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // 외부(주로 적)에서 호출할 데미지를 받는 함수
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // 체력이 0보다 아래로 내려가지 않도록 보정
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        // 체력이 0 이하가 되면 게임오버 처리
        if (currentHealth <= 0)
        {
            // GameManager를 찾아 GameOver 함수를 호출
            FindObjectOfType<GameManager>().GameOver();
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            // 현재 체력을 최대 체력으로 나누어 0과 1 사이의 비율 값을 계산
            // (float)를 붙여주는 이유: 정수끼리 나누면 결과가 0 또는 1만 나오는 것을 방지
            float healthPercentage = (float)currentHealth / maxHealth;

            // 슬라이더의 value 값을 계산된 체력 비율로 설정
            healthSlider.value = healthPercentage;
        }
    }
}