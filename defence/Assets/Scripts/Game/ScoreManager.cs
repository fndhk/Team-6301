// ���� �̸�: ScoreManager.cs (�ǽð� ���� ���� ��� ����)
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public long totalKillScore { get; private set; }
    private long totalJudgmentPoints = 0;
    private int totalJudgmentCount = 0;

    [Header("UI ����")]
    public TextMeshProUGUI scoreText; // "SCORE: " �ؽ�Ʈ

    [Header("���� ���� ����")]
    [SerializeField] private int perfectScore = 50;
    [SerializeField] private int greatScore = 20;
    [SerializeField] private int goodScore = 5;
    [SerializeField] private int syncBonusScore = 500;

    //  �ǽð� ����� ���� CoreFacility ����
    private CoreFacility coreFacility;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ResetScore();

        //  CoreFacility�� ã�� ������ ����
        coreFacility = FindFirstObjectByType<CoreFacility>();
        if (coreFacility == null)
        {
            Debug.LogError("ScoreManager: CoreFacility�� ã�� �� �����ϴ�!");
        }
    }

    // ---  �ǽð� ���� ��� (�ٽ�) ---
    void Update()
    {
        if (coreFacility == null) return;

        // 1. ��� ���� ���� ���
        double averageRhythmScore = 0.0;
        if (totalJudgmentCount > 0)
        {
            averageRhythmScore = (double)totalJudgmentPoints / totalJudgmentCount;
        }

        // 2. ���� ü�� % ���
        float healthPercentage = coreFacility.GetCurrentHealthPercentage();

        // 3. �ǽð� ���� ���� ���
        double currentFinalScore = (double)totalKillScore * averageRhythmScore * (double)healthPercentage * 0.3;

        // 4. UI ������Ʈ
        // (A)  "SCORE: " �ؽ�Ʈ�� ���� ������Ʈ���� �ʽ��ϴ�. (�Ʒ� 3�� ���� �Ǵ� �ּ� ó��)
        /*
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + totalKillScore.ToString("N0");
        }
        */

        // (B) ��ȭ UI(MaterialsUI)�� �ǽð� ���� ������ ��� ǥ��
        if (MaterialsUI.instance != null)
        {
            MaterialsUI.instance.OnScoreChanged((long)currentFinalScore);
        }
    }

    // ---  ���� �߰� �Լ����� ���� UI ������Ʈ�� ȣ������ ���� ---

    private void AddKillScoreToTotal(long amount)
    {
        totalKillScore += amount;
        // Update()�� ó���ϹǷ� UpdateScoreUI() ȣ�� ����
    }

    public void AddKillScore(int baseScore, float distance)
    {
        long scoreToAdd = Mathf.RoundToInt(baseScore * (1 + distance * 0.1f));
        AddKillScoreToTotal(scoreToAdd);
    }

    public void AddRhythmScore(JudgmentManager.Judgment judgment)
    {
        totalJudgmentCount++;
        switch (judgment)
        {
            case JudgmentManager.Judgment.Perfect: totalJudgmentPoints += 100; break;
            case JudgmentManager.Judgment.Great: totalJudgmentPoints += 90; break;
            case JudgmentManager.Judgment.Good: totalJudgmentPoints += 70; break;
            case JudgmentManager.Judgment.Miss: totalJudgmentPoints += 0; break;
        }
        // Update()�� ó���ϹǷ� UI ������Ʈ ȣ�� �ʿ� ����
    }

    public void AddSyncBonusScore()
    {
        AddKillScoreToTotal(syncBonusScore);
        // Update()�� ó���ϹǷ� UpdateScoreUI() ȣ�� ����
    }

    //  ���� ���� ��� (GameManager�� �������� ȣ��)
    public int GetFinalScore(float healthPercentage)
    {
        // �� ������ Update()�� ���� ����
        double averageRhythmScore = 0.0;
        if (totalJudgmentCount > 0)
        {
            averageRhythmScore = (double)totalJudgmentPoints / totalJudgmentCount;
        }

        double finalScore = (double)totalKillScore * averageRhythmScore * (double)healthPercentage * 0.3;
        return Mathf.RoundToInt((float)finalScore);
    }

    public void ResetScore()
    {
        totalKillScore = 0;
        totalJudgmentPoints = 0;
        totalJudgmentCount = 0;
        // Update()�� ó���ϹǷ� UpdateScoreUI() ȣ�� ����
    }

    /// <summary>
    /// 현재 리듬 게임 정확도를 0~100 사이의 float 값으로 반환합니다.
    /// 판정이 없으면 100을 반환합니다.
    /// </summary>
    public float GetAverageRhythmAccuracy()
    {
        if (totalJudgmentCount <= 0)
        {
            return 100f; // 판정이 없으면 100% (게임 시작 시)
        }

        float averageScore = (float)totalJudgmentPoints / totalJudgmentCount;
        return averageScore; // 0~100 사이의 값
    }

    //  UpdateScoreUI() �Լ��� ���� Update() �Լ��� ���յǾ����Ƿ� ����
}