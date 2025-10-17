//���� ��: NoteMarkerUI.cs
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
        // ��Ŭ�� �Ǵ� ��Ŭ������ ����
        if (eventData.button == PointerEventData.InputButton.Left ||
            eventData.button == PointerEventData.InputButton.Right)
        {
            editorManager.RemoveNote(noteData);
            Destroy(gameObject);
        }
    }
}