// ���� �̸�: JudgmentManager.cs
using UnityEngine;
using TMPro;
using System.Collections.Generic; // List�� ����ϱ� ���� �߰�

public class JudgmentManager : MonoBehaviour
{
    public static JudgmentManager instance;

    public enum Judgment { Perfect, Great, Good, Miss }

    [Header("���� ���ʽ� ����")]
    [SerializeField] private float perfectMultiplier = 1.5f;
    [SerializeField] private float greatMultiplier = 1.1f;
    [SerializeField] private float goodMultiplier = 1.0f;
    [SerializeField] private float missMultiplier = 0.8f;

    [Header("���� �ð�â (����: ��)")]
    [SerializeField] private float perfectWindow = 0.05f;
    [SerializeField] private float greatWindow = 0.1f;
    [SerializeField] private float goodWindow = 0.2f;

    [Header("UI �ǵ�� ����")]
    [SerializeField] private GameObject judgmentTextPrefab;
    [SerializeField] private Transform canvasTransform;

    // ������ ��ũ ���ʽ��� ���� �߰� ������
    private List<BaseTower> perfectTowersThisBeat = new List<BaseTower>();
    private bool isSubscribed = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // RhythmManager�� �غ�� �� �̺�Ʈ�� ����
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

    // �� ��Ʈ�� ���� ������ ȣ��Ǿ� ��ũ ���ʽ��� üũ
    private void HandleBeat(int beatNumber)
    {
        // �̹� ��Ʈ�� Perfect�� �޼��� Ÿ���� 2�� �̻��̸�
        if (perfectTowersThisBeat.Count > 1)
        {
            Debug.Log($"SYNC BONUS! {perfectTowersThisBeat.Count} Towers!");
            ScoreManager.instance.AddSyncBonusScore();
        }

        // ���� ��Ʈ�� ���� ����Ʈ�� ���
        perfectTowersThisBeat.Clear();
    }

    public void ProcessAttack(BaseTower tower, float attackTime)
    {
        float nearestBeatTime = RhythmManager.instance.GetNearestBeatTime(attackTime);
        float timeError = attackTime - nearestBeatTime;
        Judgment judgment = JudgeByTimeError(Mathf.Abs(timeError));

        // ���� �Ŵ����� ���� ����� �˷� ������ �߰��ϵ��� ��
        ScoreManager.instance.AddRhythmScore(judgment);

        // ������ ��ũ ���ʽ� ���� �߰� ������
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