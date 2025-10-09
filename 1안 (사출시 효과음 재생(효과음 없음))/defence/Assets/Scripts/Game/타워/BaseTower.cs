// ���� �̸�: BaseTower.cs
using UnityEngine;
using System.Collections;

public abstract class BaseTower : MonoBehaviour
{
    [Header("���� �ɷ�ġ")]
    [SerializeField] protected float attackRange = 10f;
    [SerializeField] public int baseDamage = 25;
    [SerializeField] private float fireRate = 1f;

    // ������ ������ ���� ������ ������ ���� �߰� ������
    [HideInInspector] public float itemDamageMultiplier = 1f;

    [Header("���� �������")]
    [SerializeField] protected string enemyTag = "Enemy";
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform firePoint;

    protected Transform target;
    private float fireCountdown = 0f;
    private float originalFireRate;

    protected virtual void Start()
    {
        originalFireRate = fireRate;
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        if (TowerManager.instance != null)
        {
            TowerManager.instance.RegisterTower(this);
        }
    }

    protected virtual void Update()
    {
        if (target == null)
        {
            fireCountdown = 0f;
            return;
        }

        fireCountdown -= Time.deltaTime;

        if (fireCountdown <= 0f)
        {
            fireCountdown = 1f / fireRate;
            if (JudgmentManager.instance != null)
            {
                JudgmentManager.instance.ProcessAttack(this, Time.time);
            }
        }
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }
        if (nearestEnemy != null && shortestDistance <= attackRange)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    public abstract void Attack(int finalDamage);

    // ������ SetDamageMultiplier �Լ��� �ٽ� �߰��մϴ� ������
    public void SetDamageMultiplier(float multiplier)
    {
        itemDamageMultiplier = multiplier;
    }

    public void ApplyAttackSpeedBuff(float multiplier, float duration)
    {
        StartCoroutine(AttackSpeedBuffCoroutine(multiplier, duration));
    }

    private IEnumerator AttackSpeedBuffCoroutine(float multiplier, float duration)
    {
        fireRate = originalFireRate * multiplier;
        yield return new WaitForSeconds(duration);
        fireRate = originalFireRate;
    }
}