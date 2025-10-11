// 파일 이름: NoteSpawner.cs (전체 교체)
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
    // ▼▼▼ 다른 스크립트에서 접근할 수 있도록 instance를 추가합니다. ▼▼▼
    public static NoteSpawner instance;

    [Header("오브젝트 연결")]
    public GameObject notePrefab;
    public Transform[] spawnPoints;
    public Transform hitZone;
    public Transform noteParent;
    public BaseTower[] correspondingTowers;
    public InstrumentType[] laneInstruments = { InstrumentType.Drum, InstrumentType.Piano, InstrumentType.Cymbal };

    [Header("노트 설정")]
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
        // 노트 낙하 속도와 파괴 위치를 계산합니다.
        RecalculateNoteSpeed();

        GenerateNotesFromPatterns();
        songStartTime = Time.time;
    }

    // ▼▼▼ 핵심 변경 사항 ▼▼▼
    // BPM이 바뀔 때마다 호출될 속도 재계산 함수입니다.
    public void RecalculateNoteSpeed()
    {
        float distanceToFall = Vector3.Distance(spawnPoints[0].position, hitZone.position);
        float timeToFall = beatsToFall * RhythmManager.instance.beatInterval;
        noteFallSpeed = distanceToFall / timeToFall;
        noteDestroyY = hitZone.position.y - 2f;
        Debug.Log($"노트 속도 재계산 완료: {noteFallSpeed}");
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