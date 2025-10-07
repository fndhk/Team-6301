using System.Collections;
using System.Collections.Generic; // List를 사용하기 위해 필수
using UnityEngine;

// 이 부분은 Enemy 클래스 밖에 있어도 되고, 안에 있어도 됩니다.
// 룻 테이블을 Inspector에서 보기 좋게 만들기 위한 클래스입니다.
[System.Serializable]
public class LootItem
{
    public ItemData itemData;
    [Range(0, 100)]
    public float dropChance;
}


public class Enemy : MonoBehaviour
{
    [Header("기본 능력치")]
    public float speed = 3f;
    public int maxHealth = 100;
    private int currentHealth;

    [Header("공격 능력치")]
    public float stopYPosition = -8f; // 적이 이동을 멈출 Y 좌표
    public int attackDamage = 10;     // 공격력
    public float attackRate = 1f;       // 공격 속도 (1초에 1번)

    [Header("아이템 드랍")]
    public GameObject itemDropPrefab;
    public List<LootItem> lootTable = new List<LootItem>();

    // 내부 상태 변수들
    private bool hasReachedDestination = false;
    private CoreFacility coreFacility;

    // 게임 시작 시 단 한번 호출됩니다.
    void Start()
    {
        currentHealth = maxHealth;
        // 게임에 있는 CoreFacility를 찾아 변수에 저장해 둡니다.
        coreFacility = FindObjectOfType<CoreFacility>();
    }

    // 매 프레임마다 호출됩니다.
    void Update()
    {
        // 목적지에 도착했다면 더 이상 이동하지 않습니다.
        if (hasReachedDestination)
        {
            return;
        }

        // 아래 방향으로 계속 이동합니다.
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // 지정된 Y좌표에 도달했는지 확인합니다.
        if (transform.position.y <= stopYPosition)
        {
            hasReachedDestination = true; // 도착 상태로 변경
            // 위치를 정확하게 고정시켜 떨림을 방지합니다.
            transform.position = new Vector3(transform.position.x, stopYPosition, transform.position.z);
            // 공격을 시작합니다.
            StartCoroutine(AttackCoroutine());
        }
    }

    // 공격을 반복하는 코루틴
    IEnumerator AttackCoroutine()
    {
        // coreFacility가 존재하는 동안 무한 반복합니다.
        while (coreFacility != null)
        {
            // 핵심 시설에 데미지를 줍니다.
            coreFacility.TakeDamage(attackDamage);

            // 다음 공격까지 정해진 시간만큼 기다립니다.
            yield return new WaitForSeconds(attackRate);
        }
    }

    // 데미지를 받는 함수
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 죽었을 때 처리하는 함수
    private void Die()
    {
        TryDropItems();
        Destroy(gameObject);
    }

    // 아이템을 드랍하는 함수
    private void TryDropItems()
    {
        if (itemDropPrefab == null || lootTable.Count == 0) return;

        float randomValue = Random.Range(0f, 100f);
        float cumulativeChance = 0f;

        foreach (var loot in lootTable)
        {
            cumulativeChance += loot.dropChance;
            if (randomValue <= cumulativeChance)
            {
                GameObject droppedItemGO = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
                ItemPickup pickupScript = droppedItemGO.GetComponent<ItemPickup>();
                if (pickupScript != null)
                {
                    pickupScript.Initialize(loot.itemData);
                }
                return;
            }
        }
    }
}