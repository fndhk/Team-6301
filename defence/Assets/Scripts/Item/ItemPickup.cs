using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemPickup : MonoBehaviour
{
    public ItemData itemData; // �� �������� '���赵'
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        // �ڱ� �ڽ��� SpriteRenderer ������Ʈ�� �̸� ã�Ƴ���
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // �������� ������ �� ȣ��Ǿ� � ���������� �����ϴ� �Լ�
    public void Initialize(ItemData data)
    {
        itemData = data;
        // SpriteRenderer�� ItemData�� �ִ� �������� ǥ��
        spriteRenderer.sprite = itemData.icon;
    }

    private void OnMouseDown()
    {
        if (itemData == null) return;

        bool wasAdded = Inventory.instance.AddItem(itemData);
        if (wasAdded)
        {
            Destroy(gameObject);
        }
    }
}