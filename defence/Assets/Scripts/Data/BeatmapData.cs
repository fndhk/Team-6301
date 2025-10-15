//���� ��: BeatmapData.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Beatmap", menuName = "TowerDefense/Beatmap Data")]
public class BeatmapData : ScriptableObject
{
    // ------ �ű� �߰�: ���� ��Ʈ ����Ʈ ------
    [Header("�ű� ä�� �ý��� (������ ����)")]
    public List<SingleNoteData> notes = new List<SingleNoteData>();

    [Header("ä�� ����")]
    public float totalBeats = 0f;

    // ------ ���� patterns�� �ּ� ó�� (���� ȥ�� ����, ���� ���� ����!) ------
    [Header("�� ä�� �ý��� (��� �� ��)")]
    //public List<NotePatternData> patterns = new List<NotePatternData>();

    // ------ �ű� �߰�: � �ý����� ������� ���� ------
    [Tooltip("üũ�ϸ� notes ���, üũ �����ϸ� patterns ���")]
    public bool useNewBeatmapSystem = true;
}