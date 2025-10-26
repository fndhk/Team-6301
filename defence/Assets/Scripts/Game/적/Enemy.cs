// 파일 이름: Enemy.cs (버그 수정 완료 버전)
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LootItem
{
    public ItemData itemData;
    [Range(0, 100)]
    public float dropChance;
}

public class Enemy : MonoBehaviour
{
    public static int liveEnemyCount = 0;
    [Header("기본 능력치")]
    public int scoreValue = 100;
    public float speed = 3f;
    public int maxHealth = 100;
    public int currentHealth { get; private set; }
    private float originalSpeed;

    [Header("공격 능력치")]
    public float stopYPosition = -8f;
    public int attackDamage = 10;
    public float attackRate = 1f;

    [Header("아이템 드랍")]
    public GameObject itemDropPrefab;
    public GameObject itemDropEffectPrefab;
    public List<LootItem> lootTable = new List<LootItem>();

    private bool hasReachedDestination = false;
    public bool isDead { get; private set; } = false;
    private CoreFacility coreFacility;

    private bool isFrozen = false;
    private Coroutine attackCoroutine;
    private Coroutine speedDebuffCoroutine;
    // ▼▼▼ 밀쳐내기 전용 코루틴 변수를 추가합니다 ▼▼▼
    private Coroutine pushbackCoroutine;
    private Barricade targetBarricade;
    private bool isBlockedByBarricade = false;
    private Coroutine attackBarricadeCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        originalSpeed = speed;
        coreFacility = FindFirstObjectByType<CoreFacility>();
        liveEnemyCount++;
        if (EnemyManager.instance != null)
        {
            EnemyManager.instance.RegisterEnemy(this);
        }
    }

    void OnDestroy()
    {
        if (EnemyManager.instance != null)
        {
            EnemyManager.instance.UnregisterEnemy(this);
        }
    }

    void Update()
    {
        if (isFrozen) return;
        if (hasReachedDestination || isDead || isBlockedByBarricade) return;

        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y <= stopYPosition)
        {
            hasReachedDestination = true;
            transform.position = new Vector3(transform.position.x, stopYPosition, transform.position.z);
            attackCoroutine = StartCoroutine(AttackCoroutine());
        }
    }

    IEnumerator AttackCoroutine()
    {
        while (coreFacility != null && !isDead)
        {
            coreFacility.TakeDamage(attackDamage, this);
            yield return new WaitForSeconds(attackRate);
        }
    }

    public void TakeDamage(int damage, Transform damageSource)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            isDead = true;
            if (ScoreManager.instance != null && damageSource != null)
            {
                float distance = Vector3.Distance(transform.position, damageSource.position);
                ScoreManager.instance.AddKillScore(scoreValue, distance);
            }
            Die();
        }
    }

    public void InstantKill(GameObject effectPrefab)
    {
        if (isDead) return;
        Debug.Log($"<color=red>암살!</color> {gameObject.name}을(를) 즉사시킵니다.");
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
        }
        isDead = true;
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddKillScore(scoreValue, 0);
        }
        Die();
    }

    public void ApplyFreeze(float duration)
    {
        if (!isFrozen)
        {
            StartCoroutine(FreezeCoroutine(duration));
        }
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        isFrozen = true;
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        yield return new WaitForSeconds(duration);
        isFrozen = false;
        if (hasReachedDestination)
        {
            attackCoroutine = StartCoroutine(AttackCoroutine());
        }
    }

    public void ApplySpeedDebuff(float multiplier, float duration)
    {
        if (speedDebuffCoroutine != null)
        {
            StopCoroutine(speedDebuffCoroutine);
        }
        speedDebuffCoroutine = StartCoroutine(SpeedDebuffCoroutine(multiplier, duration));
    }

    private IEnumerator SpeedDebuffCoroutine(float multiplier, float duration)
    {
        speed = originalSpeed * multiplier;
        yield return new WaitForSeconds(duration);
        speed = originalSpeed;
        speedDebuffCoroutine = null;
    }

    private void Die()
    {
        TryDropItems();
        Destroy(gameObject);
        liveEnemyCount--;
    }

    private void TryDropItems()
    {
        if (QuickSlotManager.instance == null || itemDropEffectPrefab == null || lootTable.Count == 0) return;

        float randomValue = Random.Range(0f, 100f);
        float cumulativeChance = 0f;

        foreach (var loot in lootTable)
        {
            cumulativeChance += loot.dropChance;
            if (randomValue <= cumulativeChance)
            {
                //  수정된 부분 시작 
                // 1. 이름이 "Canvas"인 게임 오브젝트를 명확하게 찾습니다.
                GameObject mainCanvas = GameObject.Find("Canvas");
                if (mainCanvas == null)
                {
                    Debug.LogError("이름이 'Canvas'인 UI 캔버스를 찾을 수 없습니다!");
                    // 캔버스를 못찾았을 경우 아이템만이라도 추가되도록 처리
                    QuickSlotManager.instance.AddItem(loot.itemData);
                    return;
                }

                // 2. 찾은 mainCanvas의 자식으로 프리팹을 생성합니다.
                GameObject effectGO = Instantiate(itemDropEffectPrefab, mainCanvas.transform);
                effectGO.transform.SetAsLastSibling();
                //  수정된 부분 끝 

                ItemDropAnimator animator = effectGO.GetComponent<ItemDropAnimator>();

                if (animator != null)
                {
                    animator.Initialize(loot.itemData, transform.position);
                }
                else
                {
                    Debug.LogError("ItemDropEffect 프리팹에 ItemDropAnimator 스크립트가 없습니다!");
                    QuickSlotManager.instance.AddItem(loot.itemData);
                    Destroy(effectGO);
                }
                return;
            }
        }
    }

    // ▼▼▼ 아래 두 함수(ApplyPushback, PushbackCoroutine)를 통째로 교체해주세요 ▼▼▼

    public void ApplyPushback(float force, float duration)
    {
        // 만약 이미 밀려나는 중이라면, 기존 효과를 중지하고 새로 시작합니다.
        if (pushbackCoroutine != null)
        {
            StopCoroutine(pushbackCoroutine);
        }
        pushbackCoroutine = StartCoroutine(PushbackCoroutine(force, duration));
    }

    private IEnumerator PushbackCoroutine(float force, float duration)
    {
        // 1. 밀려나기 전 상태 저장 및 변경
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        //  바리케이드 공격 중이었다면 중지
        if (attackBarricadeCoroutine != null)
        {
            StopCoroutine(attackBarricadeCoroutine);
            attackBarricadeCoroutine = null;
        }

        hasReachedDestination = false;
        isBlockedByBarricade = false; //  막힌 상태 해제
        targetBarricade = null;     //  타겟 해제

        // 2. 지정된 시간 동안 뒤로 밀려남
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.Translate(Vector3.up * force * Time.deltaTime, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 3. 밀려나기가 끝났음을 시스템에 알림
        pushbackCoroutine = null;
    }

    public void OnBarricadeDetected(Barricade barricade)
    {
        // 이미 다른 바리케이드를 공격 중이거나, 감지된 바리케이드가 없으면 무시
        if (isBlockedByBarricade || barricade == null) return;

        Debug.Log(gameObject.name + "가 바리케이드 감지! 공격 시작!");
        isBlockedByBarricade = true;
        targetBarricade = barricade;

        // 코어 공격 코루틴이 실행 중일 수 있으니 중지
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        // 바리케이드 공격 시작
        attackBarricadeCoroutine = StartCoroutine(AttackBarricadeCoroutine());
    }
    // 파일 이름: Enemy.cs (파일 하단에 추가)

    IEnumerator AttackBarricadeCoroutine()
    {
        // 타겟 바리케이드가 존재하고, 내가 죽지 않은 동안 반복
        while (targetBarricade != null && !isDead)
        {
            // 바리케이드의 TakeDamage 함수 호출
            targetBarricade.TakeDamage(attackDamage, this);
            yield return new WaitForSeconds(attackRate);
        }

        // 바리케이드가 파괴되면 (targetBarricade가 null이 됨)
        // 막힌 상태를 풀고 다시 아래로 이동 시작
        isBlockedByBarricade = false;
        attackBarricadeCoroutine = null;
    }
}