// 파일 이름: PianoTower.cs (전체 교체 - 상세 추적용)
using UnityEngine;

public class PianoTower : BaseTower
{
    [Header("피아노 타워 전용")]
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
        Debug.Log("--- [PianoTower] Attack 시작 ---");

        if (projectilePrefab == null) { Debug.LogError("[PianoTower] 오류: projectilePrefab 변수가 비어있습니다!"); return; }
        Debug.Log("A. projectilePrefab 확인: OK");

        if (firePoint == null) { Debug.LogError("[PianoTower] 오류: firePoint 변수가 비어있습니다!"); return; }
        Debug.Log("B. firePoint 확인: OK");

        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Debug.Log("C. 투사체 생성: OK");

        Projectile projectile = projectileGO.GetComponent<Projectile>();
        if (projectile == null) { Debug.LogError("[PianoTower] 오류: 생성된 투사체에 Projectile 스크립트가 없습니다!"); return; }
        Debug.Log("D. Projectile 컴포넌트 가져오기: OK");

        if (target == null) { Debug.LogError("[PianoTower] 오류: Attack 함수 실행 중 target이 사라졌습니다!"); return; }
        Debug.Log("E. 타겟 재확인: OK (" + target.name + ")");

        projectile.Initialize(finalDamage, target.position, transform);
        Debug.Log("F. 투사체 초기화: OK");

        PlaySound();
        Debug.Log("--- [PianoTower] Attack 종료 ---");
    }

    private void PlaySound()
    {
        Debug.Log("--- [PianoTower] PlaySound 시작 ---");

        if (audioSource == null) { Debug.LogWarning("[PianoTower] audioSource가 없습니다. 소리를 재생할 수 없습니다."); return; }
        Debug.Log("G. audioSource 확인: OK");

        if (notes == null || notes.Length == 0) { Debug.LogWarning("[PianoTower] notes 오디오 클립이 비어있습니다."); return; }
        Debug.Log("H. notes 오디오 클립 확인: OK");

        audioSource.PlayOneShot(notes[noteIndex]);
        Debug.Log("I. 사운드 재생 완료.");
        noteIndex = (noteIndex + 1) % notes.Length;
    }
}