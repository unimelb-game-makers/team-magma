using UnityEngine;
using System.Collections.Generic;

public class BeatSpawner : MonoBehaviour
{
    private const int BEAT_PREFIX = 4;
    // The distance that a beat will travel in one second
    private const float BEAT_DISTANCE = 700f;
    
    // IMPORTANT! This is a magic number to offset any weird visual de-sync between the actual beat and the UI
    // Note(Alex): In my findings, the UI seems to reach the center slightly too late.
    private const float ANIMATION_OFFSET_TIME = -0.05f;

    [SerializeField] private RectTransform beatHolder;
    [SerializeField] private BeatPopupItem sampleBeatPopupItem;

    [SerializeField] private RectTransform leftTarget;
    [SerializeField] private RectTransform rightTarget;
    
    private BeatHandler _beatHandler;

    private readonly Dictionary<int, BeatPopupItem> _popupItems = new();

    public void Init(BeatHandler beatHandler)
    {
        _beatHandler = beatHandler;
    }

    /// <summary>
    /// Spawns a beat based on the time it will hit the target.
    /// </summary>
    /// <param name="beat">The beat to spawn</param>
    private void SpawnBeat(int beat)
    {
        if (!_beatHandler.IsBeat(beat)) return;
        
        // The time to target is the amount of beats we spawned ahead of time
        float travelTime = _beatHandler.BeatInterval * BEAT_PREFIX + ANIMATION_OFFSET_TIME;
        float distance = BEAT_DISTANCE * travelTime;

        BeatPopupItem beatPopupItem = Instantiate(sampleBeatPopupItem, beatHolder);
        beatPopupItem.Init(this, beat, leftTarget, rightTarget, distance, travelTime);
        _popupItems.Add(beat, beatPopupItem);
    }

    /// <summary>
    /// Creates a new beat following the initial beat offset
    /// </summary>
    /// <param name="beat"></param>
    public void OnBeat(int beat)
    {
        SpawnBeat(beat + BEAT_PREFIX);
    }

    /// <summary>
    /// Resolves the beat by removing it from the dictionary
    /// </summary>
    /// <param name="beat"></param>
    public void ResolveBeat(int beat)
    {
        if (_popupItems.ContainsKey(beat))
            _popupItems.Remove(beat);
    }

    public void ProcessBeat(BeatResult beatResult)
    {
        if (!_popupItems.TryGetValue(beatResult.beat, out BeatPopupItem popupItem))
        {
            Debug.Log($"Could not find {beatResult.beat}");
            return;
        }

        popupItem.Resolve(beatResult.grade);
    }
}

