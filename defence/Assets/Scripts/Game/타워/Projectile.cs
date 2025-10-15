// 파일 이름: Projectile.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Projectile : MonoBehaviour
{
    [Header("능력치")]
    public float speed = 20f;
    private int damage;
    private Transform ownerTower;
    private Vector3 targetPosition;

    [Header("튕기는 공격(심벌즈) 설정")]
    private int bouncesLeft = 0;
    private float bounceRange = 10f;
    private List<Transform> hitEnemies = new List<Transform>();

    // ------ 신규 추가: 범위 공격(드럼) 설정 ------
    private bool isSplashDamage = false; // 범위 공격 투사체인지 여부
    private float splashRadius = 0f; // 폭발 반경
    private GameObject splashEffectPrefab; // 폭발 이펙트

    // Initialize 함수를 여러 버전으로 만들어 다양한 타워가 사용할 수 있게 함 (오버로딩)
    public void Initialize(int newDamage, Vector3 position, Transform owner)
    {
        this.damage = newDamage;
        this.targetPosition = position;
        this.ownerTower = owner;
    }

    // 심벌즈 타워를 위한 새로운 Initialize 함수
    public void Initialize(int newDamage, Vector3 position, Transform owner, int bounces, float range)
    {
        this.damage = newDamage;
        this.targetPosition = position;
        this.ownerTower = owner;
        this.bouncesLeft = bounces;
        this.bounceRange = range;
    }

    // ------ 신규 추가: 드럼 타워를 위한 범위 공격 Initialize 함수 ------
    public void InitializeAsSplash(int newDamage, Vector3 position, Transform owner, float radius, GameObject effectPrefab)
    {
        this.damage = newDamage;
        this.targetPosition = position;
        this.ownerTower = owner;
        this.isSplashDamage = true;
        this.splashRadius = radius;
        this.splashEffectPrefab = effectPrefab;
    }

    void Start()
    {
        // 저장된 게임 데이터에서 공격 속도 보너스를 불러와 적용
        if (SaveLoadManager.instance != null && SaveLoadManager.instance.gameData != null)
        {
            float speedBonus = SaveLoadManager.instance.gameData.permanentAtkSpeedBonus;
            speed *= (1 + speedBonus);
        }
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            // ------ 신규 수정: 범위 공격 투사체라면 폭발 처리 ------
            if (isSplashDamage)
            {
                ExplodeAtTarget();
                Destroy(gameObject);
                return;
            }

            // 목표에 도달했지만, 튕길 수 있다면 주변을 탐색
            if (bouncesLeft > 0)
            {
                FindNextTarget();
            }
            else
            {
                Destroy(gameObject);
            }
            return;
        }
        Vector2 dir = (targetPosition - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // ------ 신규 수정: 범위 공격 투사체라면 여기서 폭발 ------
            if (isSplashDamage)
            {
                ExplodeAtTarget();
                Destroy(gameObject);
                return;
            }

            // 일반 단일 타겟 공격
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, ownerTower);
            }

            // 튕길 횟수가 남았다면 다음 타겟을 찾고, 없다면 소멸
            if (bouncesLeft > 0)
            {
                bouncesLeft--;
                FindNextTarget();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    // ------ 신규 추가: 착탄 지점에서 범위 공격 실행 ------
    private void ExplodeAtTarget()
    {
        // 폭발 이펙트 생성
        if (splashEffectPrefab != null)
        {
            Instantiate(splashEffectPrefab, transform.position, Quaternion.identity);
        }

        // 폭발 반경 내 모든 적에게 데미지
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, splashRadius);

        int enemiesHit = 0;
        foreach (var col in hitColliders)
        {
            if (col.CompareTag("Enemy"))
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage, ownerTower);
                    enemiesHit++;
                }
            }
        }

        Debug.Log($"<color=orange>[DrumTower 폭발]</color> 반경 {splashRadius:F1} 내 {enemiesHit}명에게 {damage} 데미지!");
    }

    private void FindNextTarget()
    {
        // 1. 주변의 모든 적을 탐색
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, bounceRange);

        // 2. 탐색된 적들 중, 아직 맞추지 않은 적들만 골라내어 거리에 따라 정렬
        Transform nearestEnemy = colliders
            .Where(col => col.CompareTag("Enemy") && !hitEnemies.Contains(col.transform))
            .OrderBy(col => Vector3.Distance(transform.position, col.transform.position))
            .Select(col => col.transform)
            .FirstOrDefault();

        // 3. 가장 가까운 새 타겟을 찾았다면 목표로 설정
        if (nearestEnemy != null)
        {
            targetPosition = nearestEnemy.position;
        }
        else // 더 이상 맞출 적이 없으면 소멸
        {
            Destroy(gameObject);
        }
    }

    // ------ 신규 추가: 에디터에서 폭발 범위 시각화 ------
    void OnDrawGizmos()
    {
        if (isSplashDamage)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // 반투명 주황색
            Gizmos.DrawSphere(transform.position, splashRadius);
        }
    }
}