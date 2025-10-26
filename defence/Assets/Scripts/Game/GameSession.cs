using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession instance;

    public StageData selectedStage; // 선택된 스테이지 정보를 저장할 변수
    public int currentSaveSlot;
    public CharacterData selectedCharacter;
    [HideInInspector] public CutsceneData cutsceneToPlay; // 컷신 씬에서 재생할 컷신 데이터
    [HideInInspector] public bool isNewGameCutscene = false; // 첫 플레이 컷신인지 여부

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