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

    public int initialEmptyBeats = 8;
    public bool[] beatPattern = new bool[4];
}