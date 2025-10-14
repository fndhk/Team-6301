// ���� �̸�: PianoTower.cs (��ü ��ü - �� ������)
using UnityEngine;

public class PianoTower : BaseTower
{
    [Header("�ǾƳ� Ÿ�� ����")]
    public AudioClip[] notes;
    private int noteIndex = 0;
    private AudioSource audioSource;

    protected override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public override void Attack(int finalDamage)
    {
        Debug.Log("--- [PianoTower] Attack ���� ---");

        if (projectilePrefab == null) { Debug.LogError("[PianoTower] ����: projectilePrefab ������ ����ֽ��ϴ�!"); return; }
        Debug.Log("A. projectilePrefab Ȯ��: OK");

        if (firePoint == null) { Debug.LogError("[PianoTower] ����: firePoint ������ ����ֽ��ϴ�!"); return; }
        Debug.Log("B. firePoint Ȯ��: OK");

        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Debug.Log("C. ����ü ����: OK");

        Projectile projectile = projectileGO.GetComponent<Projectile>();
        if (projectile == null) { Debug.LogError("[PianoTower] ����: ������ ����ü�� Projectile ��ũ��Ʈ�� �����ϴ�!"); return; }
        Debug.Log("D. Projectile ������Ʈ ��������: OK");

        if (target == null) { Debug.LogError("[PianoTower] ����: Attack �Լ� ���� �� target�� ��������ϴ�!"); return; }
        Debug.Log("E. Ÿ�� ��Ȯ��: OK (" + target.name + ")");

        projectile.Initialize(finalDamage, target.position, transform);
        Debug.Log("F. ����ü �ʱ�ȭ: OK");

        PlaySound();
        Debug.Log("--- [PianoTower] Attack ���� ---");
    }

    private void PlaySound()
    {
        Debug.Log("--- [PianoTower] PlaySound ���� ---");

        if (audioSource == null) { Debug.LogWarning("[PianoTower] audioSource�� �����ϴ�. �Ҹ��� ����� �� �����ϴ�."); return; }
        Debug.Log("G. audioSource Ȯ��: OK");

        if (notes == null || notes.Length == 0) { Debug.LogWarning("[PianoTower] notes ����� Ŭ���� ����ֽ��ϴ�."); return; }
        Debug.Log("H. notes ����� Ŭ�� Ȯ��: OK");

        audioSource.PlayOneShot(notes[noteIndex]);
        Debug.Log("I. ���� ��� �Ϸ�.");
        noteIndex = (noteIndex + 1) % notes.Length;
    }
}