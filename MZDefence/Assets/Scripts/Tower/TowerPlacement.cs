using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    public GameObject towerPrefab;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // Z값 고정
            Instantiate(towerPrefab, mousePos, Quaternion.identity);
        }
    }
}
