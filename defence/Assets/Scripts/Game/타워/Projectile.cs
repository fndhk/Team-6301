// ���� �̸�: Projectile.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Projectile : MonoBehaviour
{
    [Header("�ɷ�ġ")]
    public float speed = 20f;
    private int damage;
    private Transform ownerTower;
    private Vector3 targetPosition;

    [Header("ƨ��� ����(�ɹ���) ����")]
    private int bouncesLeft = 0;
    private float bounceRange = 10f;
    private List<Transform> hitEnemies = new List<Transform>();

    // ------ �ű� �߰�: ���� ����(�巳) ���� ------
    private bool isSplashDamage = false; // ���� ���� ����ü���� ����
    private float splashRadius = 0f; // ���� �ݰ�
    private GameObject splashEffectPrefab; // ���� ����Ʈ

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

    // ------ �ű� �߰�: �巳 Ÿ���� ���� ���� ���� Initialize �Լ� ------
    public void InitializeAsSplash(int newDamage, Vector3 position, Transform owner, float radius, GameObject effectPrefab)
    {
        this.damage = newDamage;
        this.targetPosition = position;
        this.ownerTower = owner;
        this.isSplashDamage = true;
        this.splashRadius = radius;
        this.splashEffectPrefab = effectPrefab;
    }

    void Start()
    {
        // ����� ���� �����Ϳ��� ���� �ӵ� ���ʽ��� �ҷ��� ����
        if (SaveLoadManager.instance != null && SaveLoadManager.instance.gameData != null)
        {
            float speedBonus = SaveLoadManager.instance.gameData.permanentAtkSpeedBonus;
            speed *= (1 + speedBonus);
        }
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            // ------ �ű� ����: ���� ���� ����ü��� ���� ó�� ------
            if (isSplashDamage)
            {
                ExplodeAtTarget();
                Destroy(gameObject);
                return;
            }

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
            // ------ �ű� ����: ���� ���� ����ü��� ���⼭ ���� ------
            if (isSplashDamage)
            {
                ExplodeAtTarget();
                Destroy(gameObject);
                return;
            }

            // �Ϲ� ���� Ÿ�� ����
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

    // ------ �ű� �߰�: ��ź �������� ���� ���� ���� ------
    private void ExplodeAtTarget()
    {
        // ���� ����Ʈ ����
        if (splashEffectPrefab != null)
        {
            Instantiate(splashEffectPrefab, transform.position, Quaternion.identity);
        }

        // ���� �ݰ� �� ��� ������ ������
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, splashRadius);

        int enemiesHit = 0;
        foreach (var col in hitColliders)
        {
            if (col.CompareTag("Enemy"))
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage, ownerTower);
                    enemiesHit++;
                }
            }
        }

        Debug.Log($"<color=orange>[DrumTower ����]</color> �ݰ� {splashRadius:F1} �� {enemiesHit}���� {damage} ������!");
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

    // ------ �ű� �߰�: �����Ϳ��� ���� ���� �ð�ȭ ------
    void OnDrawGizmos()
    {
        if (isSplashDamage)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // ������ ��Ȳ��
            Gizmos.DrawSphere(transform.position, splashRadius);
        }
    }
}