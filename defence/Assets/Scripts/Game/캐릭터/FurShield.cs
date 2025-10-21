// 파일 이름: FurShield.cs
using UnityEngine;

public class FurShield : MonoBehaviour
{
    private float gaugeCostPerHit;
    private float pushbackForce;
    private float pushbackDuration;

    // 스킬 데이터를 받아와서 초기화하는 함수
    public void Initialize(float cost, float force, float duration)
    {
        this.gaugeCostPerHit = cost;
        this.pushbackForce = force;
        this.pushbackDuration = duration;
    }

    void Update()
    {
        // 매 프레임 스킬 게이지를 확인
        if (SkillManager.instance.GetCurrentGauge() <= 0)
        {
            Debug.Log("<color=gray>털 갑옷 비활성화.</color> 게이지를 모두 소모했습니다.");
            // 게이지가 0이 되면 자신(이 컴포넌트)을 파괴하여 스킬을 종료
            Destroy(this);
        }
    }

    /// <summary>
    /// CoreFacility가 공격받았을 때 호출될 함수
    /// </summary>
    public void OnCoreHit(Enemy attacker)
    {
        // 1. 공격한 적에게 밀치기 효과 적용
        attacker.ApplyPushback(pushbackForce, pushbackDuration);

        // 2. SkillManager의 게이지 소모
        SkillManager.instance.ConsumeGauge(gaugeCostPerHit);
    }
}