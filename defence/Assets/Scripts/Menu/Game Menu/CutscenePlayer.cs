// 파일 이름: CutscenePlayer.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CutscenePlayer : MonoBehaviour
{
    public static CutscenePlayer instance; // 다른 스크립트에서 쉽게 접근 (싱글톤)

    [Header("UI 연결")]
    public GameObject cutscenePanel;      // 컷신 전체를 덮는 부모 패널
    public Image backgroundImage;         // 배경 이미지
    public Image characterIllustration; // 캐릭터 일러스트가 표시될 Image
    public TextMeshProUGUI nameText;          // 캐릭터 이름 텍스트
    public TextMeshProUGUI dialogueText;      // 대사 텍스트
    public Button skipButton;             // 스킵 버튼

    private CutsceneData currentCutscene;
    private int currentLineIndex;
    private bool isPlaying = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 스킵 버튼에 기능 연결
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipCutscene);
        }
        // 시작할 땐 컷신을 숨겨둡니다.
        //cutscenePanel.SetActive(false);
    }

    void Update()
    {
        // 컷신이 재생 중이 아닐 때는 입력을 받지 않습니다.
        if (!isPlaying) return;

        // "아무 키나 누르거나" "마우스 클릭을 하면" 다음 대사로 넘어갑니다.
        // Input.anyKeyDown은 마우스 클릭도 포함합니다.
        if (Input.anyKeyDown)
        {
            AdvanceDialogue();
        }
    }

    // 다른 스크립트에서 이 함수를 호출하여 컷신을 시작합니다.
    public void StartCutscene(CutsceneData cutsceneToPlay)
    {
        if (cutsceneToPlay == null || cutsceneToPlay.dialogueLines.Count == 0)
        {
            Debug.LogWarning("재생할 컷신 데이터가 없습니다.");
            return;
        }

        currentCutscene = cutsceneToPlay;
        currentLineIndex = 0;
        isPlaying = true;
        cutscenePanel.SetActive(true);

        DisplayLine(currentLineIndex);
    }

    // 다음 대사로 진행
    private void AdvanceDialogue()
    {
        currentLineIndex++;

        if (currentLineIndex < currentCutscene.dialogueLines.Count)
        {
            // 다음 대사가 있으면 표시
            DisplayLine(currentLineIndex);
        }
        else
        {
            Debug.Log("<color=yellow>AdvanceDialogue: 마지막 대사 도달! EndCutscene 호출 시도.</color>");
            // 대사가 끝났으면 컷신 종료
            EndCutscene();
        }
    }

    // 현재 인덱스의 대사를 UI에 표시
    private void DisplayLine(int index)
    {
        DialogueLine line = currentCutscene.dialogueLines[index];

        // 1. 캐릭터 정보 표시 (기존과 동일)
        if (line.character != null)
        {
            characterIllustration.gameObject.SetActive(true);
            nameText.gameObject.SetActive(true);
            characterIllustration.sprite = line.character.characterIllustration;
            nameText.text = line.character.characterName;
        }
        else
        {
            characterIllustration.gameObject.SetActive(false);
            nameText.gameObject.SetActive(false);
        }

        // 2. 배경 이미지 표시 (기존과 동일)
        if (line.background != null)
        {
            backgroundImage.sprite = line.background;
        }

        // 3. --- ( 수정된 부분: 대사 텍스트 닉네임 치환) ---
        string rawText = line.dialogueText; // 원본 대사
        string processedText = rawText;     // 최종 표시될 대사

        // 대사에 [PLAYER_NAME]이라는 특수 문자열이 포함되어 있는지 확인
        if (rawText.Contains("[PLAYER_NAME]"))
        {
            // SaveLoadManager에서 현재 저장된 닉네임을 가져옵니다.
            string nickname = SaveLoadManager.instance.gameData.nickname;

            // 닉네임이 비어있지 않다면
            if (!string.IsNullOrEmpty(nickname))
            {
                // [PLAYER_NAME]을 실제 닉네임으로 교체합니다.
                processedText = rawText.Replace("[PLAYER_NAME]", nickname);
            }
            else
            {
                // 닉네임이 비어있다면 "용사" 같은 기본 이름으로 교체 (선택사항)
                processedText = rawText.Replace("[PLAYER_NAME]", "용사");
            }
        }

        // 최종 처리된 대사를 UI에 표시합니다.
        dialogueText.text = processedText;
        // --- ( 수정 끝) ---
    }

    // 스킵 버튼 클릭 시 호출
    public void SkipCutscene()
    {
        EndCutscene();
    }

    // 컷신 종료 처리
    // 파일 이름: CutscenePlayer.cs
    // EndCutscene 함수를 아래 내용으로 교체

    private void EndCutscene()
    {
        Debug.Log("<color=green>EndCutscene 함수 시작!</color>");
        isPlaying = false;
        cutscenePanel.SetActive(false);

        //  스테이지 진입 컷신이었는지 확인
        if (currentCutscene != null && !GameSession.instance.isNewGameCutscene && GameSession.instance.selectedStage != null)
        {
            int stageIndex = GameSession.instance.selectedStage.stageIndex;
            GameData gameData = SaveLoadManager.instance.gameData;

            //  아직 기록되지 않았다면 시청 기록 추가 및 저장
            if (!gameData.watchedCutsceneStageIndices.Contains(stageIndex))
            {
                gameData.watchedCutsceneStageIndices.Add(stageIndex);
                SaveLoadManager.instance.SaveGame(GameSession.instance.currentSaveSlot);
                Debug.Log($"<color=green>컷신 시청 기록 저장:</color> Stage {stageIndex}");
            }
        }

        currentCutscene = null;

        //  CutsceneSceneManager에게 다음 씬 로드를 요청
        CutsceneSceneManager sceneManager = FindFirstObjectByType<CutsceneSceneManager>();
        if (sceneManager != null)
        {
            sceneManager.LoadNextScene();
        }
        else
        {
            Debug.LogError("CutscenePlayer: CutsceneSceneManager를 찾을 수 없어 다음 씬으로 이동할 수 없습니다!");
        }
    }
}