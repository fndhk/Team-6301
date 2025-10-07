using UnityEngine;

public class TowerBullet : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 1;
    private Transform target;

    public void SetTarget(Transform enemy)
    {
        target = enemy;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // 타겟 방향으로 이동
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // 타겟과 가까워지면 충돌 처리
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}