using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Ÿ�� �ɷ�ġ")]
    [SerializeField] private float attackRange = 10f; // ���� �����Ÿ�
    [SerializeField] private float fireRate = 1f; // ���� �ӵ� (�ʴ� 1ȸ)
    private float fireCountdown = 0f; // ���� ���ݱ����� �ð� ī��Ʈ�ٿ�

    [Header("Ÿ�� �������")]
    [SerializeField] private string enemyTag = "Enemy"; // ������ ����� �±�
    // [SerializeField] private Transform partToRotate; // ȸ���� �κ� (��ž)
    [SerializeField] private GameObject projectilePrefab; // �߻��� ����ü ������
    [SerializeField] private Transform firePoint; // ����ü�� ������ ��ġ

    private Transform target; // ���� ���� ���

    void Start()
    {
        // 0.5�ʸ��� ���� ����� ���� ã�� UpdateTarget �Լ��� �ݺ������� ȣ��
        // �� �����Ӹ��� ã�� �ʴ� ������ ���� ����ȭ�� ����
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    void UpdateTarget()
    {
        // "Enemy" �±׸� ���� ��� ���� ������Ʈ�� ã��
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        // ��� ���� ��ȸ�ϸ� ���� ����� ���� ã��
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        // ���� ����� ���� �����Ÿ� �ȿ� �ִٸ� Ÿ������ ����
        if (nearestEnemy != null && shortestDistance <= attackRange)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null; // �����Ÿ� ���� ���� ������ Ÿ�� ����
        }
    }

    void Update()
    {
        // Ÿ���� ������ �ƹ��͵� ���� ����
        if (target == null)
            return;

        // Ÿ�� ����
        // LockOnTarget();

        // ���� ī��Ʈ�ٿ�
        if (fireCountdown <= 0f)
        {
            // ����!
            Shoot();
            // ���� �ӵ��� ���� ī��Ʈ�ٿ� �ʱ�ȭ
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    /*void LockOnTarget()
    {
        // Ÿ���� ���ϴ� ���� ���� ���
        Vector2 dir = target.position - transform.position;
        // atan2 �Լ��� Rad2Deg�� �̿��� ������ ���� (��������Ʈ�� ���� ������ 90�� ����)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        // Z���� �������� ȸ��
        partToRotate.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    */

    void Shoot()
    {
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            // Seek �Լ� ��� SetTargetPosition �Լ��� ȣ���ϰ�, target�� '��ġ'�� �Ѱ��ݴϴ�.
            projectile.SetTargetPosition(target.position);
        }
    }

    // Scene �信���� ���̴� �����Ÿ� �ð�ȭ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}