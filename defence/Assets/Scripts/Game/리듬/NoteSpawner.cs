// 파일 이름: NoteSpawner.cs (롤백 + 디버그 버전)
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct NoteInfo
{
    public float beat;
    public int laneIndex; // 이 변수는 이제 0, 1, 2, 3 중 하나를 가집니다.
    public InstrumentType instrumentType;
}

public class NoteSpawner : MonoBehaviour
{
    public static NoteSpawner instance;

    [Header("오브젝트 연결")]
    public GameObject notePrefab;
    public Transform[] spawnPoints;
    public Transform hitZone;
    public Transform noteParent;
    public BaseTower[] correspondingTowers;
    public InstrumentType[] laneInstruments = { InstrumentType.Drum, InstrumentType.Piano, InstrumentType.Cymbal, InstrumentType.Drum }; // 레인이 4개이므로 아이템도 4개로 맞춰주는 것이 안전합니다.

    [Header("노트 설정")]
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
        if (beatmap == null) return;

        foreach (var pattern in beatmap.patterns)
        {
            float patternEndBeat = (pattern.endBeat <= 0) ? 9999f : pattern.endBeat;
            if (pattern.repeatInterval <= 0) continue;

            for (float currentBeat = pattern.startBeat; currentBeat <= patternEndBeat; currentBeat += pattern.repeatInterval)
            {
                // BeatmapData의 레인 번호(1,2,3,4)를 코드용 번호(0,1,2,3)로 변환합니다.
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
        allNotesToSpawn = allNotesToSpawn.OrderBy(note => note.beat).ToList();
    }

    void Update()
    {
        if (!isSpawningStarted) return;
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
    public void StartSpawning()
    {
        if (isSpawningStarted) return; // 이미 시작했으면 무시

        isSpawningStarted = true;
        songStartTime = Time.time; // 현재 시간을 노래 시작 시간으로 설정

        Debug.Log("NoteSpawner: 노트 스폰 시작!");
    }

    private void SpawnNote(NoteInfo noteInfo)
    {
        // ▼▼▼ 디버그 로그 추가 ▼▼▼
        Debug.Log($"<color=cyan>노트 생성:</color> 레인 번호 {noteInfo.laneIndex}번");

        GameObject noteGO = Instantiate(notePrefab, spawnPoints[noteInfo.laneIndex].position, Quaternion.identity, noteParent);
        NoteObject noteObject = noteGO.GetComponent<NoteObject>();
        if (noteObject != null)
        {
            noteObject.fallSpeed = this.noteFallSpeed;
            noteObject.instrumentType = noteInfo.instrumentType;
            noteObject.laneIndex = noteInfo.laneIndex; // 코드용 레인 번호(0,1,2,3)를 그대로 전달
            noteObject.destroyYPosition = this.noteDestroyY;
        }
    }
}