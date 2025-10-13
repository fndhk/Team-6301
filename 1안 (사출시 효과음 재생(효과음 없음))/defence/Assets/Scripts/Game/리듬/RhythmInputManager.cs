// 파일 이름: RhythmInputManager.cs (전체 교체 - 재사용 방식)
using UnityEngine;
using TMPro;
using System.Collections; // 코루틴을 사용하기 위해 추가

public class RhythmInputManager : MonoBehaviour
{
    public static RhythmInputManager instance;

    [Header("입력 설정")]
    public KeyCode[] inputKeys = { KeyCode.A, KeyCode.S, KeyCode.Semicolon, KeyCode.Quote };
    public GameObject[] keyPressFeedbacks;

    [Header("판정 설정")]
    public Transform hitZone;
    public float perfectWindow = 0.5f;
    public float greatWindow = 1.0f;
    public float goodWindow = 1.5f;

    [Header("판정 피드백 UI")]
    // 프리팹 대신 씬에 있는 TextMeshProUGUI 오브젝트를 직접 연결합니다.
    public TextMeshProUGUI judgmentText;

    [Header("타워 연결")]
    public BaseTower[] correspondingTowers;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // 게임 시작 시 텍스트를 확실히 숨깁니다.
    void Start()
    {
        if (judgmentText != null)
        {
            judgmentText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
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

    private void CheckForNote(int laneIndex)
    {
        Collider2D[] hitNotes = Physics2D.OverlapCircleAll(hitZone.position, goodWindow);
        NoteObject closestNote = null;
        float minDistance = float.MaxValue;

        foreach (var noteCollider in hitNotes)
        {
            NoteObject note = noteCollider.GetComponent<NoteObject>();
            if (note != null && note.laneIndex == laneIndex)
            {
                float distance = Mathf.Abs(note.transform.position.y - hitZone.position.y);
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

            if (minDistance <= perfectWindow) { judgmentEnum = JudgmentManager.Judgment.Perfect; judgmentString = "Perfect"; }
            else if (minDistance <= greatWindow) { judgmentEnum = JudgmentManager.Judgment.Great; judgmentString = "Great"; }
            else if (minDistance <= goodWindow) { judgmentEnum = JudgmentManager.Judgment.Good; judgmentString = "Good"; }

            if (judgmentEnum != JudgmentManager.Judgment.Miss)
            {
                BaseTower towerToFire = correspondingTowers[laneIndex];
                if (towerToFire != null && towerToFire.gameObject.activeSelf)
                {
                    towerToFire.FireProjectile(judgmentEnum);
                }

                if (judgmentEnum == JudgmentManager.Judgment.Perfect || judgmentEnum == JudgmentManager.Judgment.Great)
                {
                    if (AudioManager.instance != null)
                    {
                        AudioManager.instance.PlayInstrumentSound(closestNote.instrumentType);
                    }
                }

                ShowJudgmentFeedback(judgmentString); // 위치 정보는 이제 필요 없습니다.
                Destroy(closestNote.gameObject);
            }
        }
    }

    public void ShowMissFeedback() // 위치 정보는 이제 필요 없습니다.
    {
        ShowJudgmentFeedback("Miss");
    }

    // 판정 텍스트를 보여주는 수정된 함수
    private void ShowJudgmentFeedback(string judgment)
    {
        if (judgmentText == null) return;

        // 혹시 실행 중인 숨기기 코루틴이 있다면 멈춥니다. (텍스트가 깜빡이는 것을 방지)
        StopAllCoroutines();

        // 1. 텍스트 내용과 색상 설정
        judgmentText.text = judgment;
        switch (judgment)
        {
            case "Perfect": judgmentText.color = Color.yellow; break;
            case "Great": judgmentText.color = new Color(0.2f, 1f, 0.2f); break;
            case "Good": judgmentText.color = new Color(0.3f, 0.7f, 1f); break;
            case "Miss": judgmentText.color = Color.red; break;
        }

        // 2. 텍스트 오브젝트를 활성화해서 보여줍니다.
        judgmentText.gameObject.SetActive(true);

        // 3. 0.5초 뒤에 텍스트를 숨기는 코루틴을 시작합니다.
        StartCoroutine(HideTextDelay(0.5f));
    }

    // 지정된 시간 후에 텍스트를 숨기는 코루틴
    private IEnumerator HideTextDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        judgmentText.gameObject.SetActive(false);
    }
}

// NoteObject.cs도 약간 수정해야 합니다.
// ShowMissFeedback() 함수가 인자(argument)를 받지 않도록 변경되었기 때문입니다.
// RhythmInputManager.instance.ShowMissFeedback(); 으로 호출해주세요.