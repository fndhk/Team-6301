// 파일 이름: CutsceneData.cs
using UnityEngine;
using System.Collections.Generic;

// [System.Serializable]은 이 클래스를 Inspector 창에 표시할 수 있게 해줍니다.
// 이것이 '레이어' 한 줄의 데이터 구조입니다.
[System.Serializable]
public class DialogueLine
{
    [Tooltip("대사하는 캐릭터. 비워두면 나레이션으로 처리됩니다.")]
    public CharacterData character; // 이 에셋에서 이름과 일러스트를 가져옵니다.

    [Tooltip("표시할 배경 이미지. 비워두면 이전 배경을 그대로 사용합니다.")]
    public Sprite background;

    [Tooltip("표시할 대사 텍스트")]
    [TextArea(3, 10)]
    public string dialogueText;
}

// 이 스크립트가 컷신 에셋(대본)의 본체입니다.
[CreateAssetMenu(fileName = "NewCutscene", menuName = "TowerDefense/Cutscene Data")]
public class CutsceneData : ScriptableObject
{
    [Tooltip("이 컷신에서 재생될 대사 '레이어' 목록")]
    public List<DialogueLine> dialogueLines;
}