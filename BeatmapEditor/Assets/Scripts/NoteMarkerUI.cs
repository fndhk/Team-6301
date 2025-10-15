//파일 명: NoteMarkerUI.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class NoteMarkerUI : MonoBehaviour, IPointerClickHandler
{
    public SingleNoteData noteData;
    private BeatmapEditorManager editorManager;

    public void Initialize(SingleNoteData data, BeatmapEditorManager manager)
    {
        noteData = data;
        editorManager = manager;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭 또는 좌클릭으로 삭제
        if (eventData.button == PointerEventData.InputButton.Left ||
            eventData.button == PointerEventData.InputButton.Right)
        {
            editorManager.RemoveNote(noteData);
            Destroy(gameObject);
        }
    }
}