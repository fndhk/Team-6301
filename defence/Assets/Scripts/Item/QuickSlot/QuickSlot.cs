using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro�� ����ϱ� ���� �߰�

public class QuickSlot : MonoBehaviour
{
    [Header("UI ����")]
    public Image iconImage;
    public TextMeshProUGUI quantityText; // ������ ǥ���� �ؽ�Ʈ

    [Header("���� ����")]
    public ItemData designatedItem; // �ڡڡ� �� ���Կ� ������ ������ (Inspector���� ����) �ڡڡ�

    private int currentQuantity = 0;
    private const int MAX_QUANTITY = 9;

    void Start()
    {
        // ���� ���� ��, ������ �������� �ִٸ� �������� ǥ���ϰ� ������ 0���� �ʱ�ȭ
        if (designatedItem != null)
        {
            iconImage.sprite = designatedItem.icon;
            iconImage.gameObject.SetActive(true);
            UpdateQuantityText();
        }
        else
        {
            // ������ �������� ������ ������ �����
            iconImage.gameObject.SetActive(false);
            quantityText.gameObject.SetActive(false);
        }
    }

    // �� ���Կ� �������� 1�� �߰��ϴ� �Լ�
    public void AddItem()
    {
        if (currentQuantity < MAX_QUANTITY)
        {
            currentQuantity++;
            UpdateQuantityText();
        }
    }

    // ���� �ؽ�Ʈ�� ������Ʈ�ϴ� �Լ�
    private void UpdateQuantityText()
    {
        // ������ 0�� ���� ���ڸ� ����
        if (currentQuantity > 0)
        {
            quantityText.text = currentQuantity.ToString();
            quantityText.gameObject.SetActive(true);
        }
        else
        {
            quantityText.gameObject.SetActive(false);
        }
    }

    // (���߿� ������ ��� �� �ʿ�) �������� 1�� ����ϴ� �Լ�
    public void UseItem()
    {
        if (currentQuantity > 0)
        {
            currentQuantity--;
            UpdateQuantityText();
            Debug.Log(designatedItem.itemName + " �������� ����߽��ϴ�. ���� ����: " + currentQuantity);
        }
    }
}