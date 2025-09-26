using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHp = 3;//적 최대 체력
    private int currentHp;//적 현재 체력

    public float speed = 2f; //적 속도설정

    void Start()
    {
        currentHp = maxHp;
    }

    void Update()
    {
        //화면 넘어가면 사라짐
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            Destroy(gameObject);
        }
    }
}