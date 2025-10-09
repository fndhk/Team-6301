// 파일 이름: PianoTower.cs
using UnityEngine;

public class PianoTower : BaseTower
{
    [Header("피아노 타워 전용")]
    public AudioClip[] notes;
    private int noteIndex = 0;
    private AudioSource audioSource;

    protected override void Start()
    {
        base.Start(); // 부모의 Start() 기능을 먼저 실행
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public override void Attack(int finalDamage)
    {
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        if (projectile != null && target != null)
        {
            projectile.Initialize(finalDamage, target.position, transform);
            PlaySound();
        }
    }

    private void PlaySound()
    {
        if (notes.Length > 0 && audioSource != null)
        {
            audioSource.PlayOneShot(notes[noteIndex]);
            noteIndex = (noteIndex + 1) % notes.Length;
        }
    }
}