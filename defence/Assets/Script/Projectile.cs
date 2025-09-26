using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("능력치")]
    public int damage = 25;
    public float speed = 20f;

    // 목표물의 '오브젝트'가 아닌 '마지막 위치'를 저장할 변수
    private Vector3 targetPosition;

    // 타워가 투사체를 발사할 때 목표물의 위치를 설정해주는 함수
    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    void Update()
    {
        // 목표 위치와 현재 위치 사이의 거리를 계산
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);

        // 목표 위치에 거의 도달했다면 (0.1f 이내) 스스로 파괴
        // 아무것도 맞히지 못했을 때 투사체가 영원히 날아가는 것을 방지
        if (distanceToTarget < 0.1f)
        {
            Destroy(gameObject);
            return;
        }

        // 목표 위치를 향하는 방향 벡터 계산
        Vector2 dir = (targetPosition - transform.position).normalized;

        // 해당 방향으로 이동
        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        // 목표를 향해 투사체가 바라보도록 회전 (선택 사항)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // isTrigger가 켜진 Collider 2D가 다른 Collider 2D와 충돌했을 때 호출됨
    // 이 함수를 다시 사용할 겁니다!
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 상대방의 Tag가 "Enemy"인지 확인
        if (other.CompareTag("Enemy"))
        {
            // 상대방 오브젝트에서 Enemy 스크립트 컴포넌트를 가져옴
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 적의 TakeDamage 함수를 호출하여 데미지를 줌
                enemy.TakeDamage(damage);
            }

            // 적과 충돌했으므로 투사체(자기 자신)는 제거
            Destroy(gameObject);
        }
    }
}