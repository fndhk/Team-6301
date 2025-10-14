// 파일 이름: EnemyManager.cs (새 파일)
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    // 모든 적에게 데미지를 주는 스킬 효과
    public void DamageAllEnemies(int damage)
    {
        // 리스트 복사본을 만들어 순회 (안전한 삭제를 위해)
        List<Enemy> enemiesToDamage = new List<Enemy>(activeEnemies);
        foreach (Enemy enemy in enemiesToDamage)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(damage, null); // 스킬 공격의 경우 공격자를 null로 전달
            }
        }
    }

    // 모든 적을 얼리는 스킬 효과
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
}