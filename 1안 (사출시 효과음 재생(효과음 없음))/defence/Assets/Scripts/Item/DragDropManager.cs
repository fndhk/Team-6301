// ���� �̸�: DragDropManager.cs
using UnityEngine;
using UnityEngine.UI;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager instance;

    public Image dragIcon; // Unity �����Ϳ��� DragIcon�� ����

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // �������� ���̰� �ϰ�, ���콺 ��ġ�� �ű�� �Լ�
    public void StartDrag(Sprite iconSprite)
    {
        dragIcon.sprite = iconSprite;
        dragIcon.gameObject.SetActive(true);
    }

    // �������� ����� �Լ�
    public void EndDrag()
    {
        dragIcon.gameObject.SetActive(false);
    }

    // �������� ���콺�� ����ٴϰ� �ϴ� �Լ�
    public void OnDrag()
    {
        dragIcon.transform.position = Input.mousePosition;
    }
}