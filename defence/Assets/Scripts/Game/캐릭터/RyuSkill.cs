// ���� �̸�: RyuSkill.cs
using UnityEngine;
using System.Collections.Generic;

// ���� �޴� �̸��� ���� �̸��� RyuSkill�� ���� ����
[CreateAssetMenu(fileName = "RyuSkill", menuName = "TowerDefense/Skills/Ryu Skill")]
// ���� Ŭ���� �̸��� RyuSkill�� ���� ����
public class RyuSkill : ItemEffect
{
    [Header("��ų ���� UI")]
    [Tooltip("�ð��� ������ �� �� ������ �г� ������")]
    public GameObject positioningPanelPrefab;

    [Header("������ ����Ʈ")]
    [Tooltip("������ ��� ������")]
    public GameObject pillarPrefab; // �� �����տ��� ���� RyuPillar.cs�� �پ�� �մϴ�.
    [Tooltip("Ÿ�� ������ ������ ĳ���� �̹��� ������")]
    public GameObject characterImagePrefab;

    [Header("������ ��ų ���")]
    [Tooltip("1, 2, 3������ ���� ��ų ���ӽð�(��)")]
    public List<float> durationByLevel = new List<float> { 3f, 3f, 4f };
    [Tooltip("1, 2, 3������ ���� �ʴ� ������")]
    public List<int> damagePerSecondByLevel = new List<int> { 80, 110, 150 };
    [Header("��ų ��ġ ����")]
    // ���� �� �κ��� BattleFocusTargeter.cs�� ���ǵ� SkillPositionInfo�� ����մϴ�.
    public List<SkillPositionInfo> skillPositions = new List<SkillPositionInfo>();

    public override void ExecuteEffect()
    {
        BattleFocusTargeter.instance.BeginTargeting(positioningPanelPrefab, skillPositions, SpawnPillarAtTarget);
    }

    private void SpawnPillarAtTarget(GameObject targetPositionObject)
    {
        if (targetPositionObject == null) return;

        // ... (���� �� �ɷ�ġ ��� �ڵ�� �״��) ...
        int levelIndex = 0;
        if (GameSession.instance != null && SaveLoadManager.instance != null)
        {
            string charID = GameSession.instance.selectedCharacter.characterID;
            int charLevel = SaveLoadManager.instance.gameData.characterLevels[charID];
            levelIndex = Mathf.Clamp(charLevel - 1, 0, durationByLevel.Count - 1);
        }
        float duration = durationByLevel[levelIndex];
        int dps = damagePerSecondByLevel[levelIndex];

        // ��� ����
        GameObject pillarGO = Instantiate(pillarPrefab, targetPositionObject.transform.position, Quaternion.identity);
        pillarGO.GetComponent<RyuPillar>().Initialize(dps, duration);

        // ���� �� �κ��� �����մϴ� ����
        if (characterImagePrefab != null)
        {
            // Quaternion.identity ��� Quaternion.Euler(0, 0, 90)���� �����Ͽ� Z������ 90�� ȸ��
            Quaternion characterRotation = Quaternion.Euler(0, 0, 90);
            GameObject charImage = Instantiate(characterImagePrefab, targetPositionObject.transform.position, characterRotation);
            Destroy(charImage, duration + 0.1f);
        }
        // ���� ������� ���� ����

        Debug.Log($"<color=blue>RYU ��ų �ߵ�!</color> Lv.{levelIndex + 1} (����:{duration}s, DPS:{dps})");
    }
}
