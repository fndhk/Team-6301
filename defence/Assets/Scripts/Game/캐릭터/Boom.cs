using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FirePositionInfo
{
    [Tooltip("źȯ�� �߻�� ��ġ ������ �Ǵ� ���� ������Ʈ")]
    public GameObject firePositionObject;
    [Tooltip("�� ��ġ���� 1, 2, 3������ �߻��� �� źȯ ��")]
    public List<int> projectileCountByLevel = new List<int> { 1, 2, 3 };
}

[CreateAssetMenu(fileName = "BoomSkill", menuName = "TowerDefense/Skills/Boom Skill")]
public class Boom : ItemEffect
{
    [Header("�߻� ����")]
    [Tooltip("�߻�� źȯ ������")]
    public GameObject projectilePrefab;
    [Tooltip("źȯ �� �ߴ� ������ (������)")]
    public List<int> damageByLevel = new List<int> { 100, 150, 200 };

    [Header("���� ����")]
    [Tooltip("źȯ�� źȯ ������ �߻� ���� (��)")]
    public float fireInterval = 0.1f;
    [Tooltip("�߻� ��ġ �� Ƚ�� ����")]
    public List<FirePositionInfo> firePositions = new List<FirePositionInfo>();

    public override void ExecuteEffect()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Boom ��ų�� Projectile Prefab�� �������� �ʾҽ��ϴ�!");
            return;
        }

        SkillCoroutineRunner.Run(FirePatternCoroutine());
    }

    private IEnumerator FirePatternCoroutine()
    {
        if (GameSession.instance == null || SaveLoadManager.instance == null) yield break;

        string charID = GameSession.instance.selectedCharacter.characterID;
        if (!SaveLoadManager.instance.gameData.characterLevels.ContainsKey(charID)) yield break;

        int charLevel = SaveLoadManager.instance.gameData.characterLevels[charID];
        int levelIndex = charLevel - 1;

        if (levelIndex < 0) yield break;

        int currentDamage = (levelIndex < damageByLevel.Count) ? damageByLevel[levelIndex] : 0;
        if (currentDamage <= 0)
        {
            Debug.LogWarning("Boom ��ų�� ���� ���� �������� 0 �����Դϴ�.");
            yield break;
        }

        List<int> fireOrder = new List<int>();
        int[] shotsRemaining = new int[firePositions.Count];
        int totalShots = 0;

        for (int i = 0; i < firePositions.Count; i++)
        {
            int count = (levelIndex < firePositions[i].projectileCountByLevel.Count) ? firePositions[i].projectileCountByLevel[levelIndex] : 0;
            shotsRemaining[i] = count;
            totalShots += count;
        }

        // �߻� ���� ����: a-b-c-d-e
        for (int i = 0; i < firePositions.Count; i++) fireOrder.Add(i);
        // �߻� ���� ����: e-d-c-b-a (����)
        for (int i = firePositions.Count - 1; i >= 0; i--) fireOrder.Add(i);
        // �߻� ���� ����: a-b-c-d-e
        for (int i = 0; i < firePositions.Count; i++) fireOrder.Add(i);

        int firedCount = 0;
        int orderIndex = 0;
        while (firedCount < totalShots)
        {
            if (orderIndex >= fireOrder.Count) orderIndex = 0;

            int currentFirePosIndex = fireOrder[orderIndex];

            if (shotsRemaining[currentFirePosIndex] > 0)
            {
                Transform firePoint = firePositions[currentFirePosIndex].firePositionObject.transform;
                Vector3 targetPos = firePoint.position + Vector3.up * 100f;

                GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                Projectile projectile = projectileGO.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.Initialize(currentDamage, targetPos, null);
                }

                shotsRemaining[currentFirePosIndex]--;
                firedCount++;
                yield return new WaitForSeconds(fireInterval);
            }
            orderIndex++;
        }
    }
}

// �ڷ�ƾ�� �����ϱ� ���� ���� Ŭ����
public class SkillCoroutineRunner : MonoBehaviour
{
    private static SkillCoroutineRunner _instance;
    public static SkillCoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SkillCoroutineRunner");
                _instance = go.AddComponent<SkillCoroutineRunner>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public static Coroutine Run(IEnumerator coroutine)
    {
        return Instance.StartCoroutine(coroutine);
    }
}

