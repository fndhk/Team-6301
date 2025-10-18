// 파일 이름: ItemEffect.cs
using UnityEngine;

// abstract(추상) 클래스는 직접 게임 오브젝트에 붙일 수 없고,
// 오직 다른 클래스가 상속하기 위한 '설계도' 역할만 합니다.
public abstract class ItemEffect : ScriptableObject
{
    [HideInInspector] public bool isAutomatic = false; // 자동 발동 스킬인지 여부

    public abstract void ExecuteEffect();
}