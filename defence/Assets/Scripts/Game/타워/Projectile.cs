using UnityEngine;
using System.Collections.Generic; // List를 사용하기 위해 추가
using System.Linq; // Linq를 사용하기 위해 추가 (OrderBy)

public class Projectile : MonoBehaviour
{
    [Header("능력치")]
    public float speed = 20f;
    private int damage;
    private Transform ownerTower;
    private Vector3 targetPosition;

    [Header("튕기는 공격(심벌즈) 설정")]
    private int bouncesLeft = 0; // 튕길 수 있는 남은 횟수
    private float bounceRange = 10f; // 다음 타겟을 찾을 범위
    private List<Transform> hitEnemies = new List<Transform>(); // 이미 맞춘 적 목록

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
    void Start()
    {
        // 저장된 게임 데이터에서 공격 속도 보너스를 불러와 적용
        if (SaveLoadManager.instance != null && SaveLoadManager.instance.gameData != null)
        {
            float speedBonus = SaveLoadManager.instance.gameData.permanentAtkSpeedBonus;
            // 예: 보너스가 0.1 (10%) 이면, 속도가 1.1배가 됨
            speed *= (1 + speedBonus);
        }
    }
    void Update()
    {
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
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
}