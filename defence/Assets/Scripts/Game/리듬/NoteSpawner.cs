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
    
    [Header("특수 노트 설정")]
    [SerializeField] private float specialNoteChance = 0.01f; // 1% 확률

    [Header("싱크 조정")]
    public float syncOffset = 0f;

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
        if (beatmap == null)
        {
            Debug.LogWarning("BeatmapData가 없습니다!");
            return;
        }

        // ------ 신규 수정: 새 시스템 vs 구 시스템 분기 ------
        if (beatmap.useNewBeatmapSystem)
        {
            // 신규 시스템: notes 리스트 사용
            Debug.Log("[NoteSpawner] 신규 채보 시스템 사용");

            foreach (var noteData in beatmap.notes)
            {
                int codeLaneIndex = noteData.laneIndex - 1; // 1~4 → 0~3 변환

                // 유효성 검사
                if (codeLaneIndex < 0 || codeLaneIndex >= laneInstruments.Length)
                {
                    Debug.LogWarning($"잘못된 레인 인덱스: {noteData.laneIndex} (beat: {noteData.beat})");
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
            // 구 시스템: patterns 사용 (주석 해제 필요)
            Debug.Log("[NoteSpawner] 구 채보 시스템 사용");

            // 아래 코드는 patterns 주석을 해제해야 작동함
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

        // 비트 순서대로 정렬
        allNotesToSpawn = allNotesToSpawn.OrderBy(note => note.beat).ToList();

        Debug.Log($"[NoteSpawner] 총 {allNotesToSpawn.Count}개의 노트 생성 완료!");
    }

    void Update()
    {
        if (!isSpawningStarted) return;

        // 1. 현재 노래 재생 시간(초) 계산
        float songPosition = (Time.time - songStartTime) + syncOffset;
        // 현재 비트 계산
        float currentBeat = songPosition / RhythmManager.instance.beatInterval;

        // 2. 노트 생성 (기존 로직)
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

        // 3. ---  음악 기준 루프 로직 ---
        // AudioManager에서 실제 음악 파일의 길이를 초 단위로 가져옵니다.
        float songDurationInSeconds = AudioManager.instance.GetMusicDuration();
        if (songDurationInSeconds <= 0) return; // 음악 길이가 없으면 루프 방지

        // 4. 현재 노래 재생 시간(songPosition)이 총 음악 길이(songDurationInSeconds)를 넘어섰는지 확인
        if (songPosition > songDurationInSeconds)
        {
            // 5. 노트 인덱스를 0으로 리셋
            noteIndex = 0;

            // 6. (중요) songStartTime을 정확히 음악 길이(초)만큼 뒤로 민다.
            songStartTime += songDurationInSeconds;

            Debug.Log($"<color=cyan>음악 기준 루프! noteIndex 리셋. new songStartTime: {songStartTime}</color>");

            // 7. 루프 직후의 첫 노트를 스폰하기 위해 현재 비트를 다시 계산하고 스폰 로직을 한 번 더 실행
            songPosition = (Time.time - songStartTime) + syncOffset;
            currentBeat = songPosition / RhythmManager.instance.beatInterval;

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
    }
    public void StartSpawning()
    {
        StartSpawningAtTime(Time.time);
    }
    public void StartSpawningAtTime(float startTime)
    {
        if (isSpawningStarted) return;

        isSpawningStarted = true;
        songStartTime = startTime;  // AudioManager가 전달한 시간 사용!

        Debug.Log($"NoteSpawner: 노트 스폰 시작! (Time: {songStartTime})");
    }
    private void SpawnNote(NoteInfo noteInfo)
    {
        // ▼▼▼ 디버그 로그 추가 ▼▼▼
        float currentTime = Time.time;
        float expectedTime = songStartTime + (noteInfo.beat * RhythmManager.instance.beatInterval);
        float timeDiff = currentTime - expectedTime;

        Debug.Log($"<color=yellow>[노트 스폰] Beat {noteInfo.beat:F2}, " +
                  $"예상: {expectedTime:F4}초, 실제: {currentTime:F4}초, " +
                  $"차이: {timeDiff * 1000:F2}ms</color>");

        GameObject noteGO = Instantiate(notePrefab, spawnPoints[noteInfo.laneIndex].position, Quaternion.identity, noteParent);
        NoteObject noteObject = noteGO.GetComponent<NoteObject>();
        if (Random.value < specialNoteChance)
        {
            noteObject.isSpecialNote = true;
        }
        if (noteObject != null)
        {
            noteObject.fallSpeed = this.noteFallSpeed;
            noteObject.instrumentType = noteInfo.instrumentType;
            noteObject.laneIndex = noteInfo.laneIndex; // 코드용 레인 번호(0,1,2,3)를 그대로 전달
            noteObject.destroyYPosition = this.noteDestroyY;
        }
    }
}