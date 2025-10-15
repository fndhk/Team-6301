// ���� �̸�: SkillCutsceneUI.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillCutsceneUI : MonoBehaviour
{
    public static SkillCutsceneUI instance;

    [Header("UI ����")]
    [Tooltip("�ƽ� �̹����� ǥ�õ� �г�")]
    public GameObject cutscenePanel;
    [Tooltip("�ƽ� �̹��� ������Ʈ")]
    public Image cutsceneImage;

    [Header("�ִϸ��̼� ����")]
    [Tooltip("�ƽ� ǥ�� �ð� (��)")]
    public float displayDuration = 1.0f;
    [Tooltip("���̵��� �ð� (��)")]
    public float fadeInDuration = 0.2f;
    [Tooltip("���̵�ƿ� �ð� (��)")]
    public float fadeOutDuration = 0.3f;
    [Tooltip("�ƽ� �̹��� ������ �ִϸ��̼� ���")]
    public bool useScaleAnimation = true;
    [Tooltip("���� ������ ����")]
    public float startScale = 1.2f;
    [Tooltip("�� ������ ����")]
    public float endScale = 1.0f;

    private CanvasGroup canvasGroup;
    private bool isPlaying = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // ------ ����: Panel���� CanvasGroup �������� ------
        if (cutscenePanel != null)
        {
            canvasGroup = cutscenePanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = cutscenePanel.AddComponent<CanvasGroup>();
            }

            // ------ ����: ���� �� �г� ���� (��Ȱ��ȭ ��� �����ϰ�) ------
            canvasGroup.alpha = 0f;
            cutscenePanel.SetActive(true); // �׻� Ȱ��ȭ ���·� ����
        }
    }

    // ------ ��ų �ƽ� ��� �Լ� ------
    public void PlayCutscene(Sprite cutsceneSprite)
    {
        if (cutsceneSprite == null)
        {
            Debug.LogWarning("SkillCutsceneUI: �ƽ� ��������Ʈ�� null�Դϴ�!");
            return;
        }

        if (isPlaying)
        {
            Debug.Log("SkillCutsceneUI: �̹� �ƽ��� ��� ���Դϴ�.");
            return;
        }

        // ------ ����: �� ��ũ��Ʈ�� �׻� Ȱ��ȭ�� ������Ʈ�� �����Ƿ� �ڷ�ƾ ���� ���� ------
        StartCoroutine(PlayCutsceneCoroutine(cutsceneSprite));
    }

    // ------ �ƽ� ��� �ڷ�ƾ ------
    private IEnumerator PlayCutsceneCoroutine(Sprite cutsceneSprite)
    {
        isPlaying = true;

        // �̹��� ����
        if (cutsceneImage != null)
        {
            cutsceneImage.sprite = cutsceneSprite;
            cutsceneImage.gameObject.SetActive(true); // �̹��� Ȱ��ȭ
        }

        // �ʱ� ���� ����
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (useScaleAnimation && cutsceneImage != null)
        {
            cutsceneImage.transform.localScale = Vector3.one * startScale;
        }

        // ���̵��� + ������ �ִϸ��̼�
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeInDuration;

            // ���̵���
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            }

            // ������ �ִϸ��̼�
            if (useScaleAnimation && cutsceneImage != null)
            {
                float currentScale = Mathf.Lerp(startScale, endScale, t);
                cutsceneImage.transform.localScale = Vector3.one * currentScale;
            }

            yield return null;
        }

        // ������ ǥ��
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        if (useScaleAnimation && cutsceneImage != null)
        {
            cutsceneImage.transform.localScale = Vector3.one * endScale;
        }

        // ������ �ð���ŭ ǥ��
        yield return new WaitForSecondsRealtime(displayDuration);

        // ���̵�ƿ�
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeOutDuration;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            }

            yield return null;
        }

        // ������ ����
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (cutsceneImage != null)
        {
            cutsceneImage.gameObject.SetActive(false); // �̹��� ��Ȱ��ȭ
        }

        isPlaying = false;
        Debug.Log("SkillCutsceneUI: �ƽ� ��� �Ϸ�!");
    }

    // ------ ��� �ƽ� �ߴ� (�ʿ��) ------
    public void StopCutscene()
    {
        StopAllCoroutines();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (cutsceneImage != null)
        {
            cutsceneImage.gameObject.SetActive(false);
        }

        isPlaying = false;
    }
}