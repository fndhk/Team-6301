using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LootItem
{
    public ItemData itemData;
    [Range(0, 100)]
    public float dropChance;
}

public class Enemy : MonoBehaviour
{
    [Header("능력치")]
    public float speed = 3f; // 적의 이동 속도
    public int maxHealth = 100; // 최대 체력
    private int currentHealth; // 현재 체력

    [Header("아이템 드랍")]
    public GameObject itemDropPrefab; // 1단계에서 만든 '범용' 아이템 드랍 프리팹
    public List<LootItem> lootTable = new List<LootItem>(); // 룻 테이블 리스트

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
        TryDropItems();
        
        Debug.Log("적이 파괴되었습니다.");
        Destroy(gameObject);
    }
    private void TryDropItems()
    {
        if (itemDropPrefab == null || lootTable.Count == 0) return;

        // 1. 0부터 100 사이의 랜덤 숫자를 단 한 번만 뽑습니다. (룰렛을 돌립니다)
        float randomValue = Random.Range(0f, 100f);

        // 누적 확률을 계산하기 위한 변수
        float cumulativeChance = 0f;

        // 2. 룻 테이블에 있는 모든 아이템을 순서대로 확인합니다.
        foreach (var loot in lootTable)
        {
            // 현재 아이템의 확률을 누적 확률에 더합니다.
            cumulativeChance += loot.dropChance;

            // 3. 뽑은 랜덤 숫자가 현재 아이템의 누적 확률 범위 안에 들어오는지 확인합니다.
            // 예: randomValue가 15이고, 첫 아이템 확률이 20%라면 -> 15 <= 20 이므로 당첨!
            if (randomValue <= cumulativeChance)
            {
                // 4. 당첨된 아이템을 생성합니다.
                GameObject droppedItemGO = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
                ItemPickup pickupScript = droppedItemGO.GetComponent<ItemPickup>();

                if (pickupScript != null)
                {
                    pickupScript.Initialize(loot.itemData);
                }

                // 5. 아이템을 하나 드랍했으므로, 더 이상 다른 아이템을 확인할 필요 없이 즉시 함수를 종료합니다.
                return;
            }
        }

        // 만약 for문이 끝날 때까지 아무 아이템도 당첨되지 않았다면, '꽝'에 해당하므로 아무 일도 일어나지 않습니다.
    }

}