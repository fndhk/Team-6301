// 파일 이름: SkillManager.cs
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    [Header("게이지 설정")]
    public float maxGauge = 100f;
    [Header("게이지 획득량 설정")]
    public float pointsForPerfect = 10f;
    public float pointsForGreat = 5f;
    public float pointsForGood = 2f;
    [Tooltip("Miss일 때 깎이는 양입니다. 양수로 입력해주세요.")]
    public float pointsLostForMiss = 8f;
    private float currentGauge = 0f;
    private ItemEffect activeSkill; // 현재 캐릭터의 스킬

    [Header("UI 연결")]
    public Slider skillGaugeSlider;
    public Button skillButton; // UI 버튼으로도 스킬을 쓸 수 있게 연결
    public KeyCode skillKey = KeyCode.Space; // 스킬 사용 키
    private Image skillButtonImage;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // GameSession에서 현재 선택한 캐릭터의 스킬 정보를 가져옵니다.
        if (skillButton != null)
        {
            skillButtonImage = skillButton.GetComponent<Image>();
            skillButton.onClick.AddListener(TryUseSkill);
        }
        if (GameSession.instance != null && GameSession.instance.selectedCharacter != null)
        {
            activeSkill = GameSession.instance.selectedCharacter.characterSkill;
        }

        skillGaugeSlider.maxValue = maxGauge;
        skillGaugeSlider.value = 0;
        if (skillButton != null)
        {
            skillButton.onClick.AddListener(TryUseSkill);
            if (skillButtonImage != null && GameSession.instance.selectedCharacter.characterIcon != null)
            {
                skillButtonImage.sprite = GameSession.instance.selectedCharacter.characterIcon;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(skillKey))
        {
            TryUseSkill();
        }
    }

    // 판정 결과에 따라 게이지를 채우거나 깎는 함수
    public void AddGaugeOnJudgment(JudgmentManager.Judgment judgment)
    {
        float amount = 0;
        switch (judgment)
        {
            case JudgmentManager.Judgment.Perfect: amount = pointsForPerfect; break;
            case JudgmentManager.Judgment.Great: amount = pointsForGreat; break;
            case JudgmentManager.Judgment.Good: amount = pointsForGood; break;
            case JudgmentManager.Judgment.Miss: amount = -pointsLostForMiss; break; // 뺄셈 처리
        }
        currentGauge = Mathf.Clamp(currentGauge + amount, 0, maxGauge);
        skillGaugeSlider.value = currentGauge;
    }

    void TryUseSkill()
    {
        if (currentGauge >= maxGauge && activeSkill != null)
        {
            // ------ 신규 추가: 컷신 재생 ------
            if (SkillCutsceneUI.instance != null && GameSession.instance != null && GameSession.instance.selectedCharacter != null)
            {
                Sprite cutsceneSprite = GameSession.instance.selectedCharacter.skillCutsceneImage;
                if (cutsceneSprite != null)
                {
                    SkillCutsceneUI.instance.PlayCutscene(cutsceneSprite);
                }
            }

            // 스킬 효과 실행
            activeSkill.ExecuteEffect();

            // 게이지 초기화
            currentGauge = 0;
            skillGaugeSlider.value = currentGauge;

            Debug.Log("스킬 사용!");
        }
    }
}