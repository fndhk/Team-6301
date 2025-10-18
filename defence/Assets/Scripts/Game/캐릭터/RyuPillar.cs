// 파일 이름: RyuPillar.cs
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

// ▼▼▼ 클래스 이름을 RyuPillar로 변경 ▼▼▼
public class RyuPillar : MonoBehaviour
{
    private int damagePerTick;
    private float tickInterval = 0.5f;
    private List<Enemy> damagedEnemiesThisTick = new List<Enemy>();

    public void Initialize(int dps, float duration)
    {
        this.damagePerTick = Mathf.RoundToInt(dps * tickInterval);
        Destroy(gameObject, duration);
        StartCoroutine(DamageTickCoroutine());
    }

    private IEnumerator DamageTickCoroutine()
    {
        while (true)
        {
            damagedEnemiesThisTick.Clear();
            yield return new WaitForSeconds(tickInterval);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !damagedEnemiesThisTick.Contains(enemy))
            {
                enemy.TakeDamage(damagePerTick, transform);
                damagedEnemiesThisTick.Add(enemy);
            }
        }
    }
}