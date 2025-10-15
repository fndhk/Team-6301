// ���� �̸�: CharacterData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "TowerDefense/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("ĳ���� ����")]
    public string characterName;
    [TextArea(3, 10)]
    public string characterDescription;
    public Sprite characterIcon; // ĳ���� ����â�� ǥ�õ� �̹���

    // ------ �ű� �߰�: ��ų �ƽ� �̹��� ------
    [Header("��ų �ƽ�")]
    [Tooltip("��ų ��� �� ǥ�õ� �ƽ� �̹���")]
    public Sprite skillCutsceneImage;

    [Header("ĳ���� �⺻ �ɷ�ġ")]
    [Tooltip("�ھ�(����)�� �⺻ ü�¿� �������� ���ʽ�")]
    public int coreHealthBonus = 0;
    [Tooltip("��� Ÿ���� �⺻ ���ݷ¿� �������� ���ʽ�")]
    public int towerAttackDamageBonus = 0;
    [Tooltip("��� Ÿ���� �⺻ ���ݼӵ��� �������� ���� (1.2 = 20% ����)")]
    public float towerAttackSpeedMultiplier = 1f;

    [Header("���� ��ų")]
    // ������ ���� ItemEffect �ý����� ��ų�� �����մϴ�.
    public ItemEffect characterSkill;
}