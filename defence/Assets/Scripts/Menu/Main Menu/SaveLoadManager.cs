using UnityEngine;
using System; // DateTime�� ����ϱ� ���� �ʿ��մϴ�.
using System.IO; // ������ �ٷ�� ���� �ʿ��մϴ�.
using System.Collections.Generic;

public class SaveLoadManager : MonoBehaviour
{
    // --- �̱��� ���� ���� ---
    // �� ��ũ��Ʈ�� �ν��Ͻ��� ������ static ����
    public static SaveLoadManager instance;

    private void Awake()
    {
        // ���� �̹� SaveLoadManager�� �ִ��� Ȯ��
        if (instance == null)
        {
            // ���ٸ�, �� �ν��Ͻ��� ���
            instance = this;
            // ���� �ٲ� �ı����� �ʵ��� ����
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // �̹� �ִٸ�, ���� ���� �� �ν��Ͻ��� �ı�
            Destroy(gameObject);
        }
    }
    // -------------------------

    // ���� ������ �����͸� ���� ����
    public GameData gameData;
    private string saveFileName = "gameSave.json"; // ����� ������ �⺻ �̸�

    // ���� ���� �� ȣ��Ǵ� �Լ�
    void Start()
    {
        // ������ ���۵� �� �⺻ �����ͷ� �ʱ�ȭ
        this.gameData = new GameData();
    }

    // --- �ֿ� ��� �Լ��� ---

    // ������ ���� ��ȣ�� ���� ���� �����͸� �����ϴ� �Լ�
    public void SaveGame(int slotIndex)
    {
        // 1. ���� ��¥�� �ð��� ���
        gameData.lastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // 2. GameData ��ü�� JSON ���ڿ��� ��ȯ
        // true�� ���ڷ� �ѱ�� ���� ���� �鿩���� �� �������� ��ȯ�˴ϴ�.
        string toJsonData = JsonUtility.ToJson(gameData, true);

        // 3. ���� ��θ� ���ϰ� JSON �����͸� ���Ϸ� ����
        string filePath = Path.Combine(Application.persistentDataPath, slotIndex + "_" + saveFileName);
        File.WriteAllText(filePath, toJsonData);

        Debug.Log($"Save Success: {filePath}");
    }

    // ������ ���� ��ȣ�� ���� �����͸� �ҷ����� �Լ�
    public bool LoadGame(int slotIndex)
    {
        // 1. ���� ��θ� Ȯ��
        string filePath = Path.Combine(Application.persistentDataPath, slotIndex + "_" + saveFileName);

        // 2. �ش� ��ο� ������ �����ϴ��� Ȯ��
        if (File.Exists(filePath))
        {
            // 3. ������ �о�ͼ� JSON �����͸� ���ڿ��� ����
            string fromJsonData = File.ReadAllText(filePath);

            // 4. JSON ���ڿ��� GameData ��ü�� ��ȯ�Ͽ� gameData ������ �����
            gameData = JsonUtility.FromJson<GameData>(fromJsonData);

            Debug.Log($"Load Success: {filePath}");
            return true; // �ҷ����� ����
        }
        else
        {
            Debug.LogWarning($"No Saved Files: {filePath}");
            return false; // �ҷ����� ����
        }
    }

    // (�ҷ����� ȭ���� ���� �߰�) ������ ������ ������ ��� ������ �������� �Լ�
    public GameData LoadSaveSummary(int slotIndex)
    {
        string filePath = Path.Combine(Application.persistentDataPath, slotIndex + "_" + saveFileName);
        if (File.Exists(filePath))
        {
            string fromJsonData = File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameData>(fromJsonData);
        }
        return null; // ������ ������ null ��ȯ
    }
    // ������ ���� �ε����� ���� ���� ���� ��θ� ��ȯ�ϴ� �Լ��Դϴ�.
    private string GetSaveFilePath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, slotIndex + "_" + saveFileName);
    }

    // (�Ʒ� �Լ� �߰�) ������ ���Կ� ���̺� ������ �����ϴ��� Ȯ���ϴ� �Լ��Դϴ�.
    public bool DoesSaveFileExist(int slotIndex)
    {
        string filePath = GetSaveFilePath(slotIndex);
        return File.Exists(filePath);
    }
    public void DeleteSaveFile(int slotIndex)
    {
        string filePath = GetSaveFilePath(slotIndex);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Save file deleted: {filePath}");
        }
    }
}