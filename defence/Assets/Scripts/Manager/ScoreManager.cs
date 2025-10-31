// 파일 이름: ScoreManager.cs (실시간 최종 점수 계산 버전)
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public long totalKillScore { get; private set; }
    private long totalJudgmentPoints = 0;
    private int totalJudgmentCount = 0;

    [Header("UI 연결")]
    public TextMeshProUGUI scoreText; // "SCORE: " 텍스트

    [Header("리듬 점수 설정")]
    [SerializeField] private int perfectScore = 50;
    [SerializeField] private int greatScore = 20;
    [SerializeField] private int goodScore = 5;
    [SerializeField] private int syncBonusScore = 500;

    //  실시간 계산을 위해 CoreFacility 참조
    private CoreFacility coreFacility;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ResetScore();

        //  CoreFacility를 찾아 변수에 저장
        coreFacility = FindFirstObjectByType<CoreFacility>();
        if (coreFacility == null)
        {
            Debug.LogError("ScoreManager: CoreFacility를 찾을 수 없습니다!");
        }
    }

    // ---  실시간 점수 계산 (핵심) ---
    void Update()
    {
        if (coreFacility == null) return;

        // 1. 평균 리듬 점수 계산
        double averageRhythmScore = 0.0;
        if (totalJudgmentCount > 0)
        {
            averageRhythmScore = (double)totalJudgmentPoints / totalJudgmentCount;
        }

        // 2. 현재 체력 % 계산
        float healthPercentage = coreFacility.GetCurrentHealthPercentage();

        // 3. 실시간 최종 점수 계산
        double currentFinalScore = (double)totalKillScore * averageRhythmScore * (double)healthPercentage * 3.0;

        // 4. UI 업데이트
        // (A)  "SCORE: " 텍스트는 이제 업데이트하지 않습니다. (아래 3줄 삭제 또는 주석 처리)
        /*
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + totalKillScore.ToString("N0");
        }
        */

        // (B) 재화 UI(MaterialsUI)는 실시간 최종 점수를 계속 표시
        if (MaterialsUI.instance != null)
        {
            MaterialsUI.instance.OnScoreChanged((long)currentFinalScore);
        }
    }

    // ---  점수 추가 함수들은 이제 UI 업데이트를 호출하지 않음 ---

    private void AddKillScoreToTotal(long amount)
    {
        totalKillScore += amount;
        // Update()가 처리하므로 UpdateScoreUI() 호출 삭제
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
        // Update()가 처리하므로 UI 업데이트 호출 필요 없음
    }

    public void AddSyncBonusScore()
    {
        AddKillScoreToTotal(syncBonusScore);
        // Update()가 처리하므로 UpdateScoreUI() 호출 삭제
    }

    //  최종 점수 계산 (GameManager가 마지막에 호출)
    public int GetFinalScore(float healthPercentage)
    {
        // 이 로직은 Update()의 계산과 동일
        double averageRhythmScore = 0.0;
        if (totalJudgmentCount > 0)
        {
            averageRhythmScore = (double)totalJudgmentPoints / totalJudgmentCount;
        }

        double finalScore = (double)totalKillScore * averageRhythmScore * (double)healthPercentage * 3.0;
        return Mathf.RoundToInt((float)finalScore);
    }

    public void ResetScore()
    {
        totalKillScore = 0;
        totalJudgmentPoints = 0;
        totalJudgmentCount = 0;
        // Update()가 처리하므로 UpdateScoreUI() 호출 삭제
    }

    //  UpdateScoreUI() 함수는 이제 Update() 함수로 통합되었으므로 삭제
}