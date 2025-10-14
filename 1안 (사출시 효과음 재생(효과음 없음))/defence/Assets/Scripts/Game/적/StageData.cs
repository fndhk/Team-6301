// ���� �̸�: StageData.cs
using System.Collections.Generic;
using UnityEngine;

// ������ EnemySpawner.cs�� ������� Wave Ŭ������ ���⿡ �ٽ� �����ϰų�,
// �ϳ��� ���Ͽ��� �����ϴ� ���� �����ϴ�. ������ �״�� ����մϴ�.

[CreateAssetMenu(fileName = "Stage_", menuName = "TowerDefense/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("�������� �⺻ ����")]
    public int stageIndex;
    public string stageName;
    [Header("���� ����")] // ���� �� �κ��� �߰��ϰų� �����մϴ� ����
    public AudioClip baseMusic;
    public AudioClip drumTrack;
    public AudioClip pianoTrack;
    public AudioClip cymbalTrack;

    [Header("���� & ���� ����")]
    public float bpm = 120f; //  ���������� �⺻ BPM
    public AudioClip stageMusic; //  ������������ ����� ��� ����

    [Header("���� ���̺� ����")]
    public List<Wave> waves = new List<Wave>();
    [Header("ä�� ����")]
    public BeatmapData beatmap;
}