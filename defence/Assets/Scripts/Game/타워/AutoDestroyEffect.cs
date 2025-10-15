//���� ��: AutoDestroyEffect.cs
using UnityEngine;

// ������ ����Ʈ�� ���� �ð� �� �ڵ����� �����ϴ� ��ũ��Ʈ
public class AutoDestroyEffect : MonoBehaviour
{
    [Header("����")]
    [Tooltip("����Ʈ�� ����� �������� �ð� (��)")]
    public float lifetime = 0.5f;

    [Tooltip("���̵� �ƿ� ȿ�� ��� ����")]
    public bool useFadeOut = true;

    private SpriteRenderer spriteRenderer;
    private float timer = 0f;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // ���̵� �ƿ� ȿ��
        if (useFadeOut && spriteRenderer != null)
        {
            float alpha = 1f - (timer / lifetime);
            spriteRenderer.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                alpha * originalColor.a
            );
        }

        // �ð��� �� �Ǹ� ����
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}