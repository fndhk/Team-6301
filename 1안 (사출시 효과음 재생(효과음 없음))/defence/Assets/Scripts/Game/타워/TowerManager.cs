// 파일 이름: TowerManager.cs
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

    public void ApplyBuff(ItemData itemData)
    {
        if (itemData.itemType != ItemType.Buff) return;

        // 공격 속도 버프는 이제 각 타워에 개별 적용
        if (itemData.effectType == EffectType.AttackSpeed)
        {
            foreach (BaseTower tower in allTowers)
            {
                if (tower != null && tower.gameObject.activeSelf)
                {
                    tower.ApplyAttackSpeedBuff(itemData.effectValue, itemData.buffDuration);
                }
            }
        }
        // 데미지 버프는 Manager가 전체 적용
        else if (itemData.effectType == EffectType.Damage)
        {
            StopCoroutine("DamageBuffCoroutine");
            StartCoroutine(DamageBuffCoroutine(itemData.effectValue, itemData.buffDuration));
        }
    }

    private IEnumerator DamageBuffCoroutine(float multiplier, float duration)
    {
        Debug.Log("데미지 버프 시작!");
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
                tower.SetDamageMultiplier(1f); // 버프가 끝나면 배율을 1로 초기화
            }
        }
        Debug.Log("데미지 버프 종료.");
    }
}