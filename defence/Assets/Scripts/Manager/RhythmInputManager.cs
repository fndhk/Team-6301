// 파일 이름: RhythmInputManager.cs (수정된 최종 버전)
using UnityEngine;
using TMPro;
using System.Collections;
using static JudgmentManager;

public class RhythmInputManager : MonoBehaviour
{
    public static RhythmInputManager instance;

    [Header("입력 설정")]
    public KeyCode[] inputKeys = { KeyCode.A, KeyCode.S, KeyCode.Semicolon, KeyCode.Quote };
    public GameObject[] keyPressFeedbacks;

    [Header("판정 설정")]
    public Transform[] hitZone;
    public float perfectWindow = 0.5f;
    public float greatWindow = 1.0f;
    public float goodWindow = 1.5f;

    [Header("판정 피드백 UI")]
    public TextMeshProUGUI judgmentText;

    [Header("타워 연결")]
    public BaseTower[] correspondingTowers;
    public CoreFacility coreFacility;

    [Header("페널티 설정")]
    [Tooltip("Miss 판정 시 코어 체력이 감소하는 양입니다.")]
    public int missDamage = 50;

    // --- 판정 강화용 변수 ---
    private float currentPerfectWindow;
    private float currentGreatWindow;
    private float currentGoodWindow;
    private Coroutine judgmentBuffCoroutine;

    // '헛스윙' 카운터를 위한 변수 추가
    private int missPressCounter = 0;
    private const int MISS_PRESS_LIMIT = 3; // 3번 헛스윙 시 HP 감소

    private bool canStartInput = false; // 카운트다운이 끝나야 true가 됨

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (judgmentText != null) judgmentText.gameObject.SetActive(false);
        currentPerfectWindow = perfectWindow;
        currentGreatWindow = greatWindow;
        currentGoodWindow = goodWindow;
    }

    void Update()
    {
        // 카운트다운이 끝나지 않았다면(canStartInput이 false라면)
        // Update 함수를 즉시 종료시켜 키 입력을 받지 않습니다.
        if (!canStartInput)
        {
            return;
        }

        for (int i = 0; i < inputKeys.Length; i++)
        {
            if (Input.GetKeyDown(inputKeys[i]))
            {
                if (keyPressFeedbacks.Length > i) keyPressFeedbacks[i].SetActive(true);
                CheckForNote(i);
            }
            if (Input.GetKeyUp(inputKeys[i]))
            {
                if (keyPressFeedbacks.Length > i) keyPressFeedbacks[i].SetActive(false);
            }
        }
    }
    public void StartInputProcessing()
    {
        canStartInput = true;
        Debug.Log("RhythmInputManager: 입력 처리를 시작합니다.");
    }

    private void CheckForNote(int laneIndex)
    {
        Collider2D[] hitNotes = Physics2D.OverlapCircleAll(hitZone[laneIndex].position, currentGoodWindow);
        NoteObject closestNote = null;
        float minDistance = float.MaxValue;

        foreach (var noteCollider in hitNotes)
        {
            NoteObject note = noteCollider.GetComponent<NoteObject>();
            if (note != null && note.laneIndex == laneIndex)
            {
                float distance = Mathf.Abs(note.transform.position.y - hitZone[laneIndex].position.y);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestNote = note;
                }
            }
        }

        if (closestNote != null)
        {
            JudgmentManager.Judgment judgmentEnum = JudgmentManager.Judgment.Miss;
            string judgmentString = "";

            if (minDistance <= currentPerfectWindow) { judgmentEnum = JudgmentManager.Judgment.Perfect; judgmentString = "Perfect"; }
            else if (minDistance <= currentGreatWindow) { judgmentEnum = JudgmentManager.Judgment.Great; judgmentString = "Great"; }
            else if (minDistance <= currentGoodWindow) { judgmentEnum = JudgmentManager.Judgment.Good; judgmentString = "Good"; }

            if (judgmentEnum == JudgmentManager.Judgment.Miss)
            {
                // Case 1: 노트를 너무 일찍/늦게 쳐서 Miss (헛스윙으로 간주)
                ShowMissFeedback();
            }
            else
            {
                // Case 2: Good, Great, Perfect 성공
                ScoreManager.instance.AddRhythmScore(judgmentEnum);
                SkillManager.instance.AddGaugeOnJudgment(judgmentEnum);
                if (judgmentEnum == JudgmentManager.Judgment.Perfect || judgmentEnum == JudgmentManager.Judgment.Great)
                {
                    if (AudioManager.instance != null) AudioManager.instance.PlayInstrumentSound(closestNote.instrumentType);
                }
                ShowJudgmentFeedback(judgmentString);
                Destroy(closestNote.gameObject);

                // 특수 노트 처리
                if (closestNote.isSpecialNote && judgmentEnum <= JudgmentManager.Judgment.Great)
                {
                    SaveLoadManager.instance.gameData.gachaTickets++;
                    Debug.Log("특수 노트 획득! 티켓 +1");
                }

                // ▼▼▼ 성공 시 '헛스윙' 카운터 초기화 ▼▼▼
                missPressCounter = 0;
            }
        }
        else
        {
            // Case 3: 허공에 키를 누름 (헛스윙)
            ShowMissFeedback();
        }
    }

    // ▼▼▼ '헛스윙' 페널티 함수 (ShowMissFeedback 이름은 그대로 사용) ▼▼▼
    // 이 함수는 이제 '헛스윙' (키 입력 실수) 시에만 호출됩니다.
    public void ShowMissFeedback()
    {
        ShowJudgmentFeedback("Miss");
        ScoreManager.instance.AddRhythmScore(JudgmentManager.Judgment.Miss);
        SkillManager.instance.AddGaugeOnJudgment(JudgmentManager.Judgment.Miss);

        // '헛스윙' 카운터 증가
        missPressCounter++;
        Debug.Log($"헛스윙 카운트: {missPressCounter} / {MISS_PRESS_LIMIT}");

        // 카운터가 3에 도달하면 HP 감소 및 카운터 초기화
        if (missPressCounter >= MISS_PRESS_LIMIT)
        {
            if (coreFacility != null)
            {
                coreFacility.TakeDamage(missDamage, null);
                Debug.LogWarning($"헛스윙 3회 누적! 코어 데미지 {missDamage}!");
            }
            missPressCounter = 0; // 카운터 초기화
        }
    }

    // ▼▼▼ '놓친 노트' 페널티 함수 (새로 추가) ▼▼▼
    // 이 함수는 NoteObject.cs가 노트를 놓쳤을 때만 호출됩니다.
    public void ProcessPassedNotePenalty()
    {
        ShowJudgmentFeedback("Miss");
        ScoreManager.instance.AddRhythmScore(JudgmentManager.Judgment.Miss);
        SkillManager.instance.AddGaugeOnJudgment(JudgmentManager.Judgment.Miss);

        // '놓친 노트'는 카운터와 상관없이 즉시 HP 감소
        if (coreFacility != null)
        {
            coreFacility.TakeDamage(missDamage, null);
            Debug.Log($"노트 놓침! 코어 데미지 {missDamage}!");
        }
    }

    private void ShowJudgmentFeedback(string judgment)
    {
        if (judgmentText == null) return;
        StopAllCoroutines();
        judgmentText.text = judgment;
        switch (judgment)
        {
            case "Perfect": judgmentText.color = Color.yellow; break;
            case "Great": judgmentText.color = new Color(0.2f, 1f, 0.2f); break;
            case "Good": judgmentText.color = new Color(0.3f, 0.7f, 1f); break;
            case "Miss": judgmentText.color = Color.red; break;
        }
        judgmentText.gameObject.SetActive(true);
        StartCoroutine(HideTextDelay(0.5f));
    }

    private IEnumerator HideTextDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        judgmentText.gameObject.SetActive(false);
    }

    public void ApplyJudgmentBuff(float multiplier, float duration)
    {
        if (judgmentBuffCoroutine != null) StopCoroutine(judgmentBuffCoroutine);
        judgmentBuffCoroutine = StartCoroutine(JudgmentBuffCoroutine(multiplier, duration));
    }

    private IEnumerator JudgmentBuffCoroutine(float multiplier, float duration)
    {
        currentPerfectWindow = perfectWindow * multiplier;
        currentGreatWindow = greatWindow * multiplier;
        currentGoodWindow = goodWindow * multiplier;
        yield return new WaitForSeconds(duration);
        currentPerfectWindow = perfectWindow;
        currentGreatWindow = greatWindow;
        currentGoodWindow = goodWindow;
        judgmentBuffCoroutine = null;
    }

}