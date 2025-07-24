using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BeatHandler
{
    private int _beat;
    private readonly float _beatInterval;
    private bool _started;
    private float _startTime;
    private BeatSettings _settings;

    public int Beat => _beat;
    public float BeatInterval => _beatInterval;

    public BeatHandler(BeatSettings settings)
    {
        _beat = 0;
        _started = false;
        _settings = settings;
        _beatInterval = 60f / _settings.bpm;
        Array.Sort(_settings.thresholds, (a, b) => a.tolerance.CompareTo(b.tolerance));
    }

    /// <summary>
    /// I implemented a DSP version, but it was absolutely terrible.
    /// Keep this just in case
    /// </summary>
    // private ulong GetDspTime()
    // {
    //     FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out ChannelGroup channelGroup);
    //     channelGroup.getDSPClock(out ulong dspClock, out _);
    //     return dspClock;
    // }
    
    private void Start()
    {
        _started = true;
        _startTime = Time.time;
    }

    public void OnBeat()
    {
        if (_beat == 0)
            Start();
        _beat += 1;
    }

    /// <summary>
    /// Checks the existing beat pattern from settings if the beat is spawnable
    /// </summary>
    /// <param name="beat"></param>
    public bool IsBeat(int beat)
    {
        // Check empty beats first
        if (beat < _settings.initialEmptyBeats) return false;
        
        // Beat 1 maps to 0, Beat 4 maps to 3
        int beatPosition = (beat + TempoSetting.TIME_SIGNATURE - 1) % TempoSetting.TIME_SIGNATURE;
        if (beatPosition >= _settings.beatPattern.Length)
            throw new IndexOutOfRangeException($"Settings Beat Pattern is too small for Pos {beatPosition}");

        bool onBeat = _settings.beatPattern[beatPosition];
        return onBeat;
    }

    /// <summary>
    /// Gets the next possible hittable beat
    /// </summary>
    /// <returns></returns>
    private int GetNextBeat()
    {
        // I can't be bothered to do the math, sorry
        for (int i = 0; i < TempoSetting.TIME_SIGNATURE; ++i)
        {
            if (IsBeat(_beat + i))
                return _beat + i;
        }
        return -1;
    }

    public BeatResult GetBeatResult()
    {
        // Get the next possible beat in order to allow for early and late beats
        int beat = GetNextBeat();
        
        Debug.Log($"Hitting Beat {beat}");
        // Get the expected beat time and compare it against the current time
        // Do beat - 1 since beat 1 starts on 0
        float expectedBeatTime = (beat - 1) * _beatInterval;
        float currentTime = Time.time - _startTime;
        
        float timeDifference = Mathf.Abs(expectedBeatTime - currentTime);
        Debug.Log($"Expected {expectedBeatTime} Current {currentTime}. Time Difference is {timeDifference}");
        for (int i = 0; i < _settings.thresholds.Length; ++i)
        {
            if (timeDifference <= _settings.thresholds[i].tolerance)
            {
                return _settings.thresholds[i].result;
            }
        }
        return BeatResult.Failed;
    }
}