using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public long currentScore { get; private set; }

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

    public void AddScore(int baseScore, float distance)
    {
        // 거리 보너스를 적용한 점수 추가 (거리가 멀수록 높은 점수)
        long scoreToAdd = Mathf.RoundToInt(baseScore * distance);
        currentScore += scoreToAdd;
        // Debug.Log($"점수 추가: {scoreToAdd} (기본: {baseScore}, 거리: {distance}), 총점: {currentScore}");
    }

    public int GetFinalScore(float healthPercentage)
    {
        // 최종 점수에 남은 체력 보너스(%) 적용
        return Mathf.RoundToInt(currentScore * healthPercentage);
    }

    public void ResetScore()
    {
        currentScore = 0;
    }
}