using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Ÿ�� �ɷ�ġ")]
    [SerializeField] private float attackRange = 10f; // ���� �����Ÿ�
    [SerializeField] private float fireRate = 1f; // ���� �ӵ� (�ʴ� 1ȸ)
    private float fireCountdown = 0f; // ���� ���ݱ����� �ð� ī��Ʈ�ٿ�

    [Header("Ÿ�� ���ݷ�")]
    public int baseDamage = 25;
    private int currentDamage;

    [Header("Ÿ�� �������")]
    [SerializeField] private string enemyTag = "Enemy"; // ������ ����� �±�
    // [SerializeField] private Transform partToRotate; // ȸ���� �κ� (��ž)
    [SerializeField] private GameObject projectilePrefab; // �߻��� ����ü ������
    [SerializeField] private Transform firePoint; // ����ü�� ������ ��ġ

    private Transform target; // ���� ���� ���

    private float originalFireRate; // ���� ���� �ӵ��� ������ ����

    void Start()
    {
        // 0.5�ʸ��� ���� ����� ���� ã�� UpdateTarget �Լ��� �ݺ������� ȣ��
        // �� �����Ӹ��� ã�� �ʴ� ������ ���� ����ȭ�� ����
        InvokeRepeating("UpdateTarget", 0f, 0.5f);

        originalFireRate = fireRate;
        currentDamage = baseDamage; // ���� �������� �⺻ �������� �ʱ�ȭ
        TowerManager.instance.RegisterTower(this);
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

    // TowerManager�� ȣ���� ���� �ӵ� ���� �Լ�
    public void SetFireRateMultiplier(float multiplier)
    {
        fireRate = originalFireRate * multiplier;
    }

    public void SetDamageMultiplier(float multiplier)
    {
        currentDamage = Mathf.RoundToInt(baseDamage * multiplier);
    }
    void Shoot()
    {
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        if (projectile != null && target != null)
        {
            projectile.SetDamage(currentDamage);
            // target(Transform) ��ü�� �ƴ�, target�� '��ġ(position)'�� �Ѱ���
            projectile.SetTargetPosition(target.position);
        }
        else
        {
            if (projectile == null)
                Debug.LogError("����ü �����տ� Projectile.cs ��ũ��Ʈ�� �����ϴ�!");
        }
    }
}