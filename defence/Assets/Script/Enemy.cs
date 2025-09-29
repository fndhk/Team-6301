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
    [Header("�ɷ�ġ")]
    public float speed = 3f; // ���� �̵� �ӵ�
    public int maxHealth = 100; // �ִ� ü��
    private int currentHealth; // ���� ü��

    [Header("������ ���")]
    public GameObject itemDropPrefab; // 1�ܰ迡�� ���� '����' ������ ��� ������
    public List<LootItem> lootTable = new List<LootItem>(); // �� ���̺� ����Ʈ

    void Start()
    {
        // ���� ���� �� ���� ü���� �ִ� ü������ ����
        currentHealth = maxHealth;
    }

    void Update()
    {
        // �� �����Ӹ��� �Ʒ� ����(Vector3.down)���� �̵�
        // Time.deltaTime�� ���� ������ �ӵ��� ������� ������ �ӵ��� �����̰� ��
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    // �ܺ�(�ַ� ����ü)���� ȣ���� �Լ�
    public void TakeDamage(int damage)
    {
        // ���� ü�¿��� ��������ŭ ����
        currentHealth -= damage;

        // ü���� 0 ���ϰ� �Ǿ����� Ȯ��
        if (currentHealth <= 0)
        {
            Die(); // �״� ó�� �Լ� ȣ��
        }
    }

    private void Die()
    {
        TryDropItems();
        
        Debug.Log("���� �ı��Ǿ����ϴ�.");
        Destroy(gameObject);
    }
    private void TryDropItems()
    {
        if (itemDropPrefab == null) return;

        // �� ���̺� �ִ� ��� �����ۿ� ���� Ȯ�� ����� �õ�
        foreach (var loot in lootTable)
        {
            float randomValue = Random.Range(0f, 100f);
            if (randomValue <= loot.dropChance)
            {
                // 1. ���� �������� ����
                GameObject droppedItemGO = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
                ItemPickup pickupScript = droppedItemGO.GetComponent<ItemPickup>();

                // 2. ������ �����ۿ��� � ���������� �˷��� (Initialize �Լ� ȣ��)
                if (pickupScript != null)
                {
                    pickupScript.Initialize(loot.itemData);
                }
            }
        }
    }
}