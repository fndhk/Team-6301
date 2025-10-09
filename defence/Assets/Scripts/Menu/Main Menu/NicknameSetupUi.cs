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
    private void OnConfirmButtonClick()
    {
        string nickname = nicknameInputField.text;

        // �г����� ����ְų� ���鸸 �ִ��� Ȯ��
        if (string.IsNullOrWhiteSpace(nickname))
        {
            Debug.LogWarning("Insert your name");
            // ���⿡ ����ڿ��� �˸��� �ִ� UI�� �߰��� ���� �ֽ��ϴ�.
            return; // �Լ� ����
        }

        // SaveLoadManager�� ���� ���� ���� �����Ϳ� �г��� ����
        SaveLoadManager.instance.gameData.nickname = nickname;

        // �����Ͱ� �غ�Ǿ����� InGame ������ �̵�
        //SceneManager.LoadScene("InGame");
        SceneManager.LoadScene("StageSelect");
    }
}