using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class QuickSlot : MonoBehaviour
{
    [Header("UI 연결")]
    public Image iconImage;
    public TextMeshProUGUI quantityText; // 수량을 표시할 텍스트

    [Header("슬롯 설정")]
    public ItemData designatedItem; // ★★★ 이 슬롯에 지정된 아이템 (Inspector에서 연결) ★★★

    private int currentQuantity = 0;
    private const int MAX_QUANTITY = 9;

    void Start()
    {
        // 게임 시작 시, 지정된 아이템이 있다면 아이콘을 표시하고 수량은 0으로 초기화
        if (designatedItem != null)
        {
            iconImage.sprite = designatedItem.icon;
            iconImage.gameObject.SetActive(true);
            UpdateQuantityText();
        }
        else
        {
            // 지정된 아이템이 없으면 슬롯을 비워둠
            iconImage.gameObject.SetActive(false);
            quantityText.gameObject.SetActive(false);
        }
    }

    // 이 슬롯에 아이템을 1개 추가하는 함수
    public void AddItem()
    {
        if (currentQuantity < MAX_QUANTITY)
        {
            currentQuantity++;
            UpdateQuantityText();
        }
    }

    // 수량 텍스트를 업데이트하는 함수
    private void UpdateQuantityText()
    {
        // 수량이 0일 때는 숫자를 숨김
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

    // (나중에 아이템 사용 시 필요) 아이템을 1개 사용하는 함수
    public void UseItem()
    {
        if (currentQuantity > 0)
        {
            currentQuantity--;
            UpdateQuantityText();
            Debug.Log(designatedItem.itemName + " 아이템을 사용했습니다. 남은 수량: " + currentQuantity);
        }
    }
}