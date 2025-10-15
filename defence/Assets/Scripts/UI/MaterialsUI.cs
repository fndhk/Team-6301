// ���� �̸�: MaterialsUI.cs
using UnityEngine;
using TMPro;
using System.Collections;

public class MaterialsUI : MonoBehaviour
{
    public static MaterialsUI instance;

    [Header("UI ����")]
    [Tooltip("��ȭ ���ڸ� ǥ���� �ؽ�Ʈ")]
    public TextMeshProUGUI materialsText;

    [Header("�ִϸ��̼� ����")]
    [Tooltip("��ȭ�� ������ �� �ؽ�Ʈ�� Ŀ���� ȿ��")]
    public bool useScaleAnimation = true;
    [Tooltip("��ȭ�� ������ �� ���� ���� ȿ��")]
    public bool useColorAnimation = true;
    [Tooltip("���� �� ǥ���� ����")]
    public Color increaseColor = Color.yellow;

    private int currentDisplayedMaterials = 0;
    private Color originalColor;
    private Vector3 originalScale;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (materialsText != null)
        {
            originalColor = materialsText.color;
            originalScale = materialsText.transform.localScale;
        }

        // ���� ���� �� ���� ��ȭ�� ǥ��
        UpdateMaterialsDisplay();
    }

    // ------ ��ȭ ǥ�� ������Ʈ (�ǽð�) ------
    public void UpdateMaterialsDisplay()
    {
        if (SaveLoadManager.instance == null || SaveLoadManager.instance.gameData == null)
        {
            Debug.LogWarning("MaterialsUI: SaveLoadManager �Ǵ� gameData�� �����ϴ�!");
            return;
        }

        int actualMaterials = SaveLoadManager.instance.gameData.enhancementMaterials;

        // ���� ǥ�õ� ���ڿ� ���� ��ȭ���� �ٸ��� �ִϸ��̼�
        if (actualMaterials != currentDisplayedMaterials)
        {
            int difference = actualMaterials - currentDisplayedMaterials;

            if (difference > 0)
            {
                // ��ȭ ����
                StartCoroutine(AnimateIncrease(actualMaterials));
            }
            else
            {
                // ��ȭ ���� (��� �ݿ�)
                currentDisplayedMaterials = actualMaterials;
                UpdateText();
            }
        }
        else
        {
            // ��ȭ ������ �׳� ������Ʈ
            UpdateText();
        }
    }

    // ------ ��ȭ ���� �ִϸ��̼� ------
    private IEnumerator AnimateIncrease(int targetAmount)
    {
        int startAmount = currentDisplayedMaterials;
        int difference = targetAmount - startAmount;
        float duration = Mathf.Min(0.5f, difference * 0.05f); // �ִ� 0.5��
        float elapsed = 0f;

        // ������ �ִϸ��̼� ����
        if (useScaleAnimation)
        {
            StartCoroutine(ScaleAnimation(1.2f, 0.2f));
        }

        // ���� ����
        if (useColorAnimation)
        {
            materialsText.color = increaseColor;
        }

        // ���� ī��Ʈ��
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Time.timeScale ���� �� ����
            float t = elapsed / duration;

            currentDisplayedMaterials = Mathf.RoundToInt(Mathf.Lerp(startAmount, targetAmount, t));
            UpdateText();

            yield return null;
        }

        currentDisplayedMaterials = targetAmount;
        UpdateText();

        // �ణ�� ���
        yield return new WaitForSecondsRealtime(0.1f);

        // ���� ����
        if (useColorAnimation)
        {
            StartCoroutine(ColorAnimation(originalColor, 0.3f));
        }
    }

    // ------ ������ �ִϸ��̼� �ڷ�ƾ ------
    private IEnumerator ScaleAnimation(float targetScale, float duration)
    {
        Vector3 startScale = materialsText.transform.localScale;
        Vector3 endScale = originalScale * targetScale;
        float elapsed = 0f;

        // Ŀ����
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            materialsText.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        materialsText.transform.localScale = endScale;

        // ��� ���
        yield return new WaitForSecondsRealtime(0.1f);

        // �������
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            materialsText.transform.localScale = Vector3.Lerp(endScale, originalScale, t);
            yield return null;
        }

        materialsText.transform.localScale = originalScale;
    }

    // ------ ���� �ִϸ��̼� �ڷ�ƾ ------
    private IEnumerator ColorAnimation(Color targetColor, float duration)
    {
        Color startColor = materialsText.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            materialsText.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        materialsText.color = targetColor;
    }

    // ------ �ؽ�Ʈ ������Ʈ (õ���� �޸�) ------
    private void UpdateText()
    {
        if (materialsText != null)
        {
            materialsText.text = currentDisplayedMaterials.ToString("N0"); // 1,234 ����
        }
    }

    // ------ �ܺο��� ��ȭ ���� �� ȣ���� �Լ� ------
    public void OnMaterialsChanged()
    {
        UpdateMaterialsDisplay();
    }
}