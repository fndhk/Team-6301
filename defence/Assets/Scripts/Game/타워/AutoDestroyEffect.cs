//파일 명: AutoDestroyEffect.cs
using UnityEngine;

// 생성된 이펙트를 일정 시간 후 자동으로 삭제하는 스크립트
public class AutoDestroyEffect : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("이펙트가 사라질 때까지의 시간 (초)")]
    public float lifetime = 0.5f;

    [Tooltip("페이드 아웃 효과 사용 여부")]
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

        // 페이드 아웃 효과
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

        // 시간이 다 되면 삭제
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}