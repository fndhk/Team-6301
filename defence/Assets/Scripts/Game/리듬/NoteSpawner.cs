// ���� �̸�: NoteSpawner.cs (�ѹ� + ����� ����)
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct NoteInfo
{
    public float beat;
    public int laneIndex; // �� ������ ���� 0, 1, 2, 3 �� �ϳ��� �����ϴ�.
    public InstrumentType instrumentType;
}

public class NoteSpawner : MonoBehaviour
{
    public static NoteSpawner instance;

    [Header("������Ʈ ����")]
    public GameObject notePrefab;
    public Transform[] spawnPoints;
    public Transform hitZone;
    public Transform noteParent;
    public BaseTower[] correspondingTowers;
    public InstrumentType[] laneInstruments = { InstrumentType.Drum, InstrumentType.Piano, InstrumentType.Cymbal, InstrumentType.Drum }; // ������ 4���̹Ƿ� �����۵� 4���� �����ִ� ���� �����մϴ�.

    [Header("��ũ ����")]
    public float syncOffset = 0f;

    [Header("��Ʈ ����")]
    public float beatsToFall = 2f;

    private float noteFallSpeed;
    private List<NoteInfo> allNotesToSpawn = new List<NoteInfo>();
    private int noteIndex = 0;
    private float songStartTime;
    private float noteDestroyY;
    private bool isSpawningStarted = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        RecalculateNoteSpeed();
        GenerateNotesFromPatterns();
        //songStartTime = Time.time;
    }

    public void RecalculateNoteSpeed()
    {
        float distanceToFall = Vector3.Distance(spawnPoints[0].position, hitZone.position);
        float timeToFall = beatsToFall * RhythmManager.instance.beatInterval;
        noteFallSpeed = distanceToFall / timeToFall;
        noteDestroyY = hitZone.position.y - 2f;
    }

    void GenerateNotesFromPatterns()
    {
        allNotesToSpawn.Clear();
        BeatmapData beatmap = GameSession.instance.selectedStage.beatmap;
        if (beatmap == null)
        {
            Debug.LogWarning("BeatmapData�� �����ϴ�!");
            return;
        }

        // ------ �ű� ����: �� �ý��� vs �� �ý��� �б� ------
        if (beatmap.useNewBeatmapSystem)
        {
            // �ű� �ý���: notes ����Ʈ ���
            Debug.Log("[NoteSpawner] �ű� ä�� �ý��� ���");

            foreach (var noteData in beatmap.notes)
            {
                int codeLaneIndex = noteData.laneIndex - 1; // 1~4 �� 0~3 ��ȯ

                // ��ȿ�� �˻�
                if (codeLaneIndex < 0 || codeLaneIndex >= laneInstruments.Length)
                {
                    Debug.LogWarning($"�߸��� ���� �ε���: {noteData.laneIndex} (beat: {noteData.beat})");
                    continue;
                }

                NoteInfo newNote = new NoteInfo
                {
                    beat = noteData.beat,
                    laneIndex = codeLaneIndex,
                    instrumentType = laneInstruments[codeLaneIndex]
                };
                allNotesToSpawn.Add(newNote);
            }
        }
        else
        {
            // �� �ý���: patterns ��� (�ּ� ���� �ʿ�)
            Debug.Log("[NoteSpawner] �� ä�� �ý��� ���");

            // �Ʒ� �ڵ�� patterns �ּ��� �����ؾ� �۵���
            /*
            foreach (var pattern in beatmap.patterns)
            {
                float patternEndBeat = (pattern.endBeat <= 0) ? 9999f : pattern.endBeat;
                if (pattern.repeatInterval <= 0) continue;

                for (float currentBeat = pattern.startBeat; currentBeat <= patternEndBeat; currentBeat += pattern.repeatInterval)
                {
                    int codeLaneIndex = pattern.laneIndex - 1;

                    NoteInfo newNote = new NoteInfo
                    {
                        beat = currentBeat,
                        laneIndex = codeLaneIndex,
                        instrumentType = laneInstruments[codeLaneIndex]
                    };
                    allNotesToSpawn.Add(newNote);
                }
            }
            */
        }

        // ��Ʈ ������� ����
        allNotesToSpawn = allNotesToSpawn.OrderBy(note => note.beat).ToList();

        Debug.Log($"[NoteSpawner] �� {allNotesToSpawn.Count}���� ��Ʈ ���� �Ϸ�!");
    }

    void Update()
    {
        if (!isSpawningStarted) return;
        if (noteIndex >= allNotesToSpawn.Count) return;
        float songPosition = (Time.time - songStartTime) + syncOffset;
        float currentBeat = songPosition / RhythmManager.instance.beatInterval;

        while (noteIndex < allNotesToSpawn.Count && currentBeat >= allNotesToSpawn[noteIndex].beat - beatsToFall)
        {
            NoteInfo noteToSpawn = allNotesToSpawn[noteIndex];
            if (correspondingTowers[noteToSpawn.laneIndex] != null &&
                correspondingTowers[noteToSpawn.laneIndex].gameObject.activeSelf)
            {
                SpawnNote(noteToSpawn);
            }
            noteIndex++;
        }
    }
    public void StartSpawning()
    {
        StartSpawningAtTime(Time.time);
    }
    public void StartSpawningAtTime(float startTime)
    {
        if (isSpawningStarted) return;

        isSpawningStarted = true;
        songStartTime = startTime;  // AudioManager�� ������ �ð� ���!

        Debug.Log($"NoteSpawner: ��Ʈ ���� ����! (Time: {songStartTime})");
    }
    private void SpawnNote(NoteInfo noteInfo)
    {
        // ���� ����� �α� �߰� ����
        float currentTime = Time.time;
        float expectedTime = songStartTime + (noteInfo.beat * RhythmManager.instance.beatInterval);
        float timeDiff = currentTime - expectedTime;

        Debug.Log($"<color=yellow>[��Ʈ ����] Beat {noteInfo.beat:F2}, " +
                  $"����: {expectedTime:F4}��, ����: {currentTime:F4}��, " +
                  $"����: {timeDiff * 1000:F2}ms</color>");

        GameObject noteGO = Instantiate(notePrefab, spawnPoints[noteInfo.laneIndex].position, Quaternion.identity, noteParent);
        NoteObject noteObject = noteGO.GetComponent<NoteObject>();
        if (noteObject != null)
        {
            noteObject.fallSpeed = this.noteFallSpeed;
            noteObject.instrumentType = noteInfo.instrumentType;
            noteObject.laneIndex = noteInfo.laneIndex; // �ڵ�� ���� ��ȣ(0,1,2,3)�� �״�� ����
            noteObject.destroyYPosition = this.noteDestroyY;
        }
    }
}