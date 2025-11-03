// 파일 이름: NoteObject.cs (수정된 버전)
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

    private Image noteImage;

    void Awake()
    {
        // Awake에서는 Image 컴포넌트만 찾아둡니다.
        noteImage = GetComponent<Image>();

        if (noteImage == null)
        {
            Debug.LogError("NoteObject에 Image 컴포넌트가 없습니다! 프리팹을 확인해주세요.", gameObject);
        }
    }

    // ▼▼▼▼▼▼▼▼▼▼ 핵심 수정 ▼▼▼▼▼▼▼▼▼▼
    // 색상 결정 로직을 Awake()에서 Start()로 이동합니다.
    // Start() 함수는 NoteSpawner가 isSpecialNote 변수를 설정한 '이후'에 호출됩니다.
    void Start()
    {
        if (noteImage == null) return; // Awake에서 이미지를 못찾았다면 실행 중지

        // NoteSpawner에 의해 isSpecialNote 값이 정해진 후 색상을 설정합니다.
        if (isSpecialNote)
        {
            noteImage.color = specialColor;
        }
        else
        {
            noteImage.color = normalColor;
        }
    }
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

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