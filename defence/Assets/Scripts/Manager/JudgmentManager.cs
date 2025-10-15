// 파일 이름: JudgmentManager.cs
using UnityEngine;
using TMPro;
using System.Collections.Generic; // List를 사용하기 위해 추가

public class JudgmentManager : MonoBehaviour
{
    public static JudgmentManager instance;

    public enum Judgment { Perfect, Great, Good, Miss }

    [Header("판정 보너스 설정")]
    [SerializeField] private float perfectMultiplier = 1.5f;
    [SerializeField] private float greatMultiplier = 1.1f;
    [SerializeField] private float goodMultiplier = 1.0f;
    [SerializeField] private float missMultiplier = 0.8f;

    [Header("판정 시간창 (단위: 초)")]
    [SerializeField] private float perfectWindow = 0.05f;
    [SerializeField] private float greatWindow = 0.1f;
    [SerializeField] private float goodWindow = 0.2f;

    [Header("UI 피드백 설정")]
    [SerializeField] private GameObject judgmentTextPrefab;
    [SerializeField] private Transform canvasTransform;

    // ▼▼▼▼▼ 싱크 보너스용 변수 추가 ▼▼▼▼▼
    private List<BaseTower> perfectTowersThisBeat = new List<BaseTower>();
    private bool isSubscribed = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // RhythmManager가 준비된 후 이벤트를 구독
    void Start()
    {
        if (RhythmManager.instance != null)
        {
            RhythmManager.OnBeat += HandleBeat;
            isSubscribed = true;
        }
    }

    void OnDestroy()
    {
        if (isSubscribed)
        {
            RhythmManager.OnBeat -= HandleBeat;
        }
    }

    // 매 비트가 끝날 때마다 호출되어 싱크 보너스를 체크
    private void HandleBeat(int beatNumber)
    {
        // 이번 비트에 Perfect를 달성한 타워가 2개 이상이면
        if (perfectTowersThisBeat.Count > 1)
        {
            Debug.Log($"SYNC BONUS! {perfectTowersThisBeat.Count} Towers!");
            ScoreManager.instance.AddSyncBonusScore();
        }

        // 다음 비트를 위해 리스트를 비움
        perfectTowersThisBeat.Clear();
    }

    public void ProcessAttack(BaseTower tower, float attackTime)
    {
        float nearestBeatTime = RhythmManager.instance.GetNearestBeatTime(attackTime);
        float timeError = attackTime - nearestBeatTime;
        Judgment judgment = JudgeByTimeError(Mathf.Abs(timeError));

        // 점수 매니저에 판정 결과를 알려 점수를 추가하도록 함
        ScoreManager.instance.AddRhythmScore(judgment);

        // ▼▼▼▼▼ 싱크 보너스 로직 추가 ▼▼▼▼▼
        if (judgment == Judgment.Perfect)
        {
            perfectTowersThisBeat.Add(tower);
        }

        float rhythmMultiplier = GetDamageMultiplier(judgment);
        int finalDamage = Mathf.RoundToInt(tower.baseDamage * tower.itemDamageMultiplier * rhythmMultiplier);
        tower.Attack(finalDamage);
        ShowJudgmentFeedback(judgment, tower.transform.position);
    }

    private Judgment JudgeByTimeError(float error)
    {
        if (error <= perfectWindow) return Judgment.Perfect;
        if (error <= greatWindow) return Judgment.Great;
        if (error <= goodWindow) return Judgment.Good;
        return Judgment.Miss;
    }

    public float GetDamageMultiplier(Judgment judgment)
    {
        switch (judgment)
        {
            case Judgment.Perfect: return perfectMultiplier;
            case Judgment.Great: return greatMultiplier;
            case Judgment.Good: return goodMultiplier;
            case Judgment.Miss: return missMultiplier;
            default: return 1f;
        }
    }

    public void ShowJudgmentFeedback(Judgment judgment, Vector3 towerPosition)
    {
        if (judgmentTextPrefab == null || canvasTransform == null) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(towerPosition);
        GameObject textGO = Instantiate(judgmentTextPrefab, screenPos, Quaternion.identity, canvasTransform);
        TextMeshProUGUI tmp = textGO.GetComponent<TextMeshProUGUI>();
        tmp.text = judgment.ToString();
        Destroy(textGO, 0.5f);
    }
}