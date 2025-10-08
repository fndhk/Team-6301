using System.Collections.Generic;
using UnityEngine;

// Wave Ŭ������ EnemySpawner.cs�� �̹� �����Ƿ�, �״�� ����մϴ�.
// ���� �ٸ� ���Ͽ� �ִٸ� using�� ���� �����;� �մϴ�.

[CreateAssetMenu(fileName = "Stage_", menuName = "TowerDefense/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("�������� ����")]
    public int stageIndex;
    public string stageName;

    [Header("���� ���̺� ����")]
    public List<Wave> waves = new List<Wave>();
}