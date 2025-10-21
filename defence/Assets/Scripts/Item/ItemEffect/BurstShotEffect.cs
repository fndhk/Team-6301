using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 추가
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BurstShotEffect", menuName = "TowerDefense/ItemEffects/Burst Shot")]
public class BurstShotEffect : ItemEffect
{
    [Header("발사 설정")]
    [Tooltip("발사될 탄환 프리팹")]
    public GameObject projectilePrefab;

    [Tooltip("탄환 한 발당 데미지")]
    public int damage = 50;

    [Header("발사 위치 설정")]
    [Tooltip("탄환이 발사될 위치 오브젝트")]
    // public List<GameObject> firePositionObjects;
    public string firePointTag = "FirePoint";

    [Header("연사 설정")]
    [Tooltip("추가 공격 사이의 시간 간격 (초)")]
    public float intervalBetweenAttacks = 0.5f; // 0.5초 간격

    public override void ExecuteEffect()
    {
        // --- 필수 요소 확인 (기존과 동일) ---
        if (projectilePrefab == null)
        {
            Debug.LogError("BurstShotEffect: projectilePrefab이 설정되지 않았습니다!");
            return;
        }
        GameObject[] firePointObjects = GameObject.FindGameObjectsWithTag(firePointTag);

        if (firePointObjects.Length == 0)
        {
            Debug.LogError($"BurstShotEffect: '{firePointTag}' 태그를 가진 발사 위치 오브젝트를 씬에서 찾을 수 없습니다!");
            return;
        }

        // --- 레벨에 따른 총 공격 횟수 계산 ---
        int upgradeLevel = 0;
        if (SaveLoadManager.instance != null && SaveLoadManager.instance.gameData != null)
        {
            upgradeLevel = SaveLoadManager.instance.gameData.quickSlotUpgradeLevel;
        }
        int totalAttacks = 1 + upgradeLevel; // 기본 1회 + 레벨당 1회 추가

        Debug.Log($"<color=orange>동시 탄환 발사! 총 {totalAttacks}번 발사 (레벨 {upgradeLevel}).</color>");

        // --- 직접 발사 대신 코루틴 시작 ---
        // SkillCoroutineRunner는 다른 스킬에서 코루틴을 실행하기 위해 만든 헬퍼 클래스입니다.
        // 만약 이 클래스가 없다면, 다른 방식으로 코루틴을 실행해야 합니다.
        SkillCoroutineRunner.Run(BurstCoroutine(totalAttacks, firePointObjects));
    }

    // --- 연사를 처리하는 코루틴 ---
    private IEnumerator BurstCoroutine(int totalAttacks, GameObject[] firePoints)
    {
        for (int i = 0; i < totalAttacks; i++)
        {
            // 찾은 오브젝트들의 위치에서 발사
            foreach (var firePointObject in firePoints)
            {
                if (firePointObject != null)
                {
                    Transform firePoint = firePointObject.transform;
                    Vector3 targetPos = firePoint.position + Vector3.up * 100f;

                    GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                    Projectile projectile = projectileGO.GetComponent<Projectile>();
                    if (projectile != null)
                    {
                        projectile.Initialize(damage, targetPos, null);
                    }
                }
            }

            if (i < totalAttacks - 1)
            {
                yield return new WaitForSeconds(intervalBetweenAttacks);
            }
        }
    }
}