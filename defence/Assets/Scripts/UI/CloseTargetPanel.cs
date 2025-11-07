using UnityEngine;

public class CloseTargetPanel : MonoBehaviour
{
    void Update()
    {
        // ESC 키가 눌렸는지 매 프레임 확인
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 이 스크립트가 붙어있는 자기 자신(패널)을 끕니다.
            gameObject.SetActive(false);
        }
    }

}