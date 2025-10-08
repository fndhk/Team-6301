using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("�ɷ�ġ")]
    public float speed = 20f;
    private int damage;

    // ��ǥ���� '������Ʈ'�� �ƴ� '������ ��ġ'�� ������ ����
    private Vector3 targetPosition;
    private Transform ownerTower;
    // Ÿ���� �������� �������ִ� �Լ�
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
    public void Initialize(int newDamage, Vector3 position, Transform owner)
    {
        this.damage = newDamage;
        this.targetPosition = position;
        this.ownerTower = owner;
    }
    // Ÿ���� ����ü�� �߻��� �� ��ǥ���� ��ġ�� �������ִ� �Լ�
    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    void Update()
    {
        // ��ǥ ��ġ�� ���� ��ġ ������ �Ÿ��� ���
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);

        // ��ǥ ��ġ�� ���� �����ߴٸ� ������ �ı�
        if (distanceToTarget < 0.1f)
        {
            Destroy(gameObject);
            return;
        }

        // ��ǥ ��ġ�� ���ϴ� ���� ���� ���
        Vector2 dir = (targetPosition - transform.position).normalized;

        // �ش� �������� �̵�
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    // isTrigger�� ���� Collider 2D�� �ٸ� Collider 2D�� �浹���� �� ȣ���
    private void OnTriggerEnter2D(Collider2D other)
    {
        // �浹�� ������ Tag�� "Enemy"���� Ȯ��
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (ScoreManager.instance != null && ownerTower != null)
                {
                    float distance = Vector3.Distance(transform.position, ownerTower.position);
                    ScoreManager.instance.AddScore(enemy.scoreValue, distance);
                }
                enemy.TakeDamage(damage);
            }
            // ���� �浹�����Ƿ� ����ü�� ����
            Destroy(gameObject);
        }
    }

}