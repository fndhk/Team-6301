//���� ��: BeatmapEditorManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BeatmapEditorManager : MonoBehaviour
{
    [Header("����� ����")]
    public AudioSource audioSource;
    public float bpm = 120f;

    [Header("UI ������Ʈ")]
    public RectTransform timelinePanel;
    public RectTransform noteGridPanel;
    public Button playPauseButton;
    public Button stopButton;
    public Button exportButton;
    public Slider timelineSlider;
    public TMP_Text currentBeatText;
    public TMP_Text currentTimeText;
    public TMP_InputField bpmInputField;
    [Header("���� ���� (���Ӱ� �����ϰ�!)")]
    public float gameBeatsToFall = 2f; // �� ������ beatsToFall ���� �����ϰ�!
    [Header("��Ʈ�� ����")]
    public RectTransform viewport;
    public Color hitZoneColor = new Color(1f, 0f, 0f, 0.5f);
    private GameObject hitZoneLine;
    private TextMeshProUGUI hitZoneBeatText;

    [Header("Ÿ�Ӷ��� ���� ����")]
    public float timeMarkerInterval = 5f;
    public Color timeMarkerColor = new Color(1f, 1f, 1f, 0.5f);
    public Font timeMarkerFont;

    [Header("�׸��� ����")]
    public float pixelsPerBeat = 100f;
    public Color[] laneColors = new Color[4];

    [Header("��Ʈ ������")]
    public GameObject noteMarkerPrefab;

    [Header("���� ��ư")]
    public Button[] laneButtons = new Button[4];

    private float beatInterval;
    private float currentBeat = 0f;
    private bool isPlaying = false;
    private List<SingleNoteData> placedNotes = new List<SingleNoteData>();
    private List<GameObject> noteMarkers = new List<GameObject>();
    private float totalBeats = 0f;
    private ScrollRect scrollRect;

    void Start()
    {
        UpdateBeatInterval();
        SetupUI();

        scrollRect = noteGridPanel.GetComponentInParent<ScrollRect>();
        viewport = scrollRect.viewport;

        noteGridPanel.sizeDelta = new Vector2(5000f, 320f);
        Debug.Log($"NoteGridPanel �ʱ� ũ��: {noteGridPanel.sizeDelta}");

        CreateFixedHitZone();

        if (audioSource != null && audioSource.clip != null)
        {
            totalBeats = (audioSource.clip.length / 60f) * bpm;
            timelineSlider.maxValue = totalBeats;
            ResizeNoteGridPanel(totalBeats);
        }
    }

    void UpdateBeatInterval()
    {
        beatInterval = 60f / bpm;
    }

    void SetupUI()
    {
        playPauseButton.onClick.AddListener(TogglePlayPause);
        stopButton.onClick.AddListener(StopPlayback);
        exportButton.onClick.AddListener(ExportBeatmap);
        bpmInputField.onEndEdit.AddListener(OnBPMChanged);
        timelineSlider.onValueChanged.AddListener(OnTimelineChanged);

        for (int i = 0; i < laneButtons.Length; i++)
        {
            int laneIndex = i + 1;
            laneButtons[i].onClick.AddListener(() => PlaceNoteAtCurrentBeat(laneIndex));

            if (laneColors.Length > i)
            {
                ColorBlock colors = laneButtons[i].colors;
                colors.normalColor = laneColors[i];
                laneButtons[i].colors = colors;
            }
        }

        bpmInputField.text = bpm.ToString();
    }

    void Update()
    {
        if (isPlaying && audioSource.isPlaying)
        {
            currentBeat = (audioSource.time / 60f) * bpm;
            timelineSlider.value = currentBeat;
            UpdateBeatDisplay();
            AutoScrollToCurrentBeat();
        }

        if (hitZoneBeatText != null)
        {
            float hitZoneBeat = CalculateBeatAtHitZone();
            hitZoneBeatText.text = $"{hitZoneBeat:F2}";
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) PlaceNoteAtCurrentBeat(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) PlaceNoteAtCurrentBeat(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) PlaceNoteAtCurrentBeat(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) PlaceNoteAtCurrentBeat(4);

        if (Input.GetKeyDown(KeyCode.Space)) TogglePlayPause();
        if (Input.GetKeyDown(KeyCode.Backspace)) RemoveLastNote();
        if (Input.GetKeyDown(KeyCode.Delete)) RemoveNoteAtCurrentBeat();
    }

    void ResizeNoteGridPanel(float totalBeats)
    {
        float requiredWidth = Mathf.Max(totalBeats * pixelsPerBeat + 200f, 2000f);
        noteGridPanel.sizeDelta = new Vector2(requiredWidth, noteGridPanel.sizeDelta.y);

        Debug.Log($"NoteGridPanel ũ�� ����: {requiredWidth}px (�� {totalBeats:F2} ��Ʈ)");
        ClearTimelineMarkers();
        CreateTimelineMarkers();
    }

    void AutoScrollToCurrentBeat()
    {
        if (scrollRect == null || noteGridPanel == null) return;

        float contentWidth = noteGridPanel.sizeDelta.x;
        float viewportWidth = scrollRect.viewport.rect.width;

        float scrollableWidth = contentWidth - viewportWidth;
        if (scrollableWidth <= 0) return;

        float currentX = currentBeat * pixelsPerBeat;
        float normalizedPosition = Mathf.Clamp01(currentX / scrollableWidth);

        scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
            scrollRect.horizontalNormalizedPosition,
            normalizedPosition,
            Time.deltaTime * 5f
        );
    }

    void ScrollToBeat(float beat)
    {
        if (scrollRect == null || noteGridPanel == null) return;

        float contentWidth = noteGridPanel.sizeDelta.x;
        float viewportWidth = scrollRect.viewport.rect.width;
        float scrollableWidth = contentWidth - viewportWidth;

        if (scrollableWidth <= 0) return;

        float targetX = beat * pixelsPerBeat;
        scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(targetX / scrollableWidth);
    }

    void PlaceNoteAtCurrentBeat(int laneIndex)
    {
        float hitZoneBeat = CalculateBeatAtHitZone();

        // ------ �ٽ� ����: �������� ����! ------
        // �����Ϳ��� ��Ʈ���� ����Ű�� ���� = ���ӿ��� ��Ʈ�� ���� ����
        // ������ (beat - beatsToFall) ������ �����ϹǷ�
        // �����Ϳ����� ��Ʈ�� beat�� �״�� ����!
        float snappedBeat = Mathf.Round(hitZoneBeat * 4f) / 4f;

        if (placedNotes.Any(n => Mathf.Abs(n.beat - snappedBeat) < 0.01f && n.laneIndex == laneIndex))
        {
            Debug.LogWarning($"Beat {snappedBeat}, Lane {laneIndex}�� �̹� ��Ʈ�� �ֽ��ϴ�!");
            return;
        }

        SingleNoteData newNote = new SingleNoteData
        {
            beat = snappedBeat,  // ��Ʈ�� beat �״��!
            laneIndex = laneIndex
        };

        placedNotes.Add(newNote);
        placedNotes = placedNotes.OrderBy(n => n.beat).ToList();
        CreateNoteMarker(newNote);

        Debug.Log($"��Ʈ ��ġ: Beat {snappedBeat:F2} (��Ʈ�� ����), Lane {laneIndex}");
    }

    float CalculateBeatAtHitZone()
    {
        if (scrollRect == null || viewport == null) return currentBeat;

        float viewportWidth = viewport.rect.width;
        float hitZoneScreenX = viewportWidth * 0.2f;
        float scrollOffset = scrollRect.horizontalNormalizedPosition *
                             (noteGridPanel.sizeDelta.x - viewportWidth);
        float hitZoneAbsoluteX = scrollOffset + hitZoneScreenX;
        float beat = hitZoneAbsoluteX / pixelsPerBeat;

        return beat;
    }

    void CreateNoteMarker(SingleNoteData note)
    {
        GameObject marker = Instantiate(noteMarkerPrefab, noteGridPanel);
        RectTransform markerRect = marker.GetComponent<RectTransform>();

        markerRect.anchorMin = new Vector2(0, 1);
        markerRect.anchorMax = new Vector2(0, 1);
        markerRect.pivot = new Vector2(0.5f, 0.5f);

        float xPos = note.beat * pixelsPerBeat;
        float yPos = -40f - (note.laneIndex - 1) * 80f;

        markerRect.anchoredPosition = new Vector2(xPos, yPos);
        markerRect.sizeDelta = new Vector2(40f, 60f);

        Image markerImage = marker.GetComponent<Image>();
        if (markerImage != null && laneColors.Length >= note.laneIndex)
        {
            markerImage.color = laneColors[note.laneIndex - 1];
        }

        NoteMarkerUI markerUI = marker.GetComponent<NoteMarkerUI>();
        if (markerUI == null)
        {
            markerUI = marker.AddComponent<NoteMarkerUI>();
        }
        markerUI.Initialize(note, this);

        noteMarkers.Add(marker);

        Debug.Log($"��Ʈ ����: Beat {note.beat}, Lane {note.laneIndex}, Pos ({xPos}, {yPos})");
    }

    void RefreshAllNoteMarkers()
    {
        foreach (var marker in noteMarkers)
        {
            if (marker != null) Destroy(marker);
        }
        noteMarkers.Clear();

        foreach (var note in placedNotes)
        {
            CreateNoteMarker(note);
        }
    }

    void TogglePlayPause()
    {
        isPlaying = !isPlaying;

        if (isPlaying)
        {
            audioSource.time = (currentBeat / bpm) * 60f;
            audioSource.Play();
            playPauseButton.GetComponentInChildren<TMP_Text>().text = "Pause";
        }
        else
        {
            audioSource.Pause();
            playPauseButton.GetComponentInChildren<TMP_Text>().text = "Play";
        }
    }

    void StopPlayback()
    {
        isPlaying = false;
        audioSource.Stop();
        currentBeat = 0f;
        timelineSlider.value = 0f;
        UpdateBeatDisplay();
        playPauseButton.GetComponentInChildren<TMP_Text>().text = "Play";

        ScrollToBeat(0f);
    }

    void OnTimelineChanged(float value)
    {
        if (!isPlaying)
        {
            currentBeat = value;
            audioSource.time = (currentBeat / bpm) * 60f;
            UpdateBeatDisplay();
        }
    }

    void OnBPMChanged(string newBpmText)
    {
        if (float.TryParse(newBpmText, out float newBpm))
        {
            bpm = Mathf.Clamp(newBpm, 60f, 300f);
            UpdateBeatInterval();
            bpmInputField.text = bpm.ToString();

            if (audioSource.clip != null)
            {
                totalBeats = (audioSource.clip.length / 60f) * bpm;
                timelineSlider.maxValue = totalBeats;
                ResizeNoteGridPanel(totalBeats);
            }
        }
    }

    void UpdateBeatDisplay()
    {
        currentBeatText.text = $"Beat: {currentBeat:F2}";
        if (audioSource != null)
        {
            currentTimeText.text = $"Time: {audioSource.time:F2}s";
        }
    }

    void RemoveLastNote()
    {
        if (placedNotes.Count > 0)
        {
            placedNotes.RemoveAt(placedNotes.Count - 1);

            if (noteMarkers.Count > 0)
            {
                Destroy(noteMarkers[noteMarkers.Count - 1]);
                noteMarkers.RemoveAt(noteMarkers.Count - 1);
            }

            Debug.Log("������ ��Ʈ ����");
        }
    }

    void RemoveNoteAtCurrentBeat()
    {
        float snappedBeat = Mathf.Round(currentBeat * 4f) / 4f;

        for (int i = placedNotes.Count - 1; i >= 0; i--)
        {
            if (Mathf.Abs(placedNotes[i].beat - snappedBeat) < 0.01f)
            {
                placedNotes.RemoveAt(i);
                if (i < noteMarkers.Count)
                {
                    Destroy(noteMarkers[i]);
                    noteMarkers.RemoveAt(i);
                }
            }
        }

        RefreshAllNoteMarkers();
    }

    void ExportBeatmap()
    {
        if (placedNotes.Count == 0)
        {
            Debug.LogWarning("��ġ�� ��Ʈ�� �����ϴ�!");
            return;
        }

        BeatmapExporter.Export(placedNotes, bpm);
    }

    public void RemoveNote(SingleNoteData noteToRemove)
    {
        for (int i = placedNotes.Count - 1; i >= 0; i--)
        {
            if (Mathf.Abs(placedNotes[i].beat - noteToRemove.beat) < 0.01f &&
                placedNotes[i].laneIndex == noteToRemove.laneIndex)
            {
                placedNotes.RemoveAt(i);
                Debug.Log($"��Ʈ ����: Beat {noteToRemove.beat}, Lane {noteToRemove.laneIndex}");
                return;
            }
        }
    }

    void CreateTimelineMarkers()
    {
        if (audioSource == null || audioSource.clip == null) return;

        float totalSeconds = audioSource.clip.length;

        for (float time = 0f; time <= totalSeconds; time += timeMarkerInterval)
        {
            float beat = (time / 60f) * bpm;
            CreateTimeMarker(beat, time);
        }
    }

    void CreateTimeMarker(float beat, float timeInSeconds)
    {
        GameObject markerLine = new GameObject($"TimeMarker_{timeInSeconds}s");
        markerLine.transform.SetParent(noteGridPanel, false);

        Image lineImage = markerLine.AddComponent<Image>();
        lineImage.color = timeMarkerColor;

        RectTransform lineRect = markerLine.GetComponent<RectTransform>();
        lineRect.anchorMin = new Vector2(0, 0);
        lineRect.anchorMax = new Vector2(0, 1);
        lineRect.pivot = new Vector2(0.5f, 0.5f);

        float xPos = beat * pixelsPerBeat;
        lineRect.anchoredPosition = new Vector2(xPos, 0f);
        lineRect.sizeDelta = new Vector2(2f, 0f);

        GameObject markerText = new GameObject("Text");
        markerText.transform.SetParent(markerLine.transform, false);

        TextMeshProUGUI textComponent = markerText.AddComponent<TextMeshProUGUI>();
        textComponent.text = $"{timeInSeconds:F0}s";
        textComponent.fontSize = 14;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Center;

        RectTransform textRect = markerText.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 1f);
        textRect.anchorMax = new Vector2(0.5f, 1f);
        textRect.pivot = new Vector2(0.5f, 0f);
        textRect.anchoredPosition = new Vector2(0f, 5f);
        textRect.sizeDelta = new Vector2(50f, 20f);
    }

    void ClearTimelineMarkers()
    {
        Transform[] children = noteGridPanel.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.name.StartsWith("TimeMarker_"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    void CreateFixedHitZone()
    {
        hitZoneLine = new GameObject("FixedHitZone");
        hitZoneLine.transform.SetParent(viewport, false);

        Image lineImage = hitZoneLine.AddComponent<Image>();
        lineImage.color = hitZoneColor;
        lineImage.raycastTarget = false;

        RectTransform lineRect = hitZoneLine.GetComponent<RectTransform>();
        lineRect.anchorMin = new Vector2(0.2f, 0f);
        lineRect.anchorMax = new Vector2(0.2f, 1f);
        lineRect.pivot = new Vector2(0.5f, 0.5f);
        lineRect.anchoredPosition = Vector2.zero;
        lineRect.sizeDelta = new Vector2(5f, 0f);

        // ��Ʈ�� �ؽ�Ʈ
        GameObject beatTextObj = new GameObject("HitZoneBeat");
        beatTextObj.transform.SetParent(hitZoneLine.transform, false);

        hitZoneBeatText = beatTextObj.AddComponent<TextMeshProUGUI>();
        hitZoneBeatText.text = "0.00";
        hitZoneBeatText.fontSize = 20;
        hitZoneBeatText.color = Color.red;
        hitZoneBeatText.alignment = TextAlignmentOptions.Center;
        hitZoneBeatText.fontStyle = FontStyles.Bold;

        RectTransform beatTextRect = beatTextObj.GetComponent<RectTransform>();
        beatTextRect.anchorMin = new Vector2(0.5f, 1f);
        beatTextRect.anchorMax = new Vector2(0.5f, 1f);
        beatTextRect.pivot = new Vector2(0.5f, 0f);
        beatTextRect.anchoredPosition = new Vector2(0f, 10f);
        beatTextRect.sizeDelta = new Vector2(100f, 40f);

        // ------ �ű� �߰�: beatsToFall ��� ǥ�� ------
        GameObject warningTextObj = new GameObject("OffsetWarning");
        warningTextObj.transform.SetParent(hitZoneLine.transform, false);

        TextMeshProUGUI warningText = warningTextObj.AddComponent<TextMeshProUGUI>();
        warningText.text = $"Offset: -{gameBeatsToFall} beats";
        warningText.fontSize = 14;
        warningText.color = Color.yellow;
        warningText.alignment = TextAlignmentOptions.Center;

        RectTransform warningRect = warningTextObj.GetComponent<RectTransform>();
        warningRect.anchorMin = new Vector2(0.5f, 1f);
        warningRect.anchorMax = new Vector2(0.5f, 1f);
        warningRect.pivot = new Vector2(0.5f, 0f);
        warningRect.anchoredPosition = new Vector2(0f, 50f);
        warningRect.sizeDelta = new Vector2(150f, 30f);
    }

}