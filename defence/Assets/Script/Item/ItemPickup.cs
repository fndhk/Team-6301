using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Sprite itemIcon;

    // OnMouseDown은 Collider가 있는 오브젝트 위에서 마우스 클릭이 시작될 때 호출됩니다.
    private void OnMouseDown()
    {
        bool wasAdded = Inventory.instance.AddItem(itemIcon);

        if (wasAdded)
        {
            Destroy(gameObject);
        }

    }
}