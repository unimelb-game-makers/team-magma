using UnityEngine;
using System.Collections.Generic;

public class BeatSpawner : MonoBehaviour
{
    private const int BEAT_PREFIX = 4;
    // The distance that a beat will travel in one second
    private const float BEAT_DISTANCE = 700f;

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
        
        // Debug.Log($"Spawning {beat}");
        
        // The time to target is the amount of beats we spawned ahead of time
        float travelTime = _beatHandler.BeatInterval * BEAT_PREFIX;
        float distance = BEAT_DISTANCE * travelTime;
        // Debug.Log($"Spawning Beat {beat}, To Target: {timeToTarget}, Time is {Time.time}");

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
}

