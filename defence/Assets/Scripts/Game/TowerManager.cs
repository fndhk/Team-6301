using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager instance; // 싱글톤 인스턴스

    // 씬에 있는 모든 타워를 저장할 리스트
    private List<Tower> allTowers = new List<Tower>();
    private bool isBuffActive = false; // 버프 중복 방지 플래그
    private bool isAttackSpeedBuffActive = false;
    private bool isDamageBuffActive = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // 타워가 생성될 때 리스트에 추가하는 함수
    public void RegisterTower(Tower tower)
    {
        if (!allTowers.Contains(tower))
        {
            allTowers.Add(tower);
        }
    }

    public void ApplyBuff(ItemData itemData)
    {
        // 아이템의 effectType에 따라 다른 처리를 함
        switch (itemData.effectType)
        {
            case EffectType.AttackSpeed:
                StartCoroutine(AttackSpeedBuffCoroutine(itemData.effectValue, itemData.buffDuration));
                break;
            case EffectType.Damage:
                StartCoroutine(DamageBuffCoroutine(itemData.effectValue, itemData.buffDuration));
                break;
        }
    }
    private IEnumerator AttackSpeedBuffCoroutine(float multiplier, float duration)
    {
        if (isAttackSpeedBuffActive) yield break; // 중복 실행 방지

        isAttackSpeedBuffActive = true;
        foreach (Tower tower in allTowers) tower.SetFireRateMultiplier(multiplier);

        yield return new WaitForSeconds(duration);

        foreach (Tower tower in allTowers) tower.SetFireRateMultiplier(1f);
        isAttackSpeedBuffActive = false;
    }
    private IEnumerator DamageBuffCoroutine(float multiplier, float duration)
    {
        if (isDamageBuffActive) yield break; // 중복 실행 방지

        isDamageBuffActive = true;
        Debug.Log($"데미지 버프 시작! ({multiplier}배, {duration}초)");

        foreach (Tower tower in allTowers)
        {
            tower.SetDamageMultiplier(multiplier);
        }

        yield return new WaitForSeconds(duration);

        foreach (Tower tower in allTowers)
        {
            tower.SetDamageMultiplier(1f);
        }

        Debug.Log("데미지 버프 종료.");
        isDamageBuffActive = false;
    }

    // 아이템 사용 시 호출될 버프 적용 함수
    public void ApplyAttackSpeedBuff(float multiplier, float duration)
    {
        // 이미 다른 공격 속도 버프가 활성화되어 있다면 중복 실행 방지
        if (isBuffActive)
        {
            Debug.Log("이미 공격 속도 버프가 활성화되어 있습니다.");
            return;
        }
        StartCoroutine(AttackSpeedBuffCoroutine(multiplier, duration));
    }
    /*
    private IEnumerator AttackSpeedBuffCoroutine(float multiplier, float duration)
    {
        isBuffActive = true;
        Debug.Log($"공격 속도 버프 시작! ({multiplier}배, {duration}초)");

        // 모든 타워의 공격 속도를 증가시킴
        foreach (Tower tower in allTowers)
        {
            tower.SetFireRateMultiplier(multiplier);
        }

        // 지정된 시간(duration)만큼 대기
        yield return new WaitForSeconds(duration);

        // 모든 타워의 공격 속도를 원래대로 되돌림
        foreach (Tower tower in allTowers)
        {
            tower.SetFireRateMultiplier(1f); // 1배속으로 되돌림
        }

        Debug.Log("공격 속도 버프 종료.");
        isBuffActive = false;
    }
    */
}