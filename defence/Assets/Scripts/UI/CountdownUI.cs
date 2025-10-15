using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownUI : MonoBehaviour
{
    public static CountdownUI instance;

    [Header("UI ����")]
    public GameObject countdownPanel; // ī��Ʈ�ٿ��� ǥ�õ� �г�
    public TextMeshProUGUI countdownText; // "3", "2", "1", "START!" �ؽ�Ʈ

    [Header("ī��Ʈ�ٿ� ����")]
    public float countdownDuration = 3f; // �� ī��Ʈ�ٿ� �ð� (3��)
    public AudioClip countBeep; // ī��Ʈ ���� (���û���)
    public AudioClip startBeep; // ���� ���� (���û���)

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
        // ���� ���� �� ī��Ʈ�ٿ� �г��� Ȱ��ȭ�ϰ� ī��Ʈ ����
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(true);
        }

        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        // 3, 2, 1 ī��Ʈ
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            countdownText.fontSize = 120; // ū ����

            // ī��Ʈ ���� ���
            if (countBeep != null && audioSource != null)
            {
                audioSource.PlayOneShot(countBeep);
            }

            yield return new WaitForSeconds(1f);
        }

        // "START!" ǥ��
        countdownText.text = "START!";
        countdownText.fontSize = 100;

        // ���� ���� ���
        if (startBeep != null && audioSource != null)
        {
            audioSource.PlayOneShot(startBeep);
        }

        yield return new WaitForSeconds(0.5f);

        // ī��Ʈ�ٿ� ����
        isCountdownFinished = true;

        // �г� ��Ȱ��ȭ
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }

        // ���� ���� ��ȣ ����
        NotifyGameStart();
    }

    private void NotifyGameStart()
    {
        // RhythmManager���� ���� ��ȣ
        if (RhythmManager.instance != null)
        {
            RhythmManager.instance.StartRhythm();
        }

        // AudioManager���� ���� ��ȣ
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StartMusic();
        }

        // EnemySpawner���� ���� ��ȣ
        EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.StartSpawning();
        }

        // NoteSpawner���� ���� ��ȣ
        
    }

    // �ܺο��� ī��Ʈ�ٿ��� �������� Ȯ���ϴ� �Լ�
    public bool IsCountdownFinished()
    {
        return isCountdownFinished;
    }
}