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
    public void ApplyDamageBuff(float multiplier, float duration)
    {
        // 기존 데미지 버프 코루틴을 시작하는 로직
        StopCoroutine("DamageBuffCoroutine"); // 기존 코루틴이 있다면 중지
        StartCoroutine(DamageBuffCoroutine(multiplier, duration));
    }
    public void ApplyTempLevelBuffToAllTowers(int levelIncrease, float duration)
    {
        StartCoroutine(TempLevelBuffCoroutine(levelIncrease, duration));
    }
    private IEnumerator TempLevelBuffCoroutine(int levelIncrease, float duration)
    {
        // 모든 타워 레벨 올리기
        foreach (BaseTower tower in allTowers)
        {
            if (tower != null) tower.ApplyTemporaryLevelBuff(levelIncrease);
        }

        yield return new WaitForSeconds(duration);

        // 모든 타워 레벨 원상복구
        foreach (BaseTower tower in allTowers)
        {
            if (tower != null) tower.RemoveTemporaryLevelBuff(levelIncrease);
        }
    }
}