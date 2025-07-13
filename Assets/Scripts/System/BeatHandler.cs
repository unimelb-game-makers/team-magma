using System;
using UnityEngine;

public class BeatHandler
{
    private float _timer;
    private float _beatInterval;
    private BeatThreshold[] _thresholds;
    private bool _started;

    public BeatHandler(BeatSettings settings)
    {
        _started = false;
        _beatInterval = 60f / settings.bpm;
        _thresholds = settings.thresholds;
        Array.Sort(_thresholds, (a, b) => a.tolerance.CompareTo(b.tolerance));
    }

    public void Start()
    {
        _started = true;
    }

    public void Update(float deltaTime)
    {
        _timer += deltaTime;
    }

    public BeatResult GetBeatResult()
    {
        float timeToNearestBeat = Mathf.Abs(Mathf.Round(_timer / _beatInterval) * _beatInterval - _timer);
        Debug.Log($"Time to Nearest Beat is {timeToNearestBeat}");
        for (int i = 0; i < _thresholds.Length; ++i)
        {
            if (timeToNearestBeat <= _thresholds[i].tolerance)
            {
                return _thresholds[i].result;
            }
        }
        return BeatResult.Failed;
    }
}