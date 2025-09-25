using UnityEngine;
public class Enemy : MonoBehaviour
{
    [Header("능력치")]
    public float speed = 3f; // 적의 이동 속도
    public int maxHealth = 100; // 최대 체력
    private int currentHealth; // 현재 체력

    void Start()
    {
        // 게임 시작 시 현재 체력을 최대 체력으로 설정
        currentHealth = maxHealth;
    }

    void Update()
    {
        // 매 프레임마다 아래 방향(Vector3.down)으로 이동
        // Time.deltaTime을 곱해 프레임 속도와 상관없이 일정한 속도로 움직이게 함
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    // 외부(주로 투사체)에서 호출할 함수
    public void TakeDamage(int damage)
    {
        // 현재 체력에서 데미지만큼 감소
        currentHealth -= damage;

        // 체력이 0 이하가 되었는지 확인
        if (currentHealth <= 0)
        {
            Die(); // 죽는 처리 함수 호출
        }
    }

    private void Die()
    {
        // 여기에 죽을 때의 이펙트(폭발 등)나 사운드 재생 코드를 추가할 수 있습니다.
        Debug.Log("적이 파괴되었습니다.");

        // 이 스크립트가 붙어있는 게임 오브젝트(자기 자신)를 씬에서 제거
        Destroy(gameObject);
    }
}