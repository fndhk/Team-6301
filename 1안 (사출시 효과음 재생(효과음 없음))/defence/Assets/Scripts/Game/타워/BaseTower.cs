// ���� �̸�: BaseTower.cs (��ü ��ü)
using UnityEngine;

public abstract class BaseTower : MonoBehaviour
{
    [Header("���� �ɷ�ġ")]
    [SerializeField] protected float attackRange = 10f;
    [SerializeField] public int baseDamage = 25;
    [SerializeField] public float baseAttackCooldown = 1.5f; //  <-- �߰�: �⺻ ���� �ӵ� (1.5�ʿ� 1��)
    [HideInInspector] public float itemDamageMultiplier = 1f;

    [Header("���� �������")]
    [SerializeField] protected string enemyTag = "Enemy";
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform firePoint;

    private float nextAttackTime = 0f;
    protected Transform target;

    protected virtual void Start()
    {
        if (SaveLoadManager.instance != null && SaveLoadManager.instance.gameData != null)
        {
            baseDamage += SaveLoadManager.instance.gameData.permanentAtkBonus;
        }
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        if (TowerManager.instance != null)
        {
            TowerManager.instance.RegisterTower(this);
        }
    }

    protected virtual void Update()
    {
        if (target == null) return;
        if (Time.time >= nextAttackTime)
        {
            // ���� ��, ���� ���� �ð��� ���� �ð��� ��Ÿ���� ���� ���
            nextAttackTime = Time.time + baseAttackCooldown;

            // ���� ������ ��� (������ ���� ���� �״�� ����)
            int finalDamage = Mathf.RoundToInt(baseDamage * itemDamageMultiplier);
            Attack(finalDamage);
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
    /*
    public void FireProjectile(JudgmentManager.Judgment judgment)
    {
        if (target == null) return;

        float rhythmMultiplier = 1f; // �⺻ ����
        if (JudgmentManager.instance != null)
        {
            rhythmMultiplier = JudgmentManager.instance.GetDamageMultiplier(judgment);
        }
        else
        {
            Debug.LogWarning("JudgmentManager�� ���� ���� ������ ������ ������ �� �����ϴ�!");
        }

        int finalDamage = Mathf.RoundToInt(baseDamage * itemDamageMultiplier * rhythmMultiplier);
        Attack(finalDamage);
    }
    */
    public abstract void Attack(int finalDamage);

    public void SetDamageMultiplier(float multiplier)
    {
        itemDamageMultiplier = multiplier;
    }
}