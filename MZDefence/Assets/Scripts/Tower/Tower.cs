using UnityEngine;

public class Tower : MonoBehaviour
{
    public float range = 3f;       // 공격 범위
    public float fireRate = 1f;    // 공격 속도 (초당 1번)
    public GameObject bulletPrefab;
    public Transform firePoint;

    private float fireCooldown = 0f;

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        // 범위 안의 적 찾기
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = hit.transform;
                }
            }
        }

        // 가장 가까운 적 공격
        if (nearestEnemy != null && fireCooldown <= 0f)
        {
            Shoot(nearestEnemy);
            fireCooldown = fireRate;
        }
    }

    void Shoot(Transform target)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<TowerBullet>().SetTarget(target);
    }

    // 공격 범위 표시 (Scene 뷰에서만 보임)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}