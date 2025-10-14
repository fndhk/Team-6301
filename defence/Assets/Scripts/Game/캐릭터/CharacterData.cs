// 파일 이름: CharacterData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "TowerDefense/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("캐릭터 정보")]
    public string characterName;
    [TextArea(3, 10)]
    public string characterDescription;
    public Sprite characterIcon; // 캐릭터 선택창에 표시될 이미지

    // ------ 신규 추가: 스킬 컷신 이미지 ------
    [Header("스킬 컷신")]
    [Tooltip("스킬 사용 시 표시될 컷신 이미지")]
    public Sprite skillCutsceneImage;

    [Header("캐릭터 기본 능력치")]
    [Tooltip("코어(기지)의 기본 체력에 더해지는 보너스")]
    public int coreHealthBonus = 0;
    [Tooltip("모든 타워의 기본 공격력에 더해지는 보너스")]
    public int towerAttackDamageBonus = 0;
    [Tooltip("모든 타워의 기본 공격속도에 곱해지는 배율 (1.2 = 20% 빠름)")]
    public float towerAttackSpeedMultiplier = 1f;

    [Header("고유 스킬")]
    // 기존에 만든 ItemEffect 시스템을 스킬로 재사용합니다.
    public ItemEffect characterSkill;
}