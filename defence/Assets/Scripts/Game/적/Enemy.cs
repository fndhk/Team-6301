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
    private float originalSpeed;

    [Header("���� �ɷ�ġ")]
    public float stopYPosition = -8f;
    public int attackDamage = 10;
    public float attackRate = 1f;

    [Header("������ ���")]
    public GameObject itemDropPrefab;
    public List<LootItem> lootTable = new List<LootItem>();

    private bool hasReachedDestination = false;
    public bool isDead { get; private set; } = false; // ���� private -> public { get; private set; } ���� ���� ����
    private CoreFacility coreFacility;

    private bool isFrozen = false;
    private Coroutine attackCoroutine;
    private Coroutine speedDebuffCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        originalSpeed = speed;
        coreFacility = FindFirstObjectByType<CoreFacility>();
        liveEnemyCount++;
        if (EnemyManager.instance != null)
        {
            EnemyManager.instance.RegisterEnemy(this);
        }
    }

    void OnDestroy()
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
            attackCoroutine = StartCoroutine(AttackCoroutine());
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

    public void TakeDamage(int damage, Transform damageSource)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            isDead = true;
            if (ScoreManager.instance != null && damageSource != null)
            {
                float distance = Vector3.Distance(transform.position, damageSource.position);
                ScoreManager.instance.AddKillScore(scoreValue, distance);
            }
            Die();
        }
    }

    // ���� �ű�: ��� �Լ� ����
    public void InstantKill(GameObject effectPrefab)
    {
        if (isDead) return;

        Debug.Log($"<color=red>�ϻ�!</color> {gameObject.name}��(��) ����ŵ�ϴ�.");

        // ��� ����Ʈ ����
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
        }

        isDead = true;

        // ������ �⺻ ������ ����
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddKillScore(scoreValue, 0);
        }

        // ���� ��� ó�� ���� ȣ��
        Die();
    }

    public void ApplyFreeze(float duration)
    {
        if (!isFrozen)
        {
            StartCoroutine(FreezeCoroutine(duration));
        }
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        isFrozen = true;
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);

        yield return new WaitForSeconds(duration);

        isFrozen = false;
        if (hasReachedDestination)
        {
            attackCoroutine = StartCoroutine(AttackCoroutine());
        }
    }

    public void ApplySpeedDebuff(float multiplier, float duration)
    {
        if (speedDebuffCoroutine != null)
        {
            StopCoroutine(speedDebuffCoroutine);
        }
        speedDebuffCoroutine = StartCoroutine(SpeedDebuffCoroutine(multiplier, duration));
    }

    private IEnumerator SpeedDebuffCoroutine(float multiplier, float duration)
    {
        speed = originalSpeed * multiplier;
        yield return new WaitForSeconds(duration);
        speed = originalSpeed;
        speedDebuffCoroutine = null;
    }

    private void Die()
    {
        TryDropItems();
        Destroy(gameObject);
        liveEnemyCount--;
    }

    private void TryDropItems()
    {
        if (QuickSlotManager.instance == null || lootTable.Count == 0) return;
        float randomValue = Random.Range(0f, 100f);
        float cumulativeChance = 0f;
        foreach (var loot in lootTable)
        {
            cumulativeChance += loot.dropChance;
            if (randomValue <= cumulativeChance)
            {
                QuickSlotManager.instance.AddItem(loot.itemData);
                if (MaterialsUI.instance != null)
                {
                    MaterialsUI.instance.OnMaterialsChanged();
                }
                return;
            }
        }
    }
}

