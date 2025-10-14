// ���� �̸�: DrumTower.cs
using UnityEngine;

public class DrumTower : BaseTower
{
    [Header("�巳 Ÿ�� ����")]
    // ------ �ű� ����: ���� ���� ���� �߰� ------
    [Tooltip("����ü�� ���� ����� �� �⺻ ���� �ݰ�")]
    [SerializeField] private float baseExplosionRadius = 2f;

    [Tooltip("������ �����ϴ� ���� �ݰ� (��: 0.5 = ������ +0.5 �ݰ�)")]
    [SerializeField] private float explosionRadiusPerLevel = 0.5f;

    public GameObject attackEffectPrefab; // ��ź ���� ���� ����Ʈ
    public AudioClip attackSound;
    private AudioSource audioSource;

    protected override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    // ------ �ű� ����: �ǾƳ�Ÿ��ó�� ����ü �߻� ------
    public override void Attack(int finalDamage)
    {
        // ����ü�� ������ ��� ���
        if (projectilePrefab == null)
        {
            Debug.LogWarning($"{gameObject.name}: Projectile Prefab�� �������� �ʾҽ��ϴ�!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning($"{gameObject.name}: Fire Point�� �������� �ʾҽ��ϴ�!");
            return;
        }

        if (target == null)
        {
            Debug.LogWarning($"{gameObject.name}: Ÿ���� �����ϴ�!");
            return;
        }

        // ����ü ����
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            // ------ �ű� ����: ���� ������ ���� ���� �ݰ� ��� ------
            int currentTotalLevel = GetCurrentTotalLevel();
            float currentExplosionRadius = baseExplosionRadius + (currentTotalLevel - 1) * explosionRadiusPerLevel;

            // �ּҰ� ���� (Ȥ�� ���� 0�̾ �ּ� �ݰ��� ����)
            currentExplosionRadius = Mathf.Max(currentExplosionRadius, 1f);

            // ------ �ű� ����: DrumProjectile�� �ʱ�ȭ (���� ���� ���� ����) ------
            projectile.InitializeAsSplash(finalDamage, target.position, transform, currentExplosionRadius, attackEffectPrefab);

            // ���� ��� (�߻� ��)
            if (attackSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(attackSound);
            }

            Debug.Log($"<color=cyan>[DrumTower]</color> ���� {currentTotalLevel} - ���� �ݰ�: {currentExplosionRadius:F1}");
        }
        else
        {
            Debug.LogError($"{gameObject.name}: ����ü�� Projectile ��ũ��Ʈ�� �����ϴ�!");
        }
    }

    // ------ �ű� �߰�: �����Ϳ��� ���� ������ ������ ǥ�� ------
    void OnDrawGizmosSelected()
    {
        // Ÿ�� ���� ���� (����ü�� ���ư� �� �ִ� �Ÿ�)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // ------ �ű� ����: ���� ������ ���� ���� ���� ��� ------
        int currentTotalLevel = GetCurrentTotalLevel();
        float currentExplosionRadius = baseExplosionRadius + (currentTotalLevel - 1) * explosionRadiusPerLevel;
        currentExplosionRadius = Mathf.Max(currentExplosionRadius, 1f);

        // ��ź �� ���� ���� (�� ���� ��)
        Gizmos.color = Color.red;
        if (target != null)
        {
            // Ÿ���� ������ Ÿ�� ��ġ�� ���� ���� ǥ��
            Gizmos.DrawWireSphere(target.position, currentExplosionRadius);

            // ���� ���� ���̺� ǥ��
#if UNITY_EDITOR
            UnityEditor.Handles.Label(
                target.position + Vector3.up * (currentExplosionRadius + 0.5f),
                $"���� �ݰ�: {currentExplosionRadius:F1}\n(Lv.{currentTotalLevel})"
            );
#endif
        }
        else
        {
            // Ÿ���� ������ Ÿ�� �տ� ���÷� ǥ��
            Vector3 previewPos = transform.position + Vector3.up * 5f;
            Gizmos.DrawWireSphere(previewPos, currentExplosionRadius);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(
                previewPos + Vector3.up * (currentExplosionRadius + 0.5f),
                $"���� �ݰ�: {currentExplosionRadius:F1}\n(Lv.{currentTotalLevel})"
            );
#endif
        }
    }
}