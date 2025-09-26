using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("타워 능력치")]
    [SerializeField] private float attackRange = 10f; // 공격 사정거리
    [SerializeField] private float fireRate = 1f; // 공격 속도 (초당 1회)
    private float fireCountdown = 0f; // 다음 공격까지의 시간 카운트다운

    [Header("타워 구성요소")]
    [SerializeField] private string enemyTag = "Enemy"; // 공격할 대상의 태그
    // [SerializeField] private Transform partToRotate; // 회전할 부분 (포탑)
    [SerializeField] private GameObject projectilePrefab; // 발사할 투사체 프리팹
    [SerializeField] private Transform firePoint; // 투사체가 생성될 위치

    private Transform target; // 현재 공격 대상

    void Start()
    {
        // 0.5초마다 가장 가까운 적을 찾는 UpdateTarget 함수를 반복적으로 호출
        // 매 프레임마다 찾지 않는 이유는 성능 최적화를 위함
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
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

    /*void LockOnTarget()
    {
        // 타겟을 향하는 방향 벡터 계산
        Vector2 dir = target.position - transform.position;
        // atan2 함수와 Rad2Deg를 이용해 각도를 구함 (스프라이트가 위를 보도록 90도 빼줌)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        // Z축을 기준으로 회전
        partToRotate.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    */

    void Shoot()
    {
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            // Seek 함수 대신 SetTargetPosition 함수를 호출하고, target의 '위치'를 넘겨줍니다.
            projectile.SetTargetPosition(target.position);
        }
    }

    // Scene 뷰에서만 보이는 사정거리 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}