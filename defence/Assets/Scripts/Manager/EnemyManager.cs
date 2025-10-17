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

    // ���� �ű�: ���� �տ� �ִ� ���� ã�� �Լ� ����
    public Enemy FindFrontmostEnemy()
    {
        // ����ִ� ���� ������ null ��ȯ
        if (activeEnemies.Count == 0)
        {
            return null;
        }

        // Linq�� ����Ͽ� y ��ǥ�� ���� ���� ���� ã�Ƽ� ��ȯ�մϴ�.
        Enemy frontmostEnemy = activeEnemies
            .Where(e => e != null && !e.isDead) // ����ִ� �� �߿���
            .OrderBy(e => e.transform.position.y) // y��ǥ ������������ ����
            .FirstOrDefault(); // �� �� ù ��° ��

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
        Debug.Log($"<color=purple>������ ����: ��� ���� �̵� �ӵ��� {multiplier}��� {duration}�ʰ� �����մϴ�.</color>");
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
}

