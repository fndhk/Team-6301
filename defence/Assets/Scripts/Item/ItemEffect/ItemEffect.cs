// ���� �̸�: ItemEffect.cs
using UnityEngine;

// abstract(�߻�) Ŭ������ ���� ���� ������Ʈ�� ���� �� ����,
// ���� �ٸ� Ŭ������ ����ϱ� ���� '���赵' ���Ҹ� �մϴ�.
public abstract class ItemEffect : ScriptableObject
{
    // �� Ŭ������ ��ӹ޴� ��� ȿ���� �ݵ�� ExecuteEffect �޼ҵ带 �����ؾ� �մϴ�.
    public abstract void ExecuteEffect();
}