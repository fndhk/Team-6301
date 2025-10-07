using System.Collections;
using System.Collections.Generic; // List�� ����ϱ� ���� �ʼ�
using UnityEngine;

// �� �κ��� Enemy Ŭ���� �ۿ� �־ �ǰ�, �ȿ� �־ �˴ϴ�.
// �� ���̺��� Inspector���� ���� ���� ����� ���� Ŭ�����Դϴ�.
[System.Serializable]
public class LootItem
{
    public ItemData itemData;
    [Range(0, 100)]
    public float dropChance;
}


public class Enemy : MonoBehaviour
{
    [Header("�⺻ �ɷ�ġ")]
    public float speed = 3f;
    public int maxHealth = 100;
    private int currentHealth;

    [Header("���� �ɷ�ġ")]
    public float stopYPosition = -8f; // ���� �̵��� ���� Y ��ǥ
    public int attackDamage = 10;     // ���ݷ�
    public float attackRate = 1f;       // ���� �ӵ� (1�ʿ� 1��)

    [Header("������ ���")]
    public GameObject itemDropPrefab;
    public List<LootItem> lootTable = new List<LootItem>();

    // ���� ���� ������
    private bool hasReachedDestination = false;
    private CoreFacility coreFacility;

    // ���� ���� �� �� �ѹ� ȣ��˴ϴ�.
    void Start()
    {
        currentHealth = maxHealth;
        // ���ӿ� �ִ� CoreFacility�� ã�� ������ ������ �Ӵϴ�.
        coreFacility = FindObjectOfType<CoreFacility>();
    }

    // �� �����Ӹ��� ȣ��˴ϴ�.
    void Update()
    {
        // �������� �����ߴٸ� �� �̻� �̵����� �ʽ��ϴ�.
        if (hasReachedDestination)
        {
            return;
        }

        // �Ʒ� �������� ��� �̵��մϴ�.
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // ������ Y��ǥ�� �����ߴ��� Ȯ���մϴ�.
        if (transform.position.y <= stopYPosition)
        {
            hasReachedDestination = true; // ���� ���·� ����
            // ��ġ�� ��Ȯ�ϰ� �������� ������ �����մϴ�.
            transform.position = new Vector3(transform.position.x, stopYPosition, transform.position.z);
            // ������ �����մϴ�.
            StartCoroutine(AttackCoroutine());
        }
    }

    // ������ �ݺ��ϴ� �ڷ�ƾ
    IEnumerator AttackCoroutine()
    {
        // coreFacility�� �����ϴ� ���� ���� �ݺ��մϴ�.
        while (coreFacility != null)
        {
            // �ٽ� �ü��� �������� �ݴϴ�.
            coreFacility.TakeDamage(attackDamage);

            // ���� ���ݱ��� ������ �ð���ŭ ��ٸ��ϴ�.
            yield return new WaitForSeconds(attackRate);
        }
    }

    // �������� �޴� �Լ�
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // �׾��� �� ó���ϴ� �Լ�
    private void Die()
    {
        TryDropItems();
        Destroy(gameObject);
    }

    // �������� ����ϴ� �Լ�
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