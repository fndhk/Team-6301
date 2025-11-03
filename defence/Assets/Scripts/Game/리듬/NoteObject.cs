// 파일 이름: NoteObject.cs (Image 컴포넌트 사용 버전)
using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트를 사용하기 위해 이 네임스페이스를 추가해야 합니다.

public class NoteObject : MonoBehaviour
{
    public float fallSpeed;
    public InstrumentType instrumentType;
    public int laneIndex;
    public float destroyYPosition;
    [Header("노트 타입")]
    public bool isSpecialNote = false; // 파란 노트 여부
    public Color normalColor = Color.yellow;
    public Color specialColor = Color.blue;

    // ▼▼▼ SpriteRenderer 대신 Image 변수로 변경 ▼▼▼
    private Image noteImage;

    void Awake()
    {
        // ▼▼▼ GetComponent<SpriteRenderer>() 대신 GetComponent<Image>()로 변경 ▼▼▼
        noteImage = GetComponent<Image>();

        // Image 컴포넌트가 있는지 먼저 확인합니다.
        if (noteImage == null)
        {
            Debug.LogError("NoteObject에 Image 컴포넌트가 없습니다! 프리팹을 확인해주세요.", gameObject);
            return; // 컴포넌트가 없으면 더 이상 진행하지 않아 에러를 방지합니다.
        }

        // ▼▼▼ spriteRenderer.color 대신 noteImage.color 로 색상 변경 ▼▼▼
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
                // 이 함수가 체력, 점수, 게이지 페널티를 모두 처리합니다.
                //RhythmInputManager.instance.ShowMissFeedback();

                RhythmInputManager.instance.ProcessPassedNotePenalty();
            }

            //  SkillManager 중복 호출 코드가 삭제되었습니다.

            Destroy(gameObject);
        }
    }
}

