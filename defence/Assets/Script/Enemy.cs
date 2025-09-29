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
        if (itemDropPrefab == null) return;

        // 룻 테이블에 있는 모든 아이템에 대해 확률 계산을 시도
        foreach (var loot in lootTable)
        {
            float randomValue = Random.Range(0f, 100f);
            if (randomValue <= loot.dropChance)
            {
                // 1. 범용 프리팹을 생성
                GameObject droppedItemGO = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
                ItemPickup pickupScript = droppedItemGO.GetComponent<ItemPickup>();

                // 2. 생성된 아이템에게 어떤 아이템인지 알려줌 (Initialize 함수 호출)
                if (pickupScript != null)
                {
                    pickupScript.Initialize(loot.itemData);
                }
            }
        }
    }
}