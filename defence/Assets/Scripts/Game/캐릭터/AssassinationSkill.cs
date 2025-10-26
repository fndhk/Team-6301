// 파일 이름: AssassinationSkill.cs (레벨별 데미지 적용)
using UnityEngine;
using System.Collections.Generic; // List를 사용하기 위해 추가

[CreateAssetMenu(fileName = "AssassinationSkill", menuName = "TowerDefense/Skills/Assassination Skill")]
public class AssassinationSkill : ItemEffect
{
    [Header("스킬 설정")]
    [Tooltip("캐릭터 레벨 1, 2, 3일 때의 스킬 데미지")]
    public List<int> damageByLevel = new List<int> { 1000, 1500, 2000 };

    [Tooltip("타격 시 대상의 몸에 생성될 이펙트 프리팹")]
    public GameObject assassinationEffectPrefab;

    public override void ExecuteEffect()
    {
        // 1. EnemyManager 찾기
        if (EnemyManager.instance == null)
        {
            Debug.LogError("AssassinationSkill: EnemyManager를 찾을 수 없습니다!");
            return;
        }

        // 2. 현재 캐릭터 레벨 가져오기 (PanGang.cs, HealSkill.cs 등 참고)
        int charLevel = 1; // 기본값
        if (GameSession.instance != null && GameSession.instance.selectedCharacter != null && SaveLoadManager.instance != null)
        {
            string charID = GameSession.instance.selectedCharacter.characterID;
            if (SaveLoadManager.instance.gameData.characterLevels.TryGetValue(charID, out int foundLevel))
            {
                charLevel = foundLevel;
            }
        }

        // 3. 레벨에 맞는 데미지 계산
        // level 1 -> index 0, level 2 -> index 1...
        int levelIndex = Mathf.Clamp(charLevel - 1, 0, damageByLevel.Count - 1);
        int currentDamage = damageByLevel[levelIndex];

        // 4. 가장 체력이 많은 적 찾기
        Enemy target = EnemyManager.instance.FindHighestHealthEnemy();

        // 5. 타겟이 있다면
        if (target != null)
        {
            Debug.Log($"<color=red>표적 공격!</color> {target.gameObject.name} (체력: {target.currentHealth})에게 {currentDamage} 데미지 (Lv.{charLevel})!");

            // 6. 이펙트 생성
            if (assassinationEffectPrefab != null)
            {
                Instantiate(assassinationEffectPrefab, target.transform.position, Quaternion.identity);
            }

            // 7. 해당 적에게 레벨에 맞는 데미지 적용
            target.TakeDamage(currentDamage, null);
        }
        else
        {
            Debug.Log("AssassinationSkill: 공격할 대상이 없습니다.");
        }
    }
}