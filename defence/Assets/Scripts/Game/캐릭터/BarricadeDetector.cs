// 파일 이름: BarricadeDetector.cs (상세 디버그 버전)
using UnityEngine;

public class BarricadeDetector : MonoBehaviour
{
    private Enemy mainScript;
    private Collider2D myCollider;

    void Start()
    {
        mainScript = GetComponentInParent<Enemy>();
        myCollider = GetComponent<Collider2D>();

        // --- 1. 필수 컴포넌트 확인 ---
        

        // --- 2. 레이어 확인 ---
        string myLayer = LayerMask.LayerToName(gameObject.layer);
        Debug.Log($"[Detector] 내 레이어는: <color=cyan>{myLayer}</color> 입니다.");

        // --- 3. 프로젝트 설정 확인 ---
        // 'Queries Start In Colliders'가 true이면, 콜라이더 안에서 시작하는 쿼리(감지)도 작동합니다.
        Debug.Log($"[Detector] Physics 2D 설정: 'Queries Start In Colliders' = <color=yellow>{Physics2D.queriesStartInColliders}</color>");
    }

    // --- 4. OnTriggerEnter2D 대신 OnTriggerStay2D로 테스트 ---
    // Stay는 충돌 중인 모든 프레임마다 호출되어 더 확실하게 감지할 수 있습니다.
    void OnTriggerStay2D(Collider2D other)
    {
        // ▼▼▼ 충돌한 대상의 모든 정보 출력 ▼▼▼
        string otherLayer = LayerMask.LayerToName(other.gameObject.layer);

        Debug.LogWarning($"[Detector] <color=lime>충돌 유지 중!</color> " +
                         $"대상: {other.name}, " +
                         $"태그: {other.tag}, " +
                         $"레이어: {otherLayer}, " +
                         $"Rigidbody 2D 유무: {other.GetComponent<Rigidbody2D>() != null}");

        if (other.CompareTag("Barricade"))
        {
            mainScript.OnBarricadeDetected(other.GetComponent<Barricade>());
        }
    }
}