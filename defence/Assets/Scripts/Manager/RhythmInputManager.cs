// ���� �̸�: RhythmInputManager.cs (���� ���� ���� ����)
using UnityEngine;
using TMPro;
using System.Collections;
using static JudgmentManager;

public class RhythmInputManager : MonoBehaviour
{
    public static RhythmInputManager instance;

    [Header("�Է� ����")]
    public KeyCode[] inputKeys = { KeyCode.A, KeyCode.S, KeyCode.Semicolon, KeyCode.Quote };
    public GameObject[] keyPressFeedbacks;

    [Header("���� ����")]
    public Transform[] hitZone;
    public float perfectWindow = 0.5f;
    public float greatWindow = 1.0f;
    public float goodWindow = 1.5f;

    [Header("���� �ǵ�� UI")]
    public TextMeshProUGUI judgmentText;

    [Header("Ÿ�� ����")]
    public BaseTower[] correspondingTowers;

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
    }

    void Update()
    {
        for (int i = 0; i < inputKeys.Length; i++)
        {
            if (Input.GetKeyDown(inputKeys[i]))
            {
                if (keyPressFeedbacks.Length > i) keyPressFeedbacks[i].SetActive(true);
                CheckForNote(i); // Ű���� �迭 ����(0,1,2,3)�� �״�� ����
            }

            if (Input.GetKeyUp(inputKeys[i]))
            {
                if (keyPressFeedbacks.Length > i) keyPressFeedbacks[i].SetActive(false);
            }
        }
    }

    private void CheckForNote(int laneIndex)
    {

        Debug.Log($"<color=yellow>Ű �Է�:</color> {laneIndex}�� Ű ����");

        Collider2D[] hitNotes = Physics2D.OverlapCircleAll(hitZone[laneIndex].position, goodWindow);
        NoteObject closestNote = null;
        float minDistance = float.MaxValue;

        foreach (var noteCollider in hitNotes)
        {
            NoteObject note = noteCollider.GetComponent<NoteObject>();
            if (note != null)
            {
                Debug.Log($"[���� ����] ��Ʈ �߰�! -> ��Ʈ ����: {note.laneIndex} / ���� Ű: {laneIndex}");
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

            if (minDistance <= perfectWindow) { judgmentEnum = JudgmentManager.Judgment.Perfect; judgmentString = "Perfect"; }
            else if (minDistance <= greatWindow) { judgmentEnum = JudgmentManager.Judgment.Great; judgmentString = "Great"; }
            else if (minDistance <= goodWindow) { judgmentEnum = JudgmentManager.Judgment.Good; judgmentString = "Good"; }

            // Ư�� ��Ʈ�̰� Great �̻��̸� Ƽ�� ����
            if (closestNote.isSpecialNote && judgmentEnum <= JudgmentManager.Judgment.Great)
            {
                SaveLoadManager.instance.gameData.gachaTickets++;
                Debug.Log("Ư�� ��Ʈ ȹ��! Ƽ�� +1");
            }

            if (judgmentEnum != JudgmentManager.Judgment.Miss)
            {
                SkillManager.instance.AddGaugeOnJudgment(judgmentEnum);

                if (judgmentEnum == JudgmentManager.Judgment.Perfect || judgmentEnum == JudgmentManager.Judgment.Great)
                {
                    if (AudioManager.instance != null)
                    {
                        AudioManager.instance.PlayInstrumentSound(closestNote.instrumentType);
                    }
                }
                ShowJudgmentFeedback(judgmentString);
                Destroy(closestNote.gameObject);
            }
        }
    }

    // ���� ���߷ȴ� �Լ����� �ٽ� �߰��մϴ�. ����

    public void ShowMissFeedback()
    {
        ShowJudgmentFeedback("Miss");
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
}