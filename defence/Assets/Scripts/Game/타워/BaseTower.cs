using UnityEngine;
using System.Collections;

public abstract class BaseTower : MonoBehaviour
{
    [Header("공통 능력치")]
    [SerializeField] protected float attackRange = 10f;
    [SerializeField] public int baseDamage = 25;
    [SerializeField] private float baseAttackCooldown = 1.5f;
    [HideInInspector] public float itemDamageMultiplier = 1f;

    [Header("레벨")]
    public int level = 1;
    private int tempLevelBuff = 0;

    [Header("공통 구성요소")]
    [SerializeField] protected string enemyTag = "Enemy";
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform firePoint;

    protected Transform target;
    private float nextAttackTime = 0f;

    // ▼▼▼ 스킬로 인한 공속 버프 변수 및 코루틴 참조 추가 ▼▼▼
    private float attackSpeedMultiplierFromSkill = 1f;
    private Coroutine attackSpeedBuffCoroutine;

    private SpriteRenderer towerSpriteRenderer;

    protected virtual void Start()
    {
        towerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (TowerManager.instance != null)
        {
            TowerManager.instance.RegisterTower(this);
        }
        UpdateActivationState();
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    protected virtual void Update()
    {
        if (target == null) return;
        int currentTotalLevel = level + tempLevelBuff;
        if (currentTotalLevel <= 0) return;

        if (Time.time >= nextAttackTime)
        {
            CharacterLevelGrowth totalStats = new CharacterLevelGrowth();
            if (GameSession.instance != null && GameSession.instance.selectedCharacter != null)
            {
                int charLevel = 1;
                string charID = GameSession.instance.selectedCharacter.characterID;
                if (SaveLoadManager.instance.gameData.characterLevels.ContainsKey(charID))
                {
                    charLevel = SaveLoadManager.instance.gameData.characterLevels[charID];
                }
                totalStats = GameSession.instance.selectedCharacter.GetTotalStatsForLevel(charLevel);
            }

            float permanentAtkSpeedBonus = SaveLoadManager.instance.gameData.permanentAtkSpeedBonus;

            // ▼▼▼ 최종 공격 속도 계산식에 스킬 버프(attackSpeedMultiplierFromSkill) 추가 ▼▼▼
            float totalSpeedMultiplier = totalStats.towerAttackSpeedMultiplier * (1 + permanentAtkSpeedBonus) * attackSpeedMultiplierFromSkill;
            float finalAttackCooldown = baseAttackCooldown / totalSpeedMultiplier;

            nextAttackTime = Time.time + finalAttackCooldown;

            int permanentAtkBonus = SaveLoadManager.instance.gameData.permanentAtkBonus;
            int finalDamage = Mathf.RoundToInt((baseDamage + totalStats.towerAttackDamageBonus + permanentAtkBonus) * itemDamageMultiplier);
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

    public abstract void Attack(int finalDamage);

    public void SetDamageMultiplier(float multiplier)
    {
        itemDamageMultiplier = multiplier;
    }

    // ▼▼▼ 신규: 공격속도 증가 버프 적용 함수 ▼▼▼
    public void ApplyAttackSpeedBuff(float multiplier, float duration)
    {
        if (attackSpeedBuffCoroutine != null)
        {
            StopCoroutine(attackSpeedBuffCoroutine);
        }
        attackSpeedBuffCoroutine = StartCoroutine(AttackSpeedBuffCoroutine(multiplier, duration));
    }

    private IEnumerator AttackSpeedBuffCoroutine(float multiplier, float duration)
    {
        attackSpeedMultiplierFromSkill = multiplier;
        yield return new WaitForSeconds(duration);
        attackSpeedMultiplierFromSkill = 1f; // 지속시간이 끝나면 원래 배율로 복구
        attackSpeedBuffCoroutine = null;
    }

    public void ApplyTemporaryLevelBuff(int amount)
    {
        tempLevelBuff += amount;
        UpdateActivationState();
    }

    public void RemoveTemporaryLevelBuff(int amount)
    {
        tempLevelBuff -= amount;
        UpdateActivationState();
    }

    private void UpdateActivationState()
    {
        int currentTotalLevel = level + tempLevelBuff;
        if (towerSpriteRenderer != null)
        {
            towerSpriteRenderer.color = (currentTotalLevel <= 0) ? new Color(0.3f, 0.3f, 0.3f, 0.8f) : Color.white;
        }
    }

    public void SetLevel(int newLevel)
    {
        level = Mathf.Clamp(newLevel, 0, 5);
        UpdateActivationState();
    }

    public int GetCurrentTotalLevel()
    {
        return level + tempLevelBuff;
    }
}

