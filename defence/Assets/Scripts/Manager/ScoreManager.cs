using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public long currentScore { get; private set; }

    [Header("UI 연결")]
    public TextMeshProUGUI scoreText; // 화면에 점수를 표시할 UI 텍스트

    [Header("리듬 점수 설정")]
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
        // 게임 시작 시 점수 초기화
        ResetScore();
    }

    // [수정] 점수를 추가하고 UI를 업데이트하는 중앙 함수
    private void AddToScore(long amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    // [이름 변경] 기존의 AddScore 함수. 이제 적 사살 전용.
    public void AddKillScore(int baseScore, float distance)
    {
        long scoreToAdd = Mathf.RoundToInt(baseScore * (1 + distance * 0.1f)); // 거리가 10 unit당 100% 보너스
        AddToScore(baseScore);
    }

    // [새 함수] 리듬 판정에 따른 점수 추가
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

    // [새 함수] 싱크 보너스 점수 추가
    public void AddSyncBonusScore()
    {
        AddToScore(syncBonusScore);
        // TODO: "SYNC BONUS!" 같은 UI 피드백을 띄워주면 좋습니다.
    }

    // 최종 점수 계산 (기존과 동일)
    public int GetFinalScore(float healthPercentage)
    {
        return Mathf.RoundToInt(currentScore * healthPercentage *103);
    }

    // 점수 초기화 (UI 업데이트 호출 추가)
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }

    // [새 함수] 점수 UI 업데이트
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            // 숫자를 1,000 단위 콤마로 구분하여 표시
            scoreText.text = "SCORE: " + currentScore.ToString("N0");
        }
    }
}