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
    private float permanentAttackSpeedBonus = 0f;

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
            permanentAttackSpeedBonus = SaveLoadManager.instance.gameData.permanentAtkSpeedBonus;
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
        if (currentTotalLevel <= 0) return;

        if (Time.time >= nextAttackTime)
        {
            // ▼▼▼ 이 부분을 아래 내용으로 교체해주세요 ▼▼▼

            // --- 1. 최종 공격 속도 계산 ---
            float totalSpeedMultiplier = characterAttackSpeedMultiplier * (1 + permanentAttackSpeedBonus);
            float finalAttackCooldown = baseAttackCooldown / totalSpeedMultiplier;

            // --- 2. 디버깅 로그 출력 (핵심!) ---
            // 콘솔 창에 모든 계산 과정을 출력하여 원인을 찾습니다.
            Debug.Log("===== 공격 속도 계산 과정 =====");
            Debug.Log("1. 기본 쿨다운: " + baseAttackCooldown);
            Debug.Log("2. 캐릭터 보너스 배율: " + characterAttackSpeedMultiplier);
            Debug.Log("3. 상점 영구 강화 보너스: " + permanentAttackSpeedBonus + " (" + (permanentAttackSpeedBonus * 100) + "%)");
            Debug.Log("4. 최종 속도 배율 (2번 * (1 + 3번)): " + totalSpeedMultiplier);
            Debug.Log("5. 최종 공격 쿨다운 (1번 / 4번): " + finalAttackCooldown);
            Debug.Log("=============================");

            // --- 3. 다음 공격 시간 설정 ---
            nextAttackTime = Time.time + finalAttackCooldown;

            // --- 4. 공격 실행 ---
            int finalDamage = Mathf.RoundToInt((baseDamage + characterDamageBonus) * itemDamageMultiplier);
            float levelMultiplier = 1f + (currentTotalLevel - 1) * 0.2f;
            int damageWithLevel = Mathf.RoundToInt(finalDamage * levelMultiplier);

            Attack(damageWithLevel);

            // ▲▲▲ 여기까지 교체 ▲▲▲
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