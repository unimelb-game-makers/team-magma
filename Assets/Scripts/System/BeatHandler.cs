using System;
using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BeatHandler
{
    private int _beat;
    private float _timer;
    private float _beatInterval;
    private bool _started;
    private float _startTime;
    private BeatSettings _settings;

    public int Beat => _beat;

    public BeatHandler(BeatSettings settings)
    {
        _beat = 0;
        _started = false;
        _beatInterval = 60f / settings.bpm;
        _settings = settings;
        Array.Sort(_settings.thresholds, (a, b) => a.tolerance.CompareTo(b.tolerance));
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

    /// <summary>
    /// Checks the existing beat pattern from settings if the beat is spawnable
    /// </summary>
    /// <param name="beat"></param>
    public bool CanSpawn(int beat)
    {
        // Beat 1 maps to 0, Beat 4 maps to 3
        int beatPosition = (beat + TempoSetting.TIME_SIGNATURE - 1) % TempoSetting.TIME_SIGNATURE;
        if (beatPosition >= _settings.beatPattern.Length)
            throw new IndexOutOfRangeException($"Settings Beat Pattern is too small for Pos {beatPosition}");

        bool onBeat = _settings.beatPattern[beatPosition];
        Debug.Log($"Can Spawn {beat} is {onBeat}. Beat Pos is {beatPosition}");
        return onBeat;
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
        Debug.Log($"Time to Nearest Beat is {timeToNearestBeat}");
        for (int i = 0; i < _settings.thresholds.Length; ++i)
        {
            if (timeToNearestBeat <= _settings.thresholds[i].tolerance)
            {
                return _settings.thresholds[i].result;
            }
        }
        return BeatResult.Failed;
    }
}