using UnityEngine;
public class Enemy : MonoBehaviour
{
    [Header("�ɷ�ġ")]
    public float speed = 3f; // ���� �̵� �ӵ�
    public int maxHealth = 100; // �ִ� ü��
    private int currentHealth; // ���� ü��

    [Header("������ ���")]
    public GameObject itemPrefab; // ����� ������ ������
    [Range(0, 100)] // �ν����� â���� �����̴��� ������ �� �ְ� ��
    public float dropChance = 20f; // ������ ��� Ȯ�� (20%)

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
        TryDropItem();
        
        Debug.Log("���� �ı��Ǿ����ϴ�.");
        Destroy(gameObject);
    }
    private void TryDropItem()
    {
        if (itemPrefab == null)
        {
            return;
        }

        float randomValue = Random.Range(0f, 100f);

        // ���� ���ڰ� ������ ��� Ȯ��(dropChance)���� �۰ų� ������ ������ ����
        if (randomValue <= dropChance)
        {
            // �������� ���� ���� ��ġ�� ����
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }
    }
}