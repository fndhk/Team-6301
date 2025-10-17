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

    // ▼▼▼ 신규: 모든 타워에 공격속도 버프 적용 ▼▼▼
    public void ApplyAttackSpeedBuffToAll(float multiplier, float duration)
    {
        Debug.Log($"<color=cyan>템포의 마법: 모든 타워의 공격 속도를 {multiplier}배로 {duration}초간 변경합니다.</color>");
        foreach (BaseTower tower in allTowers)
        {
            if (tower != null && tower.gameObject.activeSelf)
            {
                tower.ApplyAttackSpeedBuff(multiplier, duration);
            }
        }
    }

    public void ApplyDamageBuff(float multiplier, float duration)
    {
        StopCoroutine("DamageBuffCoroutine");
        StartCoroutine(DamageBuffCoroutine(multiplier, duration));
    }

    private IEnumerator DamageBuffCoroutine(float multiplier, float duration)
    {
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
                tower.SetDamageMultiplier(1f);
            }
        }
    }

    public void ApplyTempLevelBuffToAllTowers(int levelIncrease, float duration)
    {
        StartCoroutine(TempLevelBuffCoroutine(levelIncrease, duration));
    }

    private IEnumerator TempLevelBuffCoroutine(int levelIncrease, float duration)
    {
        foreach (BaseTower tower in allTowers)
        {
            if (tower != null) tower.ApplyTemporaryLevelBuff(levelIncrease);
        }
        yield return new WaitForSeconds(duration);
        foreach (BaseTower tower in allTowers)
        {
            if (tower != null) tower.RemoveTemporaryLevelBuff(levelIncrease);
        }
    }
}
