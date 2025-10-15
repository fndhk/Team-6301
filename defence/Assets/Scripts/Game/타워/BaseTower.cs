//���� �̸�: BaseTower.cs
using UnityEngine;


public abstract class BaseTower : MonoBehaviour
{
    [Header("���� �ɷ�ġ")]
    [SerializeField] protected float attackRange = 10f;
    [SerializeField] public int baseDamage = 25;
    [HideInInspector] public float itemDamageMultiplier = 1f;

    [Header("����")]
    // ------ �ű� ����: Inspector���� �ʱ� ���� ���� ���� ------
    [Tooltip("�ʱ� ���� (0 = ��Ȱ��, 1~5 = Ȱ��)")]
    public int level = 1; // 0 = ��Ȱ��, 1~5 = Ȱ��
    private int tempLevelBuff = 0;

    [Header("���� �������")]
    [SerializeField] protected string enemyTag = "Enemy";
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform firePoint;

    protected Transform target;
    private float nextAttackTime = 0f;

    // ĳ���� �ɷ�ġ�� �����ϱ� ���� public ������
    [HideInInspector] public int characterDamageBonus = 0;
    [HideInInspector] public float characterAttackSpeedMultiplier = 1f;

    // ------ �ű� ����: �ð��� ǥ���� ���� ���� ------
    private SpriteRenderer towerSpriteRenderer;

    protected virtual void Start()
    {
        // ------ �ű� ����: SpriteRenderer ã�� ------
        towerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // ���� ��ȭ �ɷ�ġ ����
        if (SaveLoadManager.instance != null && SaveLoadManager.instance.gameData != null)
        {
            baseDamage += SaveLoadManager.instance.gameData.permanentAtkBonus;
        }

        // ĳ���� ���� �ɷ�ġ ����
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

        // ------ �ű� ����: �ʱ� Ȱ��ȭ ���� ���� ------
        UpdateActivationState();
    }

    protected virtual void Update()
    {
        if (target == null) return;

        int currentTotalLevel = level + tempLevelBuff;
        if (currentTotalLevel <= 0) return; // 0���� ���ϴ� ���� �Ұ�

        if (Time.time >= nextAttackTime)
        {
            // ĳ���� ���� ���� ����
            float attackCooldown = 1.5f / characterAttackSpeedMultiplier;
            nextAttackTime = Time.time + attackCooldown;

            // ĳ���� ���ݷ� ���ʽ� ����
            int finalDamage = Mathf.RoundToInt((baseDamage + characterDamageBonus) * itemDamageMultiplier);

            // ------ �ű� ����: �ִ� ���� 6���� ��� (��ų ��� ��) ------
            // ������ ���� ������ ��� (��: ������ 20% ����)
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

    // ------ �ű� ����: �ӽ� ���� ���� ���� �� Ȱ��ȭ ���� ������Ʈ ------
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

    // ------ �ű� ����: Ȱ��ȭ ���¸� �ð������� ������Ʈ�ϴ� �Լ� ------
    private void UpdateActivationState()
    {
        int currentTotalLevel = level + tempLevelBuff;

        if (currentTotalLevel <= 0)
        {
            // 0���� ����: ��Ȱ��ȭ (ȸ�� ǥ��)
            if (towerSpriteRenderer != null)
            {
                towerSpriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
            }
        }
        else
        {
            // 1���� �̻�: Ȱ��ȭ (���� ����)
            if (towerSpriteRenderer != null)
            {
                towerSpriteRenderer.color = Color.white;
            }
        }

        Debug.Log($"{gameObject.name} - Level: {level}, Temp: {tempLevelBuff}, Total: {currentTotalLevel}, Active: {currentTotalLevel > 0}");
    }

    // ------ �ű� ����: �ܺο��� ������ ���� ������ �� �ִ� �Լ� (��ȭ ������) ------
    public void SetLevel(int newLevel)
    {
        // �Ϲ� ��ȭ�� �ִ� 5����������
        level = Mathf.Clamp(newLevel, 0, 5);
        UpdateActivationState();
    }

    // ------ �ű� ����: ���� �� ���� Ȯ�� �Լ� ------
    public int GetCurrentTotalLevel()
    {
        return level + tempLevelBuff;
    }
}