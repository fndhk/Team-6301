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
        // �Ÿ� ���ʽ��� ������ ���� �߰� (�Ÿ��� �ּ��� ���� ����)
        long scoreToAdd = Mathf.RoundToInt(baseScore * distance);
        currentScore += scoreToAdd;
        // Debug.Log($"���� �߰�: {scoreToAdd} (�⺻: {baseScore}, �Ÿ�: {distance}), ����: {currentScore}");
    }

    public int GetFinalScore(float healthPercentage)
    {
        // ���� ������ ���� ü�� ���ʽ�(%) ����
        return Mathf.RoundToInt(currentScore * healthPercentage);
    }

    public void ResetScore()
    {
        currentScore = 0;
    }
}