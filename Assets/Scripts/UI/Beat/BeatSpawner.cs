using UnityEngine;
using Timeline;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class BeatSpawner : MonoBehaviour
{
    // The number of beats that will be pre-emptively spawned
    private const int BEAT_SPAWN = 5;
    // The distance that a beat will travel in one second
    private const float BEAT_DISTANCE = 700f;

    [SerializeField] private RectTransform beatHolder;
    [SerializeField] private BeatPopupItem sampleBeatPopupItem;

    [SerializeField] private RectTransform leftTarget;
    [SerializeField] private RectTransform rightTarget;
    
    private BeatHandler _beatHandler;

    private Dictionary<int, BeatPopupItem> _popupItems = new();

    public void Init(BeatHandler beatHandler)
    {
        _beatHandler = beatHandler;
    }

    /// <summary>
    /// Starts the track and spawns the first initial beats, starting at 1
    /// </summary>
    public void StartTrack()
    {
        for (int i = 0; i < BEAT_SPAWN; ++i)
        {
            SpawnBeat(i + 1);
        }
    }

    /// <summary>
    /// Spawns a beat based on the time it will hit the target.
    /// </summary>
    /// <param name="beat">The beat to spawn</param>
    private void SpawnBeat(int beat)
    {
        float timeToTarget = _beatHandler.GetBeatTime(beat);
        float timeToTravel = timeToTarget - Time.time;
        float distance = BEAT_DISTANCE * timeToTravel;
        // Debug.Log($"Spawning Beat {beat}, To Target: {timeToTarget}, Time is {Time.time}");

        BeatPopupItem beatPopupItem = Instantiate(sampleBeatPopupItem, beatHolder);
        beatPopupItem.Init(leftTarget, rightTarget, distance, timeToTravel);
        _popupItems.Add(beat, beatPopupItem);
    }

    /// <summary>
    /// Resolve the current beat and create a new beat
    /// </summary>
    /// <param name="beat"></param>
    public void OnBeat(int beat)
    {
        // Resolve the last beat and release it
        if (_popupItems.TryGetValue(beat, out BeatPopupItem popupItem))
        {
            popupItem.Resolve();
            _popupItems.Remove(beat);
        }
        
        SpawnBeat(beat + BEAT_SPAWN);
    }
}

