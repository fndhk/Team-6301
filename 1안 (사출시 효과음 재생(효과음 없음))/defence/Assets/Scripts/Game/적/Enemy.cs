// ���� �̸�: Enemy.cs
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
    [Header("�⺻ �ɷ�ġ")]
    public int scoreValue = 100;
    public float speed = 3f;
    public int maxHealth = 100;
    private int currentHealth;

    [Header("���� �ɷ�ġ")]
    public float stopYPosition = -8f;
    public int attackDamage = 10;
    public float attackRate = 1f;

    [Header("������ ���")]
    public GameObject itemDropPrefab;
    public List<LootItem> lootTable = new List<LootItem>();

    private bool hasReachedDestination = false;
    private bool isDead = false; // ���� ó���� �ߺ����� �ʵ��� ����
    private CoreFacility coreFacility;

    void Start()
    {
        currentHealth = maxHealth;
        coreFacility = FindFirstObjectByType<CoreFacility>();
        liveEnemyCount++;
    }

    void Update()
    {
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

    // [����] TakeDamage �Լ��� ������(damageSource)�� Transform�� �޵��� ����
    public void TakeDamage(int damage, Transform damageSource)
    {
        if (isDead) return; // �̹� �׾��ٸ� �ƹ��͵� ���� ����

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            isDead = true; // ���� ���·� ����

            // ������ ���� ��� ������ �̰����� �̵� ������
            if (ScoreManager.instance != null && damageSource != null)
            {
                float distance = Vector3.Distance(transform.position, damageSource.position);
                ScoreManager.instance.AddKillScore(scoreValue, distance);
            }

            Die();
        }
    }

    private void Die()
    {
        // ���� ��� ������ TakeDamage�� �̵������Ƿ� ���⼭�� ȣ������ ����
        TryDropItems();
        Destroy(gameObject);
        liveEnemyCount--;
    }

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