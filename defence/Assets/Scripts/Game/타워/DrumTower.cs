// 파일 이름: DrumTower.cs
using UnityEngine;

public class DrumTower : BaseTower
{
    [Header("드럼 타워 전용")]
    // ------ 신규 수정: 범위 공격 설정 추가 ------
    [Tooltip("투사체가 적에 닿았을 때 기본 폭발 반경")]
    [SerializeField] private float baseExplosionRadius = 2f;

    [Tooltip("레벨당 증가하는 폭발 반경 (예: 0.5 = 레벨당 +0.5 반경)")]
    [SerializeField] private float explosionRadiusPerLevel = 0.5f;

    public GameObject attackEffectPrefab; // 착탄 지점 폭발 이펙트
    public AudioClip attackSound;
    private AudioSource audioSource;

    protected override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    // ------ 신규 수정: 피아노타워처럼 투사체 발사 ------
    public override void Attack(int finalDamage)
    {
        // 투사체가 없으면 경고 출력
        if (projectilePrefab == null)
        {
            Debug.LogWarning($"{gameObject.name}: Projectile Prefab이 설정되지 않았습니다!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning($"{gameObject.name}: Fire Point가 설정되지 않았습니다!");
            return;
        }

        if (target == null)
        {
            Debug.LogWarning($"{gameObject.name}: 타겟이 없습니다!");
            return;
        }

        // 투사체 생성
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            // ------ 신규 수정: 현재 레벨에 따른 폭발 반경 계산 ------
            int currentTotalLevel = GetCurrentTotalLevel();
            float currentExplosionRadius = baseExplosionRadius + (currentTotalLevel - 1) * explosionRadiusPerLevel;

            // 최소값 보장 (혹시 레벨 0이어도 최소 반경은 유지)
            currentExplosionRadius = Mathf.Max(currentExplosionRadius, 1f);

            // ------ 신규 수정: DrumProjectile로 초기화 (범위 공격 정보 전달) ------
            projectile.InitializeAsSplash(finalDamage, target.position, transform, currentExplosionRadius, attackEffectPrefab);

            // 사운드 재생 (발사 시)
            if (attackSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(attackSound);
            }

            Debug.Log($"<color=cyan>[DrumTower]</color> 레벨 {currentTotalLevel} - 폭발 반경: {currentExplosionRadius:F1}");
        }
        else
        {
            Debug.LogError($"{gameObject.name}: 투사체에 Projectile 스크립트가 없습니다!");
        }
    }

    // ------ 신규 추가: 에디터에서 폭발 범위를 원으로 표시 ------
    void OnDrawGizmosSelected()
    {
        // 타워 공격 범위 (투사체가 날아갈 수 있는 거리)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // ------ 신규 수정: 현재 레벨에 따른 폭발 범위 계산 ------
        int currentTotalLevel = GetCurrentTotalLevel();
        float currentExplosionRadius = baseExplosionRadius + (currentTotalLevel - 1) * explosionRadiusPerLevel;
        currentExplosionRadius = Mathf.Max(currentExplosionRadius, 1f);

        // 착탄 시 폭발 범위 (더 작은 원)
        Gizmos.color = Color.red;
        if (target != null)
        {
            // 타겟이 있으면 타겟 위치에 폭발 범위 표시
            Gizmos.DrawWireSphere(target.position, currentExplosionRadius);

            // 폭발 범위 레이블 표시
#if UNITY_EDITOR
            UnityEditor.Handles.Label(
                target.position + Vector3.up * (currentExplosionRadius + 0.5f),
                $"폭발 반경: {currentExplosionRadius:F1}\n(Lv.{currentTotalLevel})"
            );
#endif
        }
        else
        {
            // 타겟이 없으면 타워 앞에 예시로 표시
            Vector3 previewPos = transform.position + Vector3.up * 5f;
            Gizmos.DrawWireSphere(previewPos, currentExplosionRadius);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(
                previewPos + Vector3.up * (currentExplosionRadius + 0.5f),
                $"폭발 반경: {currentExplosionRadius:F1}\n(Lv.{currentTotalLevel})"
            );
#endif
        }
    }
}