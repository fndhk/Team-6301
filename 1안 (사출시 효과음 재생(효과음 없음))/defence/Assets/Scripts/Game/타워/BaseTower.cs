// 파일 이름: BaseTower.cs (전체 교체)
using UnityEngine;

public abstract class BaseTower : MonoBehaviour
{
    [Header("공통 능력치")]
    [SerializeField] protected float attackRange = 10f;
    [SerializeField] public int baseDamage = 25;
    [HideInInspector] public float itemDamageMultiplier = 1f;

    [Header("공통 구성요소")]
    [SerializeField] protected string enemyTag = "Enemy";
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform firePoint;

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

    public void FireProjectile(JudgmentManager.Judgment judgment)
    {
        if (target == null) return;

        float rhythmMultiplier = 1f; // 기본 배율
        if (JudgmentManager.instance != null)
        {
            rhythmMultiplier = JudgmentManager.instance.GetDamageMultiplier(judgment);
        }
        else
        {
            Debug.LogWarning("JudgmentManager가 씬에 없어 데미지 배율을 적용할 수 없습니다!");
        }

        int finalDamage = Mathf.RoundToInt(baseDamage * itemDamageMultiplier * rhythmMultiplier);
        Attack(finalDamage);
    }

    public abstract void Attack(int finalDamage);

    public void SetDamageMultiplier(float multiplier)
    {
        itemDamageMultiplier = multiplier;
    }
}