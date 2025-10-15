// ���� �̸�: SkillManager.cs
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    [Header("������ ����")]
    public float maxGauge = 100f;
    [Header("������ ȹ�淮 ����")]
    public float pointsForPerfect = 10f;
    public float pointsForGreat = 5f;
    public float pointsForGood = 2f;
    [Tooltip("Miss�� �� ���̴� ���Դϴ�. ����� �Է����ּ���.")]
    public float pointsLostForMiss = 8f;
    private float currentGauge = 0f;
    private ItemEffect activeSkill; // ���� ĳ������ ��ų

    [Header("UI ����")]
    public Slider skillGaugeSlider;
    public Button skillButton; // UI ��ư���ε� ��ų�� �� �� �ְ� ����
    public KeyCode skillKey = KeyCode.Space; // ��ų ��� Ű
    private Image skillButtonImage;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // GameSession���� ���� ������ ĳ������ ��ų ������ �����ɴϴ�.
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

    // ���� ����� ���� �������� ä��ų� ��� �Լ�
    public void AddGaugeOnJudgment(JudgmentManager.Judgment judgment)
    {
        float amount = 0;
        switch (judgment)
        {
            case JudgmentManager.Judgment.Perfect: amount = pointsForPerfect; break;
            case JudgmentManager.Judgment.Great: amount = pointsForGreat; break;
            case JudgmentManager.Judgment.Good: amount = pointsForGood; break;
            case JudgmentManager.Judgment.Miss: amount = -pointsLostForMiss; break; // ���� ó��
        }
        currentGauge = Mathf.Clamp(currentGauge + amount, 0, maxGauge);
        skillGaugeSlider.value = currentGauge;
    }

    void TryUseSkill()
    {
        if (currentGauge >= maxGauge && activeSkill != null)
        {
            // ------ �ű� �߰�: �ƽ� ��� ------
            if (SkillCutsceneUI.instance != null && GameSession.instance != null && GameSession.instance.selectedCharacter != null)
            {
                Sprite cutsceneSprite = GameSession.instance.selectedCharacter.skillCutsceneImage;
                if (cutsceneSprite != null)
                {
                    SkillCutsceneUI.instance.PlayCutscene(cutsceneSprite);
                }
            }

            // ��ų ȿ�� ����
            activeSkill.ExecuteEffect();

            // ������ �ʱ�ȭ
            currentGauge = 0;
            skillGaugeSlider.value = currentGauge;

            Debug.Log("��ų ���!");
        }
    }
}