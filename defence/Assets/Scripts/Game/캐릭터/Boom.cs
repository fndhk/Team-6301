using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FirePositionInfo
{
    [Tooltip("탄환이 발사될 위치 프리팹 또는 씬의 오브젝트")]
    public GameObject firePositionObject;
    [Tooltip("이 위치에서 1, 2, 3레벨에 발사할 총 탄환 수")]
    public List<int> projectileCountByLevel = new List<int> { 1, 2, 3 };
}

[CreateAssetMenu(fileName = "BoomSkill", menuName = "TowerDefense/Skills/Boom Skill")]
public class Boom : ItemEffect
{
    [Header("발사 설정")]
    [Tooltip("발사될 탄환 프리팹")]
    public GameObject projectilePrefab;
    [Tooltip("탄환 한 발당 데미지 (레벨별)")]
    public List<int> damageByLevel = new List<int> { 100, 150, 200 };

    [Header("패턴 설정")]
    [Tooltip("탄환과 탄환 사이의 발사 간격 (초)")]
    public float fireInterval = 0.1f;
    [Tooltip("발사 위치 및 횟수 설정")]
    public List<FirePositionInfo> firePositions = new List<FirePositionInfo>();

    public override void ExecuteEffect()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Boom 스킬에 Projectile Prefab이 설정되지 않았습니다!");
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
            Debug.LogWarning("Boom 스킬의 현재 레벨 데미지가 0 이하입니다.");
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

        // 발사 순서 생성: a-b-c-d-e
        for (int i = 0; i < firePositions.Count; i++) fireOrder.Add(i);
        // 발사 순서 생성: e-d-c-b-a (역순)
        for (int i = firePositions.Count - 1; i >= 0; i--) fireOrder.Add(i);
        // 발사 순서 생성: a-b-c-d-e
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

// 코루틴을 실행하기 위한 헬퍼 클래스
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

