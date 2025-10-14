// ���� �̸�: TowerSlot.cs (���� ����)
using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    [Header("������ Ÿ��")]
    // �� ������ �����ϴ� Ÿ�� ������Ʈ (Inspector���� ����)
    public BaseTower towerInSlot;

    // Ÿ���� Sprite Renderer�� �̸� ã�Ƶ� (���� �����)
    private SpriteRenderer towerSpriteRenderer;

    void Start()
    {
        // Ÿ���� �ڽ� ������Ʈ���� Sprite Renderer�� ã�ƿ�
        if (towerInSlot != null)
        {
            towerSpriteRenderer = towerInSlot.GetComponentInChildren<SpriteRenderer>();
        }

        // ���� ���� �� ��Ȱ��ȭ ���¶�� ������ ȸ������ ����
        if (towerInSlot != null && !towerInSlot.gameObject.activeSelf && towerSpriteRenderer != null)
        {
            towerSpriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 0.8f); // ��ο� ȸ��
        }
    }

    // �ܺ�(������ �ý���)���� ȣ���� Ȱ��ȭ �Լ�
    public void ActivateTower()
    {
        if (towerInSlot != null && !towerInSlot.gameObject.activeSelf)
        {
            towerInSlot.gameObject.SetActive(true);
            if (towerSpriteRenderer != null)
            {
                towerSpriteRenderer.color = Color.white; // ���� �������� ����
            }
            Debug.Log(towerInSlot.name + " Ȱ��ȭ!");
        }
    }

    // InventorySlot���� �������� ���������� ������� �� ȣ��� �Լ�
    public void OnItemDropped(ItemData droppedItem)
    {
        // 1. �� ������ Ÿ���� ��Ȱ��ȭ ���¶�� (���� ���� üũ)
        if (!towerInSlot.gameObject.activeSelf)
        {
            // ������ ������ ������� ������ Ȱ��ȭ
            ActivateTower();
        }
        // 2. Ÿ���� �̹� Ȱ��ȭ �����̰�, ��ӵ� �����ۿ� 'ȿ��'�� ����Ǿ� �ִٸ�
        else if (droppedItem.itemEffect != null)
        {
            Debug.Log($"{towerInSlot.name}�� {droppedItem.itemName} ���� ����!");

            // �����ۿ��� ���� ȿ�� ������ ��û
            droppedItem.itemEffect.ExecuteEffect();
        }
    }
}