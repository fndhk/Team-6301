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
        if (itemDropPrefab == null || lootTable.Count == 0) return;

        // 1. 0���� 100 ������ ���� ���ڸ� �� �� ���� �̽��ϴ�. (�귿�� �����ϴ�)
        float randomValue = Random.Range(0f, 100f);

        // ���� Ȯ���� ����ϱ� ���� ����
        float cumulativeChance = 0f;

        // 2. �� ���̺� �ִ� ��� �������� ������� Ȯ���մϴ�.
        foreach (var loot in lootTable)
        {
            // ���� �������� Ȯ���� ���� Ȯ���� ���մϴ�.
            cumulativeChance += loot.dropChance;

            // 3. ���� ���� ���ڰ� ���� �������� ���� Ȯ�� ���� �ȿ� �������� Ȯ���մϴ�.
            // ��: randomValue�� 15�̰�, ù ������ Ȯ���� 20%��� -> 15 <= 20 �̹Ƿ� ��÷!
            if (randomValue <= cumulativeChance)
            {
                // 4. ��÷�� �������� �����մϴ�.
                GameObject droppedItemGO = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
                ItemPickup pickupScript = droppedItemGO.GetComponent<ItemPickup>();

                if (pickupScript != null)
                {
                    pickupScript.Initialize(loot.itemData);
                }

                // 5. �������� �ϳ� ��������Ƿ�, �� �̻� �ٸ� �������� Ȯ���� �ʿ� ���� ��� �Լ��� �����մϴ�.
                return;
            }
        }

        // ���� for���� ���� ������ �ƹ� �����۵� ��÷���� �ʾҴٸ�, '��'�� �ش��ϹǷ� �ƹ� �ϵ� �Ͼ�� �ʽ��ϴ�.
    }

}