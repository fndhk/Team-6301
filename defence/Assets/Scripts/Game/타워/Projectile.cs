using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("능력치")]
    public float speed = 20f;
    private int damage;

    // 목표물의 '오브젝트'가 아닌 '마지막 위치'를 저장할 변수
    private Vector3 targetPosition;
    private Transform ownerTower;
    // 타워가 데미지를 설정해주는 함수
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
    public void Initialize(int newDamage, Vector3 position, Transform owner)
    {
        this.damage = newDamage;
        this.targetPosition = position;
        this.ownerTower = owner;
    }
    // 타워가 투사체를 발사할 때 목표물의 위치를 설정해주는 함수
    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    void Update()
    {
        // 목표 위치와 현재 위치 사이의 거리를 계산
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);

        // 목표 위치에 거의 도달했다면 스스로 파괴
        if (distanceToTarget < 0.1f)
        {
            Destroy(gameObject);
            return;
        }

        // 목표 위치를 향하는 방향 벡터 계산
        Vector2 dir = (targetPosition - transform.position).normalized;

        // 해당 방향으로 이동
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    // isTrigger가 켜진 Collider 2D가 다른 Collider 2D와 충돌했을 때 호출됨
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 상대방의 Tag가 "Enemy"인지 확인
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (ScoreManager.instance != null && ownerTower != null)
                {
                    float distance = Vector3.Distance(transform.position, ownerTower.position);
                    ScoreManager.instance.AddScore(enemy.scoreValue, distance);
                }
                enemy.TakeDamage(damage);
            }
            // 적과 충돌했으므로 투사체는 제거
            Destroy(gameObject);
        }
    }

}