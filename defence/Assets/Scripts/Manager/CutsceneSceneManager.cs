// 파일 이름: CutsceneSceneManager.cs
using UnityEngine;

public class CutsceneSceneManager : MonoBehaviour
{
    void Start()
    {
        // GameSession에서 재생할 컷신 데이터를 가져옵니다.
        CutsceneData dataToPlay = GameSession.instance.cutsceneToPlay;

        if (dataToPlay != null && CutscenePlayer.instance != null)
        {
            // CutscenePlayer에게 컷신 재생을 명령합니다.
            CutscenePlayer.instance.StartCutscene(dataToPlay);
        }
        else
        {
            Debug.LogError("CutsceneSceneManager: 재생할 컷신 데이터가 없거나 CutscenePlayer 인스턴스를 찾을 수 없습니다!");
            // 오류 발생 시 안전하게 다음 씬으로 넘어가도록 처리 (선택사항)
            LoadNextScene();
        }
    }

    // CutscenePlayer가 컷신 종료 시 호출할 함수 (CutscenePlayer.cs 에서 호출)
    public void LoadNextScene()
    {
        // (디버그 로그는 그대로 두셔도 좋습니다)
        Debug.Log($"LoadNextScene 호출됨. isNewGameCutscene = {GameSession.instance.isNewGameCutscene}");

        if (GameSession.instance.isNewGameCutscene)
        {
            // 수정: 닉네임 입력을 이미 마쳤으므로 "StageSelect" 씬으로 이동합니다.
            Debug.Log("첫 플레이 컷신 종료. StageSelect 씬 로드 시도.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("StageSelect");
        }
        else
        {
            // (스테이지 컷신일 경우는 기존과 동일)
            Debug.Log("스테이지 진입 컷신 종료. GameScene 씬 로드 시도.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }
    }
}