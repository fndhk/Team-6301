// 파일 이름: BaseTower.cs (수정 완료)
using UnityEngine;

public abstract class BaseTower : MonoBehaviour
{
    [Header("공통 능력치")]
    [SerializeField] protected float attackRange = 10f;
    [SerializeField] public int baseDamage = 25;
    [HideInInspector] public float itemDamageMultiplier = 1f;

    [Header("레벨")]
    public int level = 1; // 0 = 비활성, 1~5 = 활성
    private int tempLevelBuff = 0;

    [Header("공통 구성요소")]
    [SerializeField] protected string enemyTag = "Enemy";
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform firePoint;

    protected Transform target;
    private float nextAttackTime = 0f;

    // 캐릭터 능력치를 적용하기 위한 public 변수들
    [HideInInspector] public int characterDamageBonus = 0;
    [HideInInspector] public float characterAttackSpeedMultiplier = 1f;


    protected virtual void Start()
    {
        // 영구 강화 능력치 적용
        if (SaveLoadManager.instance != null && SaveLoadManager.instance.gameData != null)
        {
            baseDamage += SaveLoadManager.instance.gameData.permanentAtkBonus;
        }

        // 캐릭터 선택 능력치 적용
        if (GameSession.instance != null && GameSession.instance.selectedCharacter != null)
        {
            characterDamageBonus = GameSession.instance.selectedCharacter.towerAttackDamageBonus;
            characterAttackSpeedMultiplier = GameSession.instance.selectedCharacter.towerAttackSpeedMultiplier;
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

        int currentTotalLevel = level + tempLevelBuff;
        if (currentTotalLevel <= 0) return; // 0레벨 이하는 공격 불가

        if (Time.time >= nextAttackTime)
        {
            // 캐릭터 공속 배율 적용
            float attackCooldown = 1.5f / characterAttackSpeedMultiplier;
            nextAttackTime = Time.time + attackCooldown;

            // 캐릭터 공격력 보너스 적용
            int finalDamage = Mathf.RoundToInt((baseDamage + characterDamageBonus) * itemDamageMultiplier);

            // 레벨에 따른 데미지 계산 (예: 레벨당 20% 증가)
            float levelMultiplier = 1f + (currentTotalLevel - 1) * 0.2f;
            int damageWithLevel = Mathf.RoundToInt(finalDamage * levelMultiplier);

            Attack(damageWithLevel);
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

    // ▼▼▼ Attack 함수는 이렇게 선언만 남겨둡니다 ▼▼▼
    public abstract void Attack(int finalDamage);

    public void SetDamageMultiplier(float multiplier)
    {
        itemDamageMultiplier = multiplier;
    }

    public void ApplyTemporaryLevelBuff(int amount)
    {
        tempLevelBuff += amount;
    }

    public void RemoveTemporaryLevelBuff(int amount)
    {
        tempLevelBuff -= amount;
    }
}