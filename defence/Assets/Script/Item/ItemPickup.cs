using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Sprite itemIcon;

    // OnMouseDown�� Collider�� �ִ� ������Ʈ ������ ���콺 Ŭ���� ���۵� �� ȣ��˴ϴ�.
    private void OnMouseDown()
    {
        bool wasAdded = Inventory.instance.AddItem(itemIcon);

        if (wasAdded)
        {
            Destroy(gameObject);
        }

    }
}