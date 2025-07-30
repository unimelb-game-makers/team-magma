using System;
using System.Collections.Generic;
using UnityEngine;

public class BeatHandler
{
    private int _beat;
    private float _currentBeat;
    private readonly float _beatInterval;
    private bool _started;
    private BeatSettings _settings;

    public int Beat => _beat;

    // TODO: Use this in the level end screen possibly for stats?
    private readonly Dictionary<int, Grade> _processedBeats = new Dictionary<int, Grade>();

    private TempoMode _currentMode = TempoMode.Default;
    public float CurrentBeat => _currentBeat;
    public float BeatInterval => _beatInterval / TempoSetting.GetRatio(_currentMode);

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
    }

    public void OnBeat()
    {
        if (_beat == 0)
            Start();
        _beat += 1;
        
        // By resetting our current beat tracker to the beat of the track, we minimise the offset created
        // by floating point imprecision and weird delta time silliness on Unity's end.
        _currentBeat = _beat;
    }

    public void OnTempoChanged(TempoMode mode)
    {
        _currentMode = mode;
    }

    public void Update(float deltaTime)
    {
        if (!_started) return;

        _currentBeat += BeatInterval * deltaTime;
    }

    /// <summary>
    /// Checks the existing beat pattern from settings if the beat is spawnable
    /// </summary>
    /// <param name="beat"></param>
    public bool IsBeat(int beat)
    {
        // Check empty beats first
        if (beat < _settings.initialEmptyBeats) return false;
        
        // Check if we have processed it already
        if (_processedBeats.ContainsKey(beat)) return false;
        
        // Beat 1 maps to 0, Beat 4 maps to 3
        int beatPosition = (beat + TempoSetting.TIME_SIGNATURE - 1) % TempoSetting.TIME_SIGNATURE;
        if (beatPosition >= _settings.beatPattern.Length)
            throw new IndexOutOfRangeException($"Settings Beat Pattern is too small for Pos {beatPosition}");

        bool onBeat = _settings.beatPattern[beatPosition];
        return onBeat;
    }

    /// <summary>
    /// Gets the closest legal beat to the current beat
    /// </summary>
    /// <returns></returns>
    private int GetNearestBeat()
    {
        int nearestBeat = -1;
        int minDistance = int.MaxValue;

        for (int i = 0; i < TempoSetting.TIME_SIGNATURE; ++i)
        {
            int earlier = _beat - i;
            if (IsBeat(earlier) && i < minDistance)
            {
                nearestBeat = earlier;
                minDistance = i;
            }

            int later = _beat + i;
            if (IsBeat(later) && i < minDistance)
            {
                nearestBeat = later;
                minDistance = i;
            }

            if (minDistance == 0)
                break;
        }

        return nearestBeat;
    }

    public BeatResult GetBeatResult()
    {
        if (!_started) return new BeatResult(0, Grade.Failed);
        
        // Get the next possible beat in order to allow for early and late beats
        int beat = GetNearestBeat();
        if (beat == -1) return new BeatResult(beat, Grade.Failed);

        // The key is that we calculate the difference in the beats, as opposed to elapsed time!
        float beatDifference = Mathf.Abs(beat - _currentBeat);
        
        for (int i = 0; i < _settings.thresholds.Length; ++i)
        {
            if (beatDifference <= _settings.thresholds[i].tolerance)
            {
                return new BeatResult(beat, _settings.thresholds[i].result);
            }
        }

        return new BeatResult(beat, Grade.Failed);
    }

    public void ProcessBeat(BeatResult beatResult)
    {
        if (!_processedBeats.ContainsKey(beatResult.beat))
            _processedBeats.Add(beatResult.beat, beatResult.grade);
    }
}