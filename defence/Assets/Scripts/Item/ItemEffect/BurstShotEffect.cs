// 파일 이름: BurstShotEffect.cs
using UnityEngine;
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
    [Tooltip("탄환이 발사될 위치 오브젝트들의 리스트. 씬에 있는 빈 GameObject를 연결해주세요.")]
    // ▼▼▼ 이 부분을 수정했습니다 ▼▼▼
    public List<GameObject> firePositions = new List<GameObject>();

    public override void ExecuteEffect()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("BurstShotEffect: projectilePrefab이 설정되지 않았습니다!");
            return;
        }

        if (firePositions.Count == 0)
        {
            Debug.LogWarning("BurstShotEffect: firePositions가 비어있습니다.");
            return;
        }

        Debug.Log($"<color=orange>동시 탄환 발사! {firePositions.Count}개의 탄환을 발사합니다.</color>");

        foreach (var firePointObject in firePositions)
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
    }
}