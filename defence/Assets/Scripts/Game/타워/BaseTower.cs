using UnityEngine;
using System.Collections;

public abstract class BaseTower : MonoBehaviour
{
    [Header("���� �ɷ�ġ")]
    [SerializeField] protected float attackRange = 10f;
    [SerializeField] public int baseDamage = 25;
    [SerializeField] private float baseAttackCooldown = 1.5f;
    [HideInInspector] public float itemDamageMultiplier = 1f;

    [Header("����")]
    public int level = 1;
    private int tempLevelBuff = 0;

    [Header("���� �������")]
    [SerializeField] protected string enemyTag = "Enemy";
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform firePoint;

    protected Transform target;
    private float nextAttackTime = 0f;

    // ���� ��ų�� ���� ���� ���� ���� �� �ڷ�ƾ ���� �߰� ����
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

            // 리듬 게임 정확도 가져오기 (0~100)
            float rhythmAccuracy = 100f; // 기본값
            if (ScoreManager.instance != null)
            {
                rhythmAccuracy = ScoreManager.instance.GetAverageRhythmAccuracy();
            }
            // 정확도를 30%~100% 범위로 제한 (너무 낮아지면 타워가 아예 작동 안 함 방지)
            float accuracyMultiplier = Mathf.Clamp(rhythmAccuracy / 100f, 0.3f, 1.0f);

            // ���� ���� ���� �ӵ� ���Ŀ� ��ų ����(attackSpeedMultiplierFromSkill) �߰� ����
            // 정확도가 공격속도에도 영향
            float totalSpeedMultiplier = totalStats.towerAttackSpeedMultiplier * (1 + permanentAtkSpeedBonus) * attackSpeedMultiplierFromSkill * accuracyMultiplier;
            float finalAttackCooldown = baseAttackCooldown / totalSpeedMultiplier;

            nextAttackTime = Time.time + finalAttackCooldown;

            int permanentAtkBonus = SaveLoadManager.instance.gameData.permanentAtkBonus;
            // 정확도가 공격력에도 영향
            int finalDamage = Mathf.RoundToInt((baseDamage + totalStats.towerAttackDamageBonus + permanentAtkBonus) * itemDamageMultiplier * accuracyMultiplier);
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

    // ���� �ű�: ���ݼӵ� ���� ���� ���� �Լ� ����
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
        attackSpeedMultiplierFromSkill = 1f; // ���ӽð��� ������ ���� ������ ����
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

