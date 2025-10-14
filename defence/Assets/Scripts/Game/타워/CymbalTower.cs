// 파일 이름: CymbalTower.cs
using UnityEngine;

public class CymbalTower : BaseTower
{
    [Header("심벌즈 타워 전용")]
    [SerializeField] private int maxBounces = 2;
    [SerializeField] private AudioClip attackSound;
    private AudioSource audioSource;

    protected override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    public override void Attack(int finalDamage)
    {
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        if (projectile != null && target != null)
        {
            projectile.Initialize(finalDamage, target.position, transform, maxBounces, attackRange);

            if (attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }
        }
    }
}