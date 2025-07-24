using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

[Serializable]
public enum BeatResult
{
    Failed,
    Good,
    Perfect,
}

[Serializable]
public class BeatThreshold
{
    public BeatResult result;
    public float tolerance;
}

[CreateAssetMenu(menuName = "ScriptableObjects/Beat Settings", fileName = "Beat Settings")]
public class BeatSettings : ScriptableObject
{
    public FMODUnity.EventReference track;
    public int bpm;
    public BeatThreshold[] thresholds;
    
    // The DSP Interval Time that passes per beat
    // I found this to be 18,432 even though the beats should really be on 18,000
    public ulong dspInterval = 18432;

    public int initialEmptyBeats = 8;
    public bool[] beatPattern = new bool[4];
}