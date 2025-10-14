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
    private bool isFrozen = false;
    void Start()
    {
        currentHealth = maxHealth;
        coreFacility = FindFirstObjectByType<CoreFacility>();
        liveEnemyCount++;
        EnemyManager.instance.RegisterEnemy(this);
    }
    void OnDestroy() // <-- �Լ� �߰�
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
    public void ApplyFreeze(float duration) // <-- �Լ� �߰�
    {
        if (!isFrozen)
        {
            StartCoroutine(FreezeCoroutine(duration));
        }
    }
    private IEnumerator FreezeCoroutine(float duration) // <-- �ڷ�ƾ �߰�
    {
        isFrozen = true;
        speed = 0; // Ȥ�� �ִϸ��̼��� ���ߴ� �ڵ�
        yield return new WaitForSeconds(duration);
        isFrozen = false;
        // speed = �����ӵ�; // ���� �ӵ� ������ ���� �����صξ��ٰ� �����ؾ� �մϴ�.
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
        // QuickSlotManager�� ������ �������� �� �� �����Ƿ� �Լ� ����
        if (QuickSlotManager.instance == null || lootTable.Count == 0) return;

        float randomValue = Random.Range(0f, 100f);
        float cumulativeChance = 0f;
        foreach (var loot in lootTable)
        {
            cumulativeChance += loot.dropChance;
            if (randomValue <= cumulativeChance)
            {
                // �������� �����ϴ� ���, QuickSlotManager�� ������ ������ �ٷ� �����մϴ�.
                QuickSlotManager.instance.AddItem(loot.itemData);
                return; // �������� �ϳ��� ��ӵǹǷ� return
            }
        }
    }
}