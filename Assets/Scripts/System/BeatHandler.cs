using System;
using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BeatHandler
{
    private int _beat;
    private float _timer;
    private float _beatInterval;
    private BeatThreshold[] _thresholds;
    private bool _started;
    private float _startTime;

    public int Beat => _beat;

    public BeatHandler(BeatSettings settings)
    {
        _beat = 0;
        _started = false;
        _beatInterval = 60f / settings.bpm;
        _thresholds = settings.thresholds;
        Array.Sort(_thresholds, (a, b) => a.tolerance.CompareTo(b.tolerance));
    }

    public void Start()
    {
        _started = true;
        _startTime = Time.time;
    }

    public void OnBeat()
    {
        _beat += 1;
    }

    public void Update(float deltaTime)
    {
        _timer += deltaTime;
    }

    public float GetBeatTime(float targetBeat)
    {
        float beatTime = targetBeat * _beatInterval;
        return beatTime + _startTime;
    }

    public BeatResult GetBeatResult()
    {
        float timeToNearestBeat = Mathf.Abs(Mathf.Round(_timer / _beatInterval) * _beatInterval - _timer);
        // Debug.Log($"Time to Nearest Beat is {timeToNearestBeat}");
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