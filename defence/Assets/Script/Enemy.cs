using UnityEngine;
public class Enemy : MonoBehaviour
{
    [Header("�ɷ�ġ")]
    public float speed = 3f; // ���� �̵� �ӵ�
    public int maxHealth = 100; // �ִ� ü��
    private int currentHealth; // ���� ü��

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
        // ���⿡ ���� ���� ����Ʈ(���� ��)�� ���� ��� �ڵ带 �߰��� �� �ֽ��ϴ�.
        Debug.Log("���� �ı��Ǿ����ϴ�.");

        // �� ��ũ��Ʈ�� �پ��ִ� ���� ������Ʈ(�ڱ� �ڽ�)�� ������ ����
        Destroy(gameObject);
    }
}