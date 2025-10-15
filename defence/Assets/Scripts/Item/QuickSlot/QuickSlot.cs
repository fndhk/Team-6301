using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public void UseItem()
    {
        // 1. ����� �������� ������ 0���� ū��, �׸��� ȿ���� �����Ǿ� �ִ��� Ȯ��
        if (currentQuantity > 0 && designatedItem != null && designatedItem.itemEffect != null)
        {
            // 2. ������ �����Ϳ� ����� 'ȿ��'���� ���� ������ ���
            designatedItem.itemEffect.ExecuteEffect();
            Debug.Log(designatedItem.itemName + " �������� ����߽��ϴ�.");

            // 3. ������ 1 ���ҽ�Ű�� UI�� ������Ʈ
            currentQuantity--;
            UpdateQuantityText();
        }
        else
        {
            Debug.Log("����� �������� ���ų�, �����ۿ� ȿ���� �������� �ʾҽ��ϴ�.");
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
    public void ClearSlot()
    {
        currentQuantity = 0; // ������ 0���� �ʱ�ȭ
        UpdateQuantityText(); // ���� �ؽ�Ʈ ������Ʈ (���� ó��)
    }
}