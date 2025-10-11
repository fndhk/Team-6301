// 파일 이름: NoteObject.cs (전체 교체)
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

        // 노트가 파괴될 위치에 도달하면
        if (transform.position.y < destroyYPosition)
        {
            // ▼▼▼ "Miss" 판정을 호출하는 코드를 추가합니다. ▼▼▼
            if (RhythmInputManager.instance != null)
            {
                RhythmInputManager.instance.ShowMissFeedback();
            }

            // 그 후에 스스로를 파괴합니다.
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 이 부분은 디버깅이 끝났다면 지워도 좋습니다.
        // Debug.Log($"[NoteObject] 내가 전달받은 속도: {fallSpeed}");
    }
}