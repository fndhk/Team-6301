using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownUI : MonoBehaviour
{
    public static CountdownUI instance;

    [Header("UI 연결")]
    public GameObject countdownPanel; // 카운트다운이 표시될 패널
    public TextMeshProUGUI countdownText; // "3", "2", "1", "START!" 텍스트

    [Header("카운트다운 설정")]
    public float countdownDuration = 3f; // 총 카운트다운 시간 (3초)
    public AudioClip countBeep; // 카운트 사운드 (선택사항)
    public AudioClip startBeep; // 시작 사운드 (선택사항)

    private AudioSource audioSource;
    private bool isCountdownFinished = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        // 게임 시작 시 카운트다운 패널을 활성화하고 카운트 시작
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(true);
        }

        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        // 3, 2, 1 카운트
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            countdownText.fontSize = 120; // 큰 숫자

            // 카운트 사운드 재생
            if (countBeep != null && audioSource != null)
            {
                audioSource.PlayOneShot(countBeep);
            }

            yield return new WaitForSeconds(1f);
        }

        // "START!" 표시
        countdownText.text = "START!";
        countdownText.fontSize = 100;

        // 시작 사운드 재생
        if (startBeep != null && audioSource != null)
        {
            audioSource.PlayOneShot(startBeep);
        }

        yield return new WaitForSeconds(0.5f);

        // 카운트다운 종료
        isCountdownFinished = true;

        // 패널 비활성화
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }

        // 게임 시작 신호 전송
        NotifyGameStart();
    }

    private void NotifyGameStart()
    {
        // RhythmManager에게 시작 신호
        if (RhythmManager.instance != null)
        {
            RhythmManager.instance.StartRhythm();
        }

        // AudioManager에게 시작 신호
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StartMusic();
        }

        // EnemySpawner에게 시작 신호
        EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.StartSpawning();
        }

        if (RhythmInputManager.instance != null)
        {
            RhythmInputManager.instance.StartInputProcessing();
        }

    }

    // 외부에서 카운트다운이 끝났는지 확인하는 함수
    public bool IsCountdownFinished()
    {
        return isCountdownFinished;
    }
}