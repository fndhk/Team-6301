// ���� �̸�: CharacterSelectUI.cs (�� ����)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class CharacterSelectUI : MonoBehaviour
{
    [Tooltip("�ν����Ϳ��� 3���� ĳ���� ������ ������ ����")]
    public List<CharacterData> characterDatas;

    [Header("UI ����")]
    public Button[] characterButtons;
    public TextMeshProUGUI descriptionText;
    public Button confirmButton;

    private int selectedIndex = -1;

    void Start()
    {
        // ��ư�� ��� ����
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i; // �߿�: Ŭ���� ���� ����
            characterButtons[i].onClick.AddListener(() => OnClickCharacterButton(index));

            // ��ư �̹��� ����
            if (i < characterDatas.Count && characterDatas[i].characterIcon != null)
            {
                characterButtons[i].image.sprite = characterDatas[i].characterIcon;
            }
        }
        confirmButton.onClick.AddListener(StartGame);

        // ó������ �ƹ��͵� ���õ��� ���� ���·� ����
        OnClickCharacterButton(0); // �Ǵ� ù��° ĳ���͸� �⺻ ����
    }

    private void OnClickCharacterButton(int index)
    {
        selectedIndex = index;
        descriptionText.text = $"{characterDatas[index].characterName}\n\n{characterDatas[index].characterDescription}";

        // (���û���) ���õ� ��ư�� �ð������� �����ϴ� ȿ�� �߰�
    }

    private void StartGame()
    {
        if (selectedIndex != -1)
        {
            // GameSession�� ���õ� ĳ���� ������ ����
            GameSession.instance.selectedCharacter = characterDatas[selectedIndex];
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.LogWarning("ĳ���͸� �������ּ���!");
        }
    }
}