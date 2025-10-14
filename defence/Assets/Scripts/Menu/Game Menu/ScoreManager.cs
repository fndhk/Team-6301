using UnityEngine;
using TMPro; // TextMeshPro�� ����ϱ� ���� �߰�

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public long currentScore { get; private set; }

    [Header("UI ����")]
    public TextMeshProUGUI scoreText; // ȭ�鿡 ������ ǥ���� UI �ؽ�Ʈ

    [Header("���� ���� ����")]
    [SerializeField] private int perfectScore = 50;
    [SerializeField] private int greatScore = 20;
    [SerializeField] private int goodScore = 5;
    [SerializeField] private int syncBonusScore = 500;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // ���� ���� �� ���� �ʱ�ȭ
        ResetScore();
    }

    // [����] ������ �߰��ϰ� UI�� ������Ʈ�ϴ� �߾� �Լ�
    private void AddToScore(long amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    // [�̸� ����] ������ AddScore �Լ�. ���� �� ��� ����.
    public void AddKillScore(int baseScore, float distance)
    {
        long scoreToAdd = Mathf.RoundToInt(baseScore * (1 + distance * 0.1f)); // �Ÿ��� 10 unit�� 100% ���ʽ�
        AddToScore(baseScore);
    }

    // [�� �Լ�] ���� ������ ���� ���� �߰�
    public void AddRhythmScore(JudgmentManager.Judgment judgment)
    {
        switch (judgment)
        {
            case JudgmentManager.Judgment.Perfect:
                AddToScore(perfectScore);
                break;
            case JudgmentManager.Judgment.Great:
                AddToScore(greatScore);
                break;
            case JudgmentManager.Judgment.Good:
                AddToScore(goodScore);
                break;
        }
    }

    // [�� �Լ�] ��ũ ���ʽ� ���� �߰�
    public void AddSyncBonusScore()
    {
        AddToScore(syncBonusScore);
        // TODO: "SYNC BONUS!" ���� UI �ǵ���� ����ָ� �����ϴ�.
    }

    // ���� ���� ��� (������ ����)
    public int GetFinalScore(float healthPercentage)
    {
        return Mathf.RoundToInt(currentScore * healthPercentage *103);
    }

    // ���� �ʱ�ȭ (UI ������Ʈ ȣ�� �߰�)
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }

    // [�� �Լ�] ���� UI ������Ʈ
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            // ���ڸ� 1,000 ���� �޸��� �����Ͽ� ǥ��
            scoreText.text = "SCORE: " + currentScore.ToString("N0");
        }
    }
}