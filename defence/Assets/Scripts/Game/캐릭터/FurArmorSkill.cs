// 파일 이름: FurArmorSkill.cs
using UnityEngine;
using static UnityEngine.Android.AndroidGame;

[CreateAssetMenu(fileName = "FurArmorSkill", menuName = "TowerDefense/Skills/Fur Armor Skill")]
public class FurArmorSkill : ItemEffect
{
    [Header("스킬 계수")]
    [Tooltip("적이 공격할 때마다 소모되는 스킬 게이지 양")]
    public float gaugeCostPerHit = 5f;
    [Tooltip("적을 밀어내는 힘 (거리)")]
    public float pushbackForce = 2f;
    [Tooltip("적이 밀려나는 시간 (초)")]
    public float pushbackDuration = 0.2f;

    public FurArmorSkill()
    {
        // 이 스킬은 게이지가 꽉 차면 자동으로 발동되도록 설정
        isAutomatic = true;
    }

    public override void ExecuteEffect()
    {
        // 1. CoreFacility 오브젝트를 찾습니다.
        CoreFacility core = FindFirstObjectByType<CoreFacility>();
        if (core == null)
        {
            Debug.LogError("FurArmorSkill: 씬에서 CoreFacility를 찾을 수 없습니다!");
            return;
        }

        // 2. CoreFacility에 FurShield 컴포넌트가 이미 있는지 확인합니다.
        if (core.GetComponent<FurShield>() != null)
        {
            // 이미 스킬이 활성화 상태이므로 아무것도 하지 않음
            return;
        }

        // 3. CoreFacility에 FurShield 컴포넌트를 추가하고 초기화합니다.
        FurShield shield = core.gameObject.AddComponent<FurShield>();
        shield.Initialize(gaugeCostPerHit, pushbackForce, pushbackDuration);

        Debug.Log("<color=brown>털 갑옷 활성화!</color> 이제부터 공격을 반격합니다.");
    }
}