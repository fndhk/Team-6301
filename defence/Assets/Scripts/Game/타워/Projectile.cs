using UnityEngine;
using System.Collections.Generic; // List�� ����ϱ� ���� �߰�
using System.Linq; // Linq�� ����ϱ� ���� �߰� (OrderBy)

public class Projectile : MonoBehaviour
{
    [Header("�ɷ�ġ")]
    public float speed = 20f;
    private int damage;
    private Transform ownerTower;
    private Vector3 targetPosition;

    [Header("ƨ��� ����(�ɹ���) ����")]
    private int bouncesLeft = 0; // ƨ�� �� �ִ� ���� Ƚ��
    private float bounceRange = 10f; // ���� Ÿ���� ã�� ����
    private List<Transform> hitEnemies = new List<Transform>(); // �̹� ���� �� ���

    // Initialize �Լ��� ���� �������� ����� �پ��� Ÿ���� ����� �� �ְ� �� (�����ε�)
    public void Initialize(int newDamage, Vector3 position, Transform owner)
    {
        this.damage = newDamage;
        this.targetPosition = position;
        this.ownerTower = owner;
    }

    // �ɹ��� Ÿ���� ���� ���ο� Initialize �Լ�
    public void Initialize(int newDamage, Vector3 position, Transform owner, int bounces, float range)
    {
        this.damage = newDamage;
        this.targetPosition = position;
        this.ownerTower = owner;
        this.bouncesLeft = bounces;
        this.bounceRange = range;
    }
    void Start()
    {
        // ����� ���� �����Ϳ��� ���� �ӵ� ���ʽ��� �ҷ��� ����
        if (SaveLoadManager.instance != null && SaveLoadManager.instance.gameData != null)
        {
            float speedBonus = SaveLoadManager.instance.gameData.permanentAtkSpeedBonus;
            // ��: ���ʽ��� 0.1 (10%) �̸�, �ӵ��� 1.1�谡 ��
            speed *= (1 + speedBonus);
        }
    }
    void Update()
    {
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            // ��ǥ�� ����������, ƨ�� �� �ִٸ� �ֺ��� Ž��
            if (bouncesLeft > 0)
            {
                FindNextTarget();
            }
            else
            {
                Destroy(gameObject);
            }
            return;
        }
        Vector2 dir = (targetPosition - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, ownerTower);
            }

            // ƨ�� Ƚ���� ���Ҵٸ� ���� Ÿ���� ã��, ���ٸ� �Ҹ�
            if (bouncesLeft > 0)
            {
                bouncesLeft--;
                FindNextTarget();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void FindNextTarget()
    {
        // 1. �ֺ��� ��� ���� Ž��
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, bounceRange);

        // 2. Ž���� ���� ��, ���� ������ ���� ���鸸 ��󳻾� �Ÿ��� ���� ����
        Transform nearestEnemy = colliders
            .Where(col => col.CompareTag("Enemy") && !hitEnemies.Contains(col.transform))
            .OrderBy(col => Vector3.Distance(transform.position, col.transform.position))
            .Select(col => col.transform)
            .FirstOrDefault();

        // 3. ���� ����� �� Ÿ���� ã�Ҵٸ� ��ǥ�� ����
        if (nearestEnemy != null)
        {
            targetPosition = nearestEnemy.position;
        }
        else // �� �̻� ���� ���� ������ �Ҹ�
        {
            Destroy(gameObject);
        }
    }
}