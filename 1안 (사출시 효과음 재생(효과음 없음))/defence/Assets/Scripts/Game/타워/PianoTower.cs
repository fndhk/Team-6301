// ���� �̸�: PianoTower.cs
using UnityEngine;

public class PianoTower : BaseTower
{
    [Header("�ǾƳ� Ÿ�� ����")]
    public AudioClip[] notes;
    private int noteIndex = 0;
    private AudioSource audioSource;

    protected override void Start()
    {
        base.Start(); // �θ��� Start() ����� ���� ����
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