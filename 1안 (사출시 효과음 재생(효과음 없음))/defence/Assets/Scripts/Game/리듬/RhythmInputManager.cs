// ���� �̸�: RhythmInputManager.cs (��ü ��ü - ���� ���)
using UnityEngine;
using TMPro;
using System.Collections; // �ڷ�ƾ�� ����ϱ� ���� �߰�

public class RhythmInputManager : MonoBehaviour
{
    public static RhythmInputManager instance;

    [Header("�Է� ����")]
    public KeyCode[] inputKeys = { KeyCode.A, KeyCode.S, KeyCode.Semicolon, KeyCode.Quote };
    public GameObject[] keyPressFeedbacks;

    [Header("���� ����")]
    public Transform hitZone;
    public float perfectWindow = 0.5f;
    public float greatWindow = 1.0f;
    public float goodWindow = 1.5f;

    [Header("���� �ǵ�� UI")]
    // ������ ��� ���� �ִ� TextMeshProUGUI ������Ʈ�� ���� �����մϴ�.
    public TextMeshProUGUI judgmentText;

    [Header("Ÿ�� ����")]
    public BaseTower[] correspondingTowers;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // ���� ���� �� �ؽ�Ʈ�� Ȯ���� ����ϴ�.
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

                ShowJudgmentFeedback(judgmentString); // ��ġ ������ ���� �ʿ� �����ϴ�.
                Destroy(closestNote.gameObject);
            }
        }
    }

    public void ShowMissFeedback() // ��ġ ������ ���� �ʿ� �����ϴ�.
    {
        ShowJudgmentFeedback("Miss");
    }

    // ���� �ؽ�Ʈ�� �����ִ� ������ �Լ�
    private void ShowJudgmentFeedback(string judgment)
    {
        if (judgmentText == null) return;

        // Ȥ�� ���� ���� ����� �ڷ�ƾ�� �ִٸ� ����ϴ�. (�ؽ�Ʈ�� �����̴� ���� ����)
        StopAllCoroutines();

        // 1. �ؽ�Ʈ ����� ���� ����
        judgmentText.text = judgment;
        switch (judgment)
        {
            case "Perfect": judgmentText.color = Color.yellow; break;
            case "Great": judgmentText.color = new Color(0.2f, 1f, 0.2f); break;
            case "Good": judgmentText.color = new Color(0.3f, 0.7f, 1f); break;
            case "Miss": judgmentText.color = Color.red; break;
        }

        // 2. �ؽ�Ʈ ������Ʈ�� Ȱ��ȭ�ؼ� �����ݴϴ�.
        judgmentText.gameObject.SetActive(true);

        // 3. 0.5�� �ڿ� �ؽ�Ʈ�� ����� �ڷ�ƾ�� �����մϴ�.
        StartCoroutine(HideTextDelay(0.5f));
    }

    // ������ �ð� �Ŀ� �ؽ�Ʈ�� ����� �ڷ�ƾ
    private IEnumerator HideTextDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        judgmentText.gameObject.SetActive(false);
    }
}

// NoteObject.cs�� �ణ �����ؾ� �մϴ�.
// ShowMissFeedback() �Լ��� ����(argument)�� ���� �ʵ��� ����Ǿ��� �����Դϴ�.
// RhythmInputManager.instance.ShowMissFeedback(); ���� ȣ�����ּ���.