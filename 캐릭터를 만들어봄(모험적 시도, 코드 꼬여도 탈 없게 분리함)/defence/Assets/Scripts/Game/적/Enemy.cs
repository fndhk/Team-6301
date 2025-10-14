// 파일 이름: Enemy.cs
using UnityEngine;
using System.Collections;
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
    public static int liveEnemyCount = 0;
    [Header("기본 능력치")]
    public int scoreValue = 100;
    public float speed = 3f;
    public int maxHealth = 100;
    private int currentHealth;

    [Header("공격 능력치")]
    public float stopYPosition = -8f;
    public int attackDamage = 10;
    public float attackRate = 1f;

    [Header("아이템 드랍")]
    public GameObject itemDropPrefab;
    public List<LootItem> lootTable = new List<LootItem>();

    private bool hasReachedDestination = false;
    private bool isDead = false; // 죽음 처리가 중복되지 않도록 방지
    private CoreFacility coreFacility;
    private bool isFrozen = false;
    void Start()
    {
        currentHealth = maxHealth;
        coreFacility = FindFirstObjectByType<CoreFacility>();
        liveEnemyCount++;
        EnemyManager.instance.RegisterEnemy(this);
    }
    void OnDestroy() // <-- 함수 추가
    {
        if (EnemyManager.instance != null)
        {
            EnemyManager.instance.UnregisterEnemy(this);
        }
    }
    void Update()
    {
        if (isFrozen) return;
        if (hasReachedDestination || isDead) return;
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y <= stopYPosition)
        {
            hasReachedDestination = true;
            transform.position = new Vector3(transform.position.x, stopYPosition, transform.position.z);
            StartCoroutine(AttackCoroutine());
        }
    }

    IEnumerator AttackCoroutine()
    {
        while (coreFacility != null && !isDead)
        {
            coreFacility.TakeDamage(attackDamage);
            yield return new WaitForSeconds(attackRate);
        }
    }

    // [수정] TakeDamage 함수가 공격자(damageSource)의 Transform을 받도록 변경
    public void TakeDamage(int damage, Transform damageSource)
    {
        if (isDead) return; // 이미 죽었다면 아무것도 하지 않음

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            isDead = true; // 죽음 상태로 변경

            // ▼▼▼▼▼ 점수 계산 로직을 이곳으로 이동 ▼▼▼▼▼
            if (ScoreManager.instance != null && damageSource != null)
            {
                float distance = Vector3.Distance(transform.position, damageSource.position);
                ScoreManager.instance.AddKillScore(scoreValue, distance);
            }

            Die();
        }
    }
    public void ApplyFreeze(float duration) // <-- 함수 추가
    {
        if (!isFrozen)
        {
            StartCoroutine(FreezeCoroutine(duration));
        }
    }
    private IEnumerator FreezeCoroutine(float duration) // <-- 코루틴 추가
    {
        isFrozen = true;
        speed = 0; // 혹은 애니메이션을 멈추는 코드
        yield return new WaitForSeconds(duration);
        isFrozen = false;
        // speed = 원래속도; // 원래 속도 변수를 따로 저장해두었다가 복원해야 합니다.
    }
    private void Die()
    {
        // 점수 계산 로직은 TakeDamage로 이동했으므로 여기서는 호출하지 않음
        TryDropItems();
        Destroy(gameObject);
        liveEnemyCount--;
    }

    private void TryDropItems()
    {
        // QuickSlotManager가 없으면 아이템을 줄 수 없으므로 함수 종료
        if (QuickSlotManager.instance == null || lootTable.Count == 0) return;

        float randomValue = Random.Range(0f, 100f);
        float cumulativeChance = 0f;
        foreach (var loot in lootTable)
        {
            cumulativeChance += loot.dropChance;
            if (randomValue <= cumulativeChance)
            {
                // 아이템을 생성하는 대신, QuickSlotManager에 아이템 정보를 바로 전달합니다.
                QuickSlotManager.instance.AddItem(loot.itemData);
                return; // 아이템은 하나만 드롭되므로 return
            }
        }
    }
}