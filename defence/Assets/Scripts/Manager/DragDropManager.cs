// 파일 이름: DragDropManager.cs
using UnityEngine;
using UnityEngine.UI;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager instance;

    public Image dragIcon; // Unity 에디터에서 DragIcon을 연결

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // 아이콘을 보이게 하고, 마우스 위치로 옮기는 함수
    public void StartDrag(Sprite iconSprite)
    {
        dragIcon.sprite = iconSprite;
        dragIcon.gameObject.SetActive(true);
    }

    // 아이콘을 숨기는 함수
    public void EndDrag()
    {
        dragIcon.gameObject.SetActive(false);
    }

    // 아이콘이 마우스를 따라다니게 하는 함수
    public void OnDrag()
    {
        dragIcon.transform.position = Input.mousePosition;
    }
}