// ���� �̸�: NoteSpawner.cs (��ü ��ü)
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct NoteInfo
{
    public float beat;
    public int laneIndex;
    public InstrumentType instrumentType;
}

public class NoteSpawner : MonoBehaviour
{
    // ���� �ٸ� ��ũ��Ʈ���� ������ �� �ֵ��� instance�� �߰��մϴ�. ����
    public static NoteSpawner instance;

    [Header("������Ʈ ����")]
    public GameObject notePrefab;
    public Transform[] spawnPoints;
    public Transform hitZone;
    public Transform noteParent;
    public BaseTower[] correspondingTowers;
    public InstrumentType[] laneInstruments = { InstrumentType.Drum, InstrumentType.Piano, InstrumentType.Cymbal };

    [Header("��Ʈ ����")]
    public float beatsToFall = 2f;

    private float noteFallSpeed;
    private List<NoteInfo> allNotesToSpawn = new List<NoteInfo>();
    private int noteIndex = 0;
    private float songStartTime;
    private float noteDestroyY;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // ��Ʈ ���� �ӵ��� �ı� ��ġ�� ����մϴ�.
        RecalculateNoteSpeed();

        GenerateNotesFromPatterns();
        songStartTime = Time.time;
    }

    // ���� �ٽ� ���� ���� ����
    // BPM�� �ٲ� ������ ȣ��� �ӵ� ���� �Լ��Դϴ�.
    public void RecalculateNoteSpeed()
    {
        float distanceToFall = Vector3.Distance(spawnPoints[0].position, hitZone.position);
        float timeToFall = beatsToFall * RhythmManager.instance.beatInterval;
        noteFallSpeed = distanceToFall / timeToFall;
        noteDestroyY = hitZone.position.y - 2f;
        Debug.Log($"��Ʈ �ӵ� ���� �Ϸ�: {noteFallSpeed}");
    }

    void GenerateNotesFromPatterns()
    {
        allNotesToSpawn.Clear();
        BeatmapData beatmap = GameSession.instance.selectedStage.beatmap;
        if (beatmap == null) return;

        foreach (var pattern in beatmap.patterns)
        {
            float patternEndBeat = (pattern.endBeat <= 0) ? 9999f : pattern.endBeat;
            if (pattern.repeatInterval <= 0) continue;

            for (float currentBeat = pattern.startBeat; currentBeat <= patternEndBeat; currentBeat += pattern.repeatInterval)
            {
                NoteInfo newNote = new NoteInfo
                {
                    beat = currentBeat,
                    laneIndex = pattern.laneIndex - 1,
                    instrumentType = laneInstruments[pattern.laneIndex - 1]
                };
                allNotesToSpawn.Add(newNote);
            }
        }
        allNotesToSpawn = allNotesToSpawn.OrderBy(note => note.beat).ToList();
    }

    void Update()
    {
        if (noteIndex >= allNotesToSpawn.Count) return;
        float songPosition = Time.time - songStartTime;
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

    private void SpawnNote(NoteInfo noteInfo)
    {
        GameObject noteGO = Instantiate(notePrefab, spawnPoints[noteInfo.laneIndex].position, Quaternion.identity, noteParent);
        NoteObject noteObject = noteGO.GetComponent<NoteObject>();
        if (noteObject != null)
        {
            noteObject.fallSpeed = this.noteFallSpeed;
            noteObject.instrumentType = noteInfo.instrumentType;
            noteObject.laneIndex = noteInfo.laneIndex;
            noteObject.destroyYPosition = this.noteDestroyY;
        }
    }
}