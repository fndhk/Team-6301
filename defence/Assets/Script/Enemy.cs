using UnityEngine;
public class Enemy : MonoBehaviour
{
    [Header("능력치")]
    public float speed = 3f; // 적의 이동 속도
    public int maxHealth = 100; // 최대 체력
    private int currentHealth; // 현재 체력

    [Header("아이템 드랍")]
    public GameObject itemPrefab; // 드랍할 아이템 프리팹
    [Range(0, 100)] // 인스펙터 창에서 슬라이더로 조절할 수 있게 함
    public float dropChance = 20f; // 아이템 드랍 확률 (20%)

    void Start()
    {
        // 게임 시작 시 현재 체력을 최대 체력으로 설정
        currentHealth = maxHealth;
    }

    void Update()
    {
        // 매 프레임마다 아래 방향(Vector3.down)으로 이동
        // Time.deltaTime을 곱해 프레임 속도와 상관없이 일정한 속도로 움직이게 함
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    // 외부(주로 투사체)에서 호출할 함수
    public void TakeDamage(int damage)
    {
        // 현재 체력에서 데미지만큼 감소
        currentHealth -= damage;

        // 체력이 0 이하가 되었는지 확인
        if (currentHealth <= 0)
        {
            Die(); // 죽는 처리 함수 호출
        }
    }

    private void Die()
    {
        TryDropItem();
        
        Debug.Log("적이 파괴되었습니다.");
        Destroy(gameObject);
    }
    private void TryDropItem()
    {
        if (itemPrefab == null)
        {
            return;
        }

        float randomValue = Random.Range(0f, 100f);

        // 랜덤 숫자가 설정된 드랍 확률(dropChance)보다 작거나 같으면 아이템 생성
        if (randomValue <= dropChance)
        {
            // 아이템을 적이 죽은 위치에 생성
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }
    }
}