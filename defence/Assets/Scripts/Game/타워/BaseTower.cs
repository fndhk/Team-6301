//파일 이름: BaseTower.cs
using UnityEngine;


public abstract class BaseTower : MonoBehaviour
{
    [Header("공통 능력치")]
    [SerializeField] protected float attackRange = 10f;
    [SerializeField] public int baseDamage = 25;
    [HideInInspector] public float itemDamageMultiplier = 1f;

    [Header("레벨")]
    // ------ 신규 수정: Inspector에서 초기 레벨 설정 가능 ------
    [Tooltip("초기 레벨 (0 = 비활성, 1~5 = 활성)")]
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

    // ------ 신규 수정: 시각적 표현을 위한 참조 ------
    private SpriteRenderer towerSpriteRenderer;

    protected virtual void Start()
    {
        // ------ 신규 수정: SpriteRenderer 찾기 ------
        towerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

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

        // ------ 신규 수정: 초기 활성화 상태 적용 ------
        UpdateActivationState();
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

            // ------ 신규 수정: 최대 레벨 6까지 허용 (스킬 사용 시) ------
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

    public abstract void Attack(int finalDamage);

    public void SetDamageMultiplier(float multiplier)
    {
        itemDamageMultiplier = multiplier;
    }

    // ------ 신규 수정: 임시 레벨 버프 적용 시 활성화 상태 업데이트 ------
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

    // ------ 신규 수정: 활성화 상태를 시각적으로 업데이트하는 함수 ------
    private void UpdateActivationState()
    {
        int currentTotalLevel = level + tempLevelBuff;

        if (currentTotalLevel <= 0)
        {
            // 0레벨 이하: 비활성화 (회색 표시)
            if (towerSpriteRenderer != null)
            {
                towerSpriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
            }
        }
        else
        {
            // 1레벨 이상: 활성화 (원래 색상)
            if (towerSpriteRenderer != null)
            {
                towerSpriteRenderer.color = Color.white;
            }
        }

        Debug.Log($"{gameObject.name} - Level: {level}, Temp: {tempLevelBuff}, Total: {currentTotalLevel}, Active: {currentTotalLevel > 0}");
    }

    // ------ 신규 수정: 외부에서 레벨을 직접 설정할 수 있는 함수 (강화 상점용) ------
    public void SetLevel(int newLevel)
    {
        // 일반 강화는 최대 5레벨까지만
        level = Mathf.Clamp(newLevel, 0, 5);
        UpdateActivationState();
    }

    // ------ 신규 수정: 현재 총 레벨 확인 함수 ------
    public int GetCurrentTotalLevel()
    {
        return level + tempLevelBuff;
    }
}