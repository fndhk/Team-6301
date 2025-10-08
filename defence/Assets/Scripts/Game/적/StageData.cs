using System.Collections.Generic;
using UnityEngine;

// Wave 클래스는 EnemySpawner.cs에 이미 있으므로, 그대로 사용합니다.
// 만약 다른 파일에 있다면 using을 통해 가져와야 합니다.

[CreateAssetMenu(fileName = "Stage_", menuName = "TowerDefense/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("스테이지 정보")]
    public int stageIndex;
    public string stageName;

    [Header("등장 웨이브 정보")]
    public List<Wave> waves = new List<Wave>();
}