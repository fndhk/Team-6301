// ���� �̸�: NoteData.cs (������ NotePatternData�� ����)

using System;
using UnityEngine;

[Serializable]
public class NotePatternData
{
    [Tooltip("������ ���۵� ��Ʈ ��ġ")]
    public float startBeat = 1f;

    [Tooltip("������ ������ ��Ʈ ��ġ (0�̸� �� ������ �ݺ�)")]
    public float endBeat = 0f;

    [Tooltip("�� ��Ʈ���� ��Ʈ�� �������� ���� (��: 4�� 4��Ʈ����)")]
    public float repeatInterval = 4f;

    [Tooltip("��Ʈ�� ������ ���� ��ȣ (1, 2, 3)")]
    public int laneIndex = 1;
}