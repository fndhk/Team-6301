using System.Collections.Generic; // List�� ����ϱ� ���� �ʿ��մϴ�.

// [System.Serializable] ��Ʈ����Ʈ�� �� Ŭ������ ������
// ���Ϸ� �����ϰų� �ٸ� �������� ��ȯ(����ȭ)�� �� �ְ� ������ݴϴ�.
[System.Serializable]
public class GameData
{
    // ������ ������ �׸��
    public string nickname;
    public int playerLevel;
    public int currentStage;
    public List<string> items;
    public string lastSaveTime;

    // ������ ó�� ������ �� ����� �⺻�� ���� (������)
    public GameData()
    {
        this.nickname = "";
        this.playerLevel = 1;
        this.currentStage = 1;
        this.items = new List<string>();
        this.lastSaveTime = "";
    }
}