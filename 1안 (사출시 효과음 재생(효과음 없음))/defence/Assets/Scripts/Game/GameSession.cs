using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession instance;

    public StageData selectedStage; // 선택된 스테이지 정보를 저장할 변수
    public int currentSaveSlot;

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