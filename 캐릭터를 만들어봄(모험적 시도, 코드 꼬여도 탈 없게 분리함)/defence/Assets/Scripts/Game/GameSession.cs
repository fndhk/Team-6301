using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession instance;

    public StageData selectedStage; // ���õ� �������� ������ ������ ����
    public int currentSaveSlot;
    public CharacterData selectedCharacter;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}