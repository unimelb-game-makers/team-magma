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
    public float _startTime;
    private ulong _timeSinceLastBeat;
    private int _bpm;

    public int Beat => _beat;


    private ulong _startDspTime;

    private ulong GetDspTime()
    {
        FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out ChannelGroup channelGroup);
        channelGroup.getDSPClock(out ulong dspClock, out _);
        return dspClock;
    }

    private int GetSampleRate()
    {
        FMODUnity.RuntimeManager.CoreSystem.getSoftwareFormat(out int sampleRate, out _, out _);
        return sampleRate;
    }

    private int SamplesPerBeat()
    {
        int sampleRate = GetSampleRate();
        float beatsPerSecond = 60f / _bpm;
        return sampleRate * (int)beatsPerSecond;
    }

    public BeatHandler(BeatSettings settings)
    {
        _beat = 0;
        _started = false;
        _bpm = settings.bpm;
        _beatInterval = 60f / settings.bpm;
        _thresholds = settings.thresholds;
        Array.Sort(_thresholds, (a, b) => a.tolerance.CompareTo(b.tolerance));
    }

    public void Start()
    {
        _startDspTime = GetDspTime();
        _started = true;
        _startTime = Time.time;
        _timeSinceLastBeat = _startDspTime;
    }

    public void OnBeat()
    {
        float beatTime = GetDspTime() - _timeSinceLastBeat;
        // Debug.Log($"Beat Time is {beatTime} Samples Per beat is {SamplesPerBeat()}");
        _timeSinceLastBeat = GetDspTime();
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