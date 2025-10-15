using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public void UseItem()
    {
        // 1. 사용할 아이템의 수량이 0보다 큰지, 그리고 효과가 지정되어 있는지 확인
        if (currentQuantity > 0 && designatedItem != null && designatedItem.itemEffect != null)
        {
            // 2. 아이템 데이터에 연결된 '효과'에게 직접 실행을 명령
            designatedItem.itemEffect.ExecuteEffect();
            Debug.Log(designatedItem.itemName + " 아이템을 사용했습니다.");

            // 3. 수량을 1 감소시키고 UI를 업데이트
            currentQuantity--;
            UpdateQuantityText();
        }
        else
        {
            Debug.Log("사용할 아이템이 없거나, 아이템에 효과가 지정되지 않았습니다.");
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
    public void ClearSlot()
    {
        currentQuantity = 0; // 수량을 0으로 초기화
        UpdateQuantityText(); // 수량 텍스트 업데이트 (숨김 처리)
    }
}