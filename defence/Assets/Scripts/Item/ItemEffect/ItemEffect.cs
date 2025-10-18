// ���� �̸�: ItemEffect.cs
using UnityEngine;

// abstract(�߻�) Ŭ������ ���� ���� ������Ʈ�� ���� �� ����,
// ���� �ٸ� Ŭ������ ����ϱ� ���� '���赵' ���Ҹ� �մϴ�.
public abstract class ItemEffect : ScriptableObject
{
    [HideInInspector] public bool isAutomatic = false; // �ڵ� �ߵ� ��ų���� ����

    public abstract void ExecuteEffect();
}