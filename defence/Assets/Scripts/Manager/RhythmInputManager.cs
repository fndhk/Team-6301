// 파일 이름: RhythmInputManager.cs (오류 수정 최종 버전)
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
    public int missDamage = 5;
    
    // --- 판정 강화용 변수 ---
    private float currentPerfectWindow;
    private float currentGreatWindow;
    private float currentGoodWindow;
    private Coroutine judgmentBuffCoroutine; // 버프 지속시간을 제어할 코루틴
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (judgmentText != null)
        {
            judgmentText.gameObject.SetActive(false);
        }
        
        currentPerfectWindow = perfectWindow;
        currentGreatWindow = greatWindow;
        currentGoodWindow = goodWindow;
    }

    void Update()
    {
        for (int i = 0; i < inputKeys.Length; i++)
        {
            if (Input.GetKeyDown(inputKeys[i]))
            {
                if (keyPressFeedbacks.Length > i) keyPressFeedbacks[i].SetActive(true);
                CheckForNote(i); // 키보드 배열 순서(0,1,2,3)를 그대로 전달
            }

            if (Input.GetKeyUp(inputKeys[i]))
            {
                if (keyPressFeedbacks.Length > i) keyPressFeedbacks[i].SetActive(false);
            }
        }
    }

    private void CheckForNote(int laneIndex)
    {

        Debug.Log($"<color=yellow>키 입력:</color> {laneIndex}번 키 눌림");

        Collider2D[] hitNotes = Physics2D.OverlapCircleAll(hitZone[laneIndex].position, currentGoodWindow);
        NoteObject closestNote = null;
        float minDistance = float.MaxValue;

        foreach (var noteCollider in hitNotes)
        {
            NoteObject note = noteCollider.GetComponent<NoteObject>();
            if (note != null)
            {
                Debug.Log($"[판정 범위] 노트 발견! -> 노트 레인: {note.laneIndex} / 눌린 키: {laneIndex}");
            }

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


            // ---  수정된 판정 로직 ---
            if (judgmentEnum == JudgmentManager.Judgment.Miss)
            {
                // Case 1: 노트를 너무 일찍/늦게 쳐서 Miss
                ShowMissFeedback(); // Miss에 대한 모든 페널티(체력, 점수, 게이지) 처리
            }
            else
            {
                // Case 2: Good, Great, Perfect 성공
                // 점수 기록
                ScoreManager.instance.AddRhythmScore(judgmentEnum);
                // 게이지 획득
                SkillManager.instance.AddGaugeOnJudgment(judgmentEnum);

                // 사운드 재생
                if (judgmentEnum == JudgmentManager.Judgment.Perfect || judgmentEnum == JudgmentManager.Judgment.Great)
                {
                    if (AudioManager.instance != null)
                    {
                        AudioManager.instance.PlayInstrumentSound(closestNote.instrumentType);
                    }
                }

                // 피드백 및 노트 제거
                ShowJudgmentFeedback(judgmentString);
                Destroy(closestNote.gameObject);

                // 특수 노트 처리
                if (closestNote.isSpecialNote && judgmentEnum <= JudgmentManager.Judgment.Great)
                {
                    SaveLoadManager.instance.gameData.gachaTickets++;
                    Debug.Log("특수 노트 획득! 티켓 +1");
                }
            }
        }
        else
        {
            // Case 3: 허공에 키를 누름 (Miss)
            ShowMissFeedback(); // Miss에 대한 모든 페널티(체력, 점수, 게이지) 처리
        }
    }

    // ▼▼▼ 빠뜨렸던 함수들을 다시 추가합니다. ▼▼▼

    public void ShowMissFeedback()
    {
        ShowJudgmentFeedback("Miss");

        // 1. 코어 체력 5 감소 (기존 로직)
        if (coreFacility != null)
        {
            coreFacility.TakeDamage(missDamage, null);
        }

        // 2.  ScoreManager에 Miss 기록 (새 로직)
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddRhythmScore(JudgmentManager.Judgment.Miss);
        }

        // 3.  SkillManager에 게이지 페널티 적용 (새 로직)
        if (SkillManager.instance != null)
        {
            SkillManager.instance.AddGaugeOnJudgment(JudgmentManager.Judgment.Miss);
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
        // 이미 버프가 걸려있다면, 기존 버프를 중지하고 새로 시작
        if (judgmentBuffCoroutine != null)
        {
            StopCoroutine(judgmentBuffCoroutine);
        }
        judgmentBuffCoroutine = StartCoroutine(JudgmentBuffCoroutine(multiplier, duration));
    }

    private IEnumerator JudgmentBuffCoroutine(float multiplier, float duration)
    {
        Debug.Log($"<color=cyan>판정 강화 시작! (x{multiplier}, {duration}초)</color>");

        // 버프 적용: 현재 판정 범위를 기본값 * 배율로 설정
        currentPerfectWindow = perfectWindow * multiplier;
        currentGreatWindow = greatWindow * multiplier;
        currentGoodWindow = goodWindow * multiplier;

        // 지속시간만큼 대기
        yield return new WaitForSeconds(duration);

        // 버프 종료: 모든 값을 다시 기본값으로 복구
        Debug.Log("판정 강화 종료.");
        currentPerfectWindow = perfectWindow;
        currentGreatWindow = greatWindow;
        currentGoodWindow = goodWindow;

        judgmentBuffCoroutine = null;
    }
}