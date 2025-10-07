// 파일 이름: TowerManager.cs (단순 버프 적용 버전)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager instance;

    private List<Tower> allTowers = new List<Tower>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void RegisterTower(Tower tower)
    {
        if (!allTowers.Contains(tower))
        {
            allTowers.Add(tower);
        }
    }

    // 아이템 데이터를 받아 종류에 맞는 코루틴을 실행시켜주는 역할만 함
    public void ApplyBuff(ItemData itemData)
    {
        // 동일한 타입의 기존 코루틴이 있다면 중지 (연속 클릭 시 타이머 꼬임 방지)
        if (itemData.effectType == EffectType.AttackSpeed)
        {
            StopCoroutine("AttackSpeedBuffCoroutine");
            StartCoroutine(AttackSpeedBuffCoroutine(itemData.effectValue, itemData.buffDuration));
        }
        else if (itemData.effectType == EffectType.Damage)
        {
            StopCoroutine("DamageBuffCoroutine");
            StartCoroutine(DamageBuffCoroutine(itemData.effectValue, itemData.buffDuration));
        }
    }

    // 공격 속도 버프 코루틴
    private IEnumerator AttackSpeedBuffCoroutine(float multiplier, float duration)
    {
        Debug.Log("공격 속도 버프 시작!");
        foreach (Tower tower in allTowers) if (tower != null) tower.SetFireRateMultiplier(multiplier);

        yield return new WaitForSeconds(duration);

        foreach (Tower tower in allTowers) if (tower != null) tower.SetFireRateMultiplier(1f);
        Debug.Log("공격 속도 버프 종료.");
    }

    // 데미지 버프 코루틴
    private IEnumerator DamageBuffCoroutine(float multiplier, float duration)
    {
        Debug.Log("데미지 버프 시작!");
        foreach (Tower tower in allTowers) if (tower != null) tower.SetDamageMultiplier(multiplier);

        yield return new WaitForSeconds(duration);

        foreach (Tower tower in allTowers) if (tower != null) tower.SetDamageMultiplier(1f);
        Debug.Log("데미지 버프 종료.");
    }
}