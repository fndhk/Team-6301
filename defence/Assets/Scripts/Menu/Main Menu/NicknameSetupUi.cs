using UnityEngine;
using UnityEngine.UI; // Unity UI ��Ҹ� ����ϱ� ���� �ʿ��մϴ�.
using TMPro; // TextMeshPro�� ����ϱ� ���� �ʿ��մϴ�.
using UnityEngine.SceneManagement; // �� ������ ���� �ʿ��մϴ�.

public class NicknameSetupUI : MonoBehaviour
{
    // Unity �����Ϳ��� ������ UI ��ҵ�
    public TMP_InputField nicknameInputField; // �г��� �Է�â
    public Button confirmButton; // Ȯ�� ��ư

    void Start()
    {
        // ��ư�� Ŭ�� �̺�Ʈ ������ �߰�
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
    }

    // Ȯ�� ��ư�� Ŭ���Ǿ��� �� ȣ��� �Լ�
    //���� ��: NicknameSetupUI.cs (OnConfirmButtonClick �Լ� ����)

    private void OnConfirmButtonClick()
    {
        string nickname = nicknameInputField.text;

        if (string.IsNullOrWhiteSpace(nickname))
        {
            Debug.LogWarning("Insert your name");
            return;
        }

        SaveLoadManager.instance.gameData.nickname = nickname;

        // ------ �ű� �߰�: �⺻ ĳ���� ���� ------
        if (string.IsNullOrEmpty(SaveLoadManager.instance.gameData.currentSelectedCharacterID))
        {
            SaveLoadManager.instance.gameData.currentSelectedCharacterID = "Char_Boom";

            // characterLevels���� �߰�
            if (!SaveLoadManager.instance.gameData.characterLevels.ContainsKey("Char_Boom"))
            {
                SaveLoadManager.instance.gameData.characterLevels.Add("Char_Boom", 1);
            }

            Debug.Log("NicknameSetupUI: �⺻ ĳ����(Char_Boom)�� ���� �Ϸ�");
        }

        SaveLoadManager.instance.SaveGame(GameSession.instance.currentSaveSlot);
        SceneManager.LoadScene("StageSelect");
    }
}