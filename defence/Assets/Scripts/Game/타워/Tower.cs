using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("타워 능력치")]
    [SerializeField] private float attackRange = 10f; // 공격 사정거리
    [SerializeField] private float fireRate = 1f; // 공격 속도 (초당 1회)
    private float fireCountdown = 0f; // 다음 공격까지의 시간 카운트다운

    [Header("타워 공격력")]
    public int baseDamage = 25;
    private int currentDamage;

    [Header("타워 구성요소")]
    [SerializeField] private string enemyTag = "Enemy"; // 공격할 대상의 태그
    // [SerializeField] private Transform partToRotate; // 회전할 부분 (포탑)
    [SerializeField] private GameObject projectilePrefab; // 발사할 투사체 프리팹
    [SerializeField] private Transform firePoint; // 투사체가 생성될 위치

    private Transform target; // 현재 공격 대상

    private float originalFireRate; // 원래 공격 속도를 저장할 변수

    void Start()
    {
        // 0.5초마다 가장 가까운 적을 찾는 UpdateTarget 함수를 반복적으로 호출
        // 매 프레임마다 찾지 않는 이유는 성능 최적화를 위함
        InvokeRepeating("UpdateTarget", 0f, 0.5f);

        originalFireRate = fireRate;
        currentDamage = baseDamage; // 현재 데미지를 기본 데미지로 초기화
        TowerManager.instance.RegisterTower(this);
    }

    void UpdateTarget()
    {
        // "Enemy" 태그를 가진 모든 게임 오브젝트를 찾음
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        // 모든 적을 순회하며 가장 가까운 적을 찾음
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        // 가장 가까운 적이 사정거리 안에 있다면 타겟으로 설정
        if (nearestEnemy != null && shortestDistance <= attackRange)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null; // 사정거리 내에 적이 없으면 타겟 해제
        }
    }

    void Update()
    {
        // 타겟이 없으면 아무것도 하지 않음
        if (target == null)
            return;

        // 타겟 조준
        // LockOnTarget();

        // 공격 카운트다운
        if (fireCountdown <= 0f)
        {
            // 공격!
            Shoot();
            // 공격 속도에 맞춰 카운트다운 초기화
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    // TowerManager가 호출할 공격 속도 변경 함수
    public void SetFireRateMultiplier(float multiplier)
    {
        fireRate = originalFireRate * multiplier;
    }

    public void SetDamageMultiplier(float multiplier)
    {
        currentDamage = Mathf.RoundToInt(baseDamage * multiplier);
    }
    void Shoot()
    {
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        if (projectile != null && target != null)
        {
            projectile.SetDamage(currentDamage);
            // target(Transform) 자체가 아닌, target의 '위치(position)'를 넘겨줌
            projectile.SetTargetPosition(target.position);
        }
        else
        {
            if (projectile == null)
                Debug.LogError("투사체 프리팹에 Projectile.cs 스크립트가 없습니다!");
        }
    }
}