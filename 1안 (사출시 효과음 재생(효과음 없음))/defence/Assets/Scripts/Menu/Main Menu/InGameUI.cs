// ���� �̸�: InGameUI.cs (��ü ��ü)
using UnityEngine;
using TMPro;

public class InGameUI : MonoBehaviour
{
    [Header("Player Info UI")]
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI stageText; // �������� �̸� ǥ�ÿ�

    void Start()
    {
        UpdateAllUI();
    }

    public void UpdateAllUI()
    {
        GameData data = SaveLoadManager.instance.gameData;
        nicknameText.text = "Player: " + data.nickname;

        // GameSession���� ���� �������� ������ ������ �̸��� ǥ���մϴ�.
        if (GameSession.instance != null && GameSession.instance.selectedStage != null)
        {
            stageText.text = "Stage: " + GameSession.instance.selectedStage.stageName;
        }
        else
        {
            stageText.text = "Stage: Unknown";
        }
    }

    // ����: ������ �ִ� ������/�ٿ�, �������� �̵�, ������ �߰�, ���� ���� ��ư ��ɵ���
    // ���� �ٸ� ��ũ��Ʈ(GameManager, StageSelectManager ��)���� ó���ϹǷ�
    // InGameUI������ ���ŵǾ����ϴ�. �� ��ũ��Ʈ�� ���� ���� ǥ�� ���Ҹ� �մϴ�.
}