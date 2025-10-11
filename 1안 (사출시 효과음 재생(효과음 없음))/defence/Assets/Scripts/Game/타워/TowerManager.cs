// ���� �̸�: TowerManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TowerManager : MonoBehaviour
{
    public static TowerManager instance;
    private List<BaseTower> allTowers = new List<BaseTower>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void RegisterTower(BaseTower tower)
    {
        if (!allTowers.Contains(tower))
        {
            allTowers.Add(tower);
        }
    }

    

    private IEnumerator DamageBuffCoroutine(float multiplier, float duration)
    {
        Debug.Log("������ ���� ����!");
        foreach (BaseTower tower in allTowers)
        {
            if (tower != null && tower.gameObject.activeSelf)
            {
                tower.SetDamageMultiplier(multiplier);
            }
        }

        yield return new WaitForSeconds(duration);

        foreach (BaseTower tower in allTowers)
        {
            if (tower != null)
            {
                tower.SetDamageMultiplier(1f); // ������ ������ ������ 1�� �ʱ�ȭ
            }
        }
        Debug.Log("������ ���� ����.");
    }
    public void ApplyDamageBuff(float multiplier, float duration)
    {
        // ���� ������ ���� �ڷ�ƾ�� �����ϴ� ����
        StopCoroutine("DamageBuffCoroutine"); // ���� �ڷ�ƾ�� �ִٸ� ����
        StartCoroutine(DamageBuffCoroutine(multiplier, duration));
    }
    

}