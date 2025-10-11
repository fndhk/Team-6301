// ���� �̸�: NoteObject.cs (��ü ��ü)
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public float fallSpeed;
    public InstrumentType instrumentType;
    public int laneIndex;
    public float destroyYPosition;

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        // ��Ʈ�� �ı��� ��ġ�� �����ϸ�
        if (transform.position.y < destroyYPosition)
        {
            // ���� "Miss" ������ ȣ���ϴ� �ڵ带 �߰��մϴ�. ����
            if (RhythmInputManager.instance != null)
            {
                RhythmInputManager.instance.ShowMissFeedback();
            }

            // �� �Ŀ� �����θ� �ı��մϴ�.
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // �� �κ��� ������� �����ٸ� ������ �����ϴ�.
        // Debug.Log($"[NoteObject] ���� ���޹��� �ӵ�: {fallSpeed}");
    }
}