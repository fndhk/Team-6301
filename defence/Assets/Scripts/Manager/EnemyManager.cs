using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public List<Enemy> activeEnemies = new List<Enemy>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    // ▼▼▼ 신규: 가장 앞에 있는 적을 찾는 함수 ▼▼▼
    public Enemy FindFrontmostEnemy()
    {
        // 살아있는 적이 없으면 null 반환
        if (activeEnemies.Count == 0)
        {
            return null;
        }

        // Linq를 사용하여 y 좌표가 가장 낮은 적을 찾아서 반환합니다.
        Enemy frontmostEnemy = activeEnemies
            .Where(e => e != null && !e.isDead) // 살아있는 적 중에서
            .OrderBy(e => e.transform.position.y) // y좌표 오름차순으로 정렬
            .FirstOrDefault(); // 그 중 첫 번째 적

        return frontmostEnemy;
    }

    public void DamageAllEnemies(int damage)
    {
        List<Enemy> enemiesToDamage = new List<Enemy>(activeEnemies);
        foreach (Enemy enemy in enemiesToDamage)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(damage, null);
            }
        }
    }

    public void FreezeAllEnemies(float duration)
    {
        foreach (Enemy enemy in activeEnemies)
        {
            if (enemy != null)
            {
                enemy.ApplyFreeze(duration);
            }
        }
    }

    public void ApplySpeedDebuffToAll(float multiplier, float duration)
    {
        Debug.Log($"<color=purple>템포의 마법: 모든 적의 이동 속도를 {multiplier}배로 {duration}초간 변경합니다.</color>");
        List<Enemy> enemiesToDebuff = new List<Enemy>(activeEnemies);
        foreach (Enemy enemy in enemiesToDebuff)
        {
            if (enemy != null)
            {
                enemy.ApplySpeedDebuff(multiplier, duration);
            }
        }
    }

    public Enemy FindClosestEnemyInLane(Vector3 position, float laneWidth)
    {
        return activeEnemies
            .Where(e => e != null && Mathf.Abs(e.transform.position.x - position.x) < laneWidth / 2f)
            .OrderBy(e => e.transform.position.y)
            .FirstOrDefault();
    }
    // 파일 이름: EnemyManager.cs (파일 맨 아래에 추가)

    /// <summary>
    /// 현재 맵에 살아있는 적 중, 남은 체력이 가장 많은 적을 찾습니다.
    /// </summary>
    public Enemy FindHighestHealthEnemy()
    {
        if (activeEnemies.Count == 0)
        {
            return null;
        }

        // Linq를 사용하여 현재 체력(currentHealth) 기준으로 내림차순 정렬 후 첫 번째 적을 반환
        Enemy highestHealthEnemy = activeEnemies
            .Where(e => e != null && !e.isDead) // 살아있는 적 중에서
            .OrderByDescending(e => e.currentHealth) // 체력이 높은 순으로 정렬
            .FirstOrDefault(); // 그 중 첫 번째 적

        return highestHealthEnemy;
    }
}

