using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("�ɷ�ġ")]
    public int damage = 25;
    public float speed = 20f;

    // ��ǥ���� '������Ʈ'�� �ƴ� '������ ��ġ'�� ������ ����
    private Vector3 targetPosition;

    // Ÿ���� ����ü�� �߻��� �� ��ǥ���� ��ġ�� �������ִ� �Լ�
    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    void Update()
    {
        // ��ǥ ��ġ�� ���� ��ġ ������ �Ÿ��� ���
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);

        // ��ǥ ��ġ�� ���� �����ߴٸ� (0.1f �̳�) ������ �ı�
        // �ƹ��͵� ������ ������ �� ����ü�� ������ ���ư��� ���� ����
        if (distanceToTarget < 0.1f)
        {
            Destroy(gameObject);
            return;
        }

        // ��ǥ ��ġ�� ���ϴ� ���� ���� ���
        Vector2 dir = (targetPosition - transform.position).normalized;

        // �ش� �������� �̵�
        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        // ��ǥ�� ���� ����ü�� �ٶ󺸵��� ȸ�� (���� ����)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // isTrigger�� ���� Collider 2D�� �ٸ� Collider 2D�� �浹���� �� ȣ���
    // �� �Լ��� �ٽ� ����� �̴ϴ�!
    private void OnTriggerEnter2D(Collider2D other)
    {
        // �浹�� ������ Tag�� "Enemy"���� Ȯ��
        if (other.CompareTag("Enemy"))
        {
            // ���� ������Ʈ���� Enemy ��ũ��Ʈ ������Ʈ�� ������
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // ���� TakeDamage �Լ��� ȣ���Ͽ� �������� ��
                enemy.TakeDamage(damage);
            }

            // ���� �浹�����Ƿ� ����ü(�ڱ� �ڽ�)�� ����
            Destroy(gameObject);
        }
    }
}