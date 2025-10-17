// ���� �̸�: NoteObject.cs (Image ������Ʈ ��� ����)
using UnityEngine;
using UnityEngine.UI; // Image ������Ʈ�� ����ϱ� ���� �� ���ӽ����̽��� �߰��ؾ� �մϴ�.

public class NoteObject : MonoBehaviour
{
    public float fallSpeed;
    public InstrumentType instrumentType;
    public int laneIndex;
    public float destroyYPosition;
    [Header("��Ʈ Ÿ��")]
    public bool isSpecialNote = false; // �Ķ� ��Ʈ ����
    public Color normalColor = Color.yellow;
    public Color specialColor = Color.blue;

    // ���� SpriteRenderer ��� Image ������ ���� ����
    private Image noteImage;

    void Awake()
    {
        // ���� GetComponent<SpriteRenderer>() ��� GetComponent<Image>()�� ���� ����
        noteImage = GetComponent<Image>();

        // Image ������Ʈ�� �ִ��� ���� Ȯ���մϴ�.
        if (noteImage == null)
        {
            Debug.LogError("NoteObject�� Image ������Ʈ�� �����ϴ�! �������� Ȯ�����ּ���.", gameObject);
            return; // ������Ʈ�� ������ �� �̻� �������� �ʾ� ������ �����մϴ�.
        }

        // ���� spriteRenderer.color ��� noteImage.color �� ���� ���� ����
        if (isSpecialNote)
        {
            noteImage.color = specialColor;
        }
        else
        {
            noteImage.color = normalColor;
        }
    }

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < destroyYPosition)
        {
            if (RhythmInputManager.instance != null)
            {
                RhythmInputManager.instance.ShowMissFeedback();
            }
            if (SkillManager.instance != null)
            {
                SkillManager.instance.AddGaugeOnJudgment(JudgmentManager.Judgment.Miss);
            }
            Destroy(gameObject);
        }
    }
}

