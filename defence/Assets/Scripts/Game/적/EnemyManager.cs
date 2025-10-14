// ���� �̸�: EnemyManager.cs (�� ����)
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

    // ��� ������ �������� �ִ� ��ų ȿ��
    public void DamageAllEnemies(int damage)
    {
        // ����Ʈ ���纻�� ����� ��ȸ (������ ������ ����)
        List<Enemy> enemiesToDamage = new List<Enemy>(activeEnemies);
        foreach (Enemy enemy in enemiesToDamage)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(damage, null); // ��ų ������ ��� �����ڸ� null�� ����
            }
        }
    }

    // ��� ���� �󸮴� ��ų ȿ��
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