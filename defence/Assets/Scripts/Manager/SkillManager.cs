// 파일 이름: SkillManager.cs (수정 완료 버전)
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
    private ItemEffect activeSkill;
    private CharacterData activeCharacter;
    [Header("UI 연결")]
    public Slider skillGaugeSlider;
    public Button skillButton;
    public KeyCode skillKey = KeyCode.Space;
    private Image skillButtonImage;
    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        // AudioSource 컴포넌트를 찾거나, 없으면 새로 추가합니다.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; // 게임 시작 시 자동 재생 방지
        }
    }

    void Start()
    {
        if (GameSession.instance != null && GameSession.instance.selectedCharacter != null)
        {
            activeCharacter = GameSession.instance.selectedCharacter;
            activeSkill = activeCharacter.characterSkill; // 스킬은 activeCharacter에서 가져옵니다.
        }

        skillGaugeSlider.maxValue = maxGauge;
        skillGaugeSlider.value = 0;
        if (skillButton != null)
        {
            skillButtonImage = skillButton.GetComponent<Image>();
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

    public void AddGaugeOnJudgment(JudgmentManager.Judgment judgment)
    {
        float amount = 0;
        switch (judgment)
        {
            case JudgmentManager.Judgment.Perfect: amount = pointsForPerfect; break;
            case JudgmentManager.Judgment.Great: amount = pointsForGreat; break;
            case JudgmentManager.Judgment.Good: amount = pointsForGood; break;
            case JudgmentManager.Judgment.Miss: amount = -pointsLostForMiss; break;
        }

        // 자동 스킬이 활성화 상태(게이지 100)일 때는 게이지가 오르지 않음
        if (activeSkill != null && activeSkill.isAutomatic && currentGauge >= maxGauge)
        {
            return;
        }

        // ▼▼▼ isAutomatic 변수를 한 번만 체크하도록 로직을 정리했습니다 ▼▼▼
        bool wasGaugeFull = currentGauge >= maxGauge; // 게이지가 차기 전 상태 저장

        currentGauge = Mathf.Clamp(currentGauge + amount, 0, maxGauge);
        skillGaugeSlider.value = currentGauge;

        // 게이지가 꽉 차지 않았다가 -> 꽉 찼고, 현재 스킬이 '자동 스킬'이라면
        if (!wasGaugeFull && currentGauge >= maxGauge && activeSkill != null && activeSkill.isAutomatic)
        {
            // ▼▼▼ 컷신 재생 코드를 여기에 추가합니다 ▼▼▼
            if (SkillCutsceneUI.instance != null && GameSession.instance != null && GameSession.instance.selectedCharacter != null)
            {
                Sprite cutsceneSprite = GameSession.instance.selectedCharacter.skillCutsceneImage;
                if (cutsceneSprite != null)
                {
                    SkillCutsceneUI.instance.PlayCutscene(cutsceneSprite);
                }
            }
            if (activeCharacter != null && activeCharacter.skillSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(activeCharacter.skillSound);
            }
            // 스킬 효과 실행
            activeSkill.ExecuteEffect();
            Debug.Log("자동 스킬 발동!");
        }
    }

    public void ConsumeGauge(float amount)
    {
        currentGauge -= amount;
        if (currentGauge < 0) currentGauge = 0;
        skillGaugeSlider.value = currentGauge;
    }

    public float GetCurrentGauge()
    {
        return currentGauge;
    }

    void TryUseSkill()
    {
        // ▼▼▼ 자동 스킬(isAutomatic)일 경우 수동으로 사용할 수 없도록 조건을 추가했습니다 ▼▼▼
        if (currentGauge >= maxGauge && activeSkill != null && !activeSkill.isAutomatic)
        {
            if (SkillCutsceneUI.instance != null && GameSession.instance != null && GameSession.instance.selectedCharacter != null)
            {
                Sprite cutsceneSprite = GameSession.instance.selectedCharacter.skillCutsceneImage;
                if (cutsceneSprite != null)
                {
                    SkillCutsceneUI.instance.PlayCutscene(cutsceneSprite);
                }
            }
            if (activeCharacter != null && activeCharacter.skillSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(activeCharacter.skillSound);
            }

            activeSkill.ExecuteEffect();

            // 게이지 초기화
            currentGauge = 0;
            skillGaugeSlider.value = currentGauge;

            Debug.Log("스킬 사용!");
        }
    }
}