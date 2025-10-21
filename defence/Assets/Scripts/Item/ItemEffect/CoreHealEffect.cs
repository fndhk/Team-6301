// 파일 이름: CoreHealEffect.cs
using UnityEngine;

[CreateAssetMenu(fileName = "CoreHealEffect", menuName = "TowerDefense/ItemEffects/Core Heal")]
public class CoreHealEffect : ItemEffect
{
    [Header("회복 설정")]
    [Tooltip("코어를 회복시킬 양")]
    public int healAmount = 200;

    public override void ExecuteEffect()
    {
        // 1. 씬에 있는 CoreFacility 오브젝트를 찾습니다.
        CoreFacility core = FindFirstObjectByType<CoreFacility>();

        // 2. 만약 코어가 없다면 오류 메시지를 출력하고 종료합니다.
        if (core == null)
        {
            Debug.LogError("CoreHealEffect: 씬에서 CoreFacility를 찾을 수 없습니다!");
            return;
        }

        // 3. 찾은 코어의 Heal 함수를 호출합니다.
        core.Heal(healAmount); //

        Debug.Log($"<color=green>코어 회복 아이템 사용! 코어의 체력을 {healAmount}만큼 회복합니다.</color>");
    }
}