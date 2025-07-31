using System;
using UnityEngine;

[Serializable]
public enum Grade
{
    Failed,
    Good,
    Perfect,
}

public struct BeatResult
{
    public int beat;
    public Grade grade;

    public BeatResult(int beat, Grade grade)
    {
        this.beat = beat;
        this.grade = grade;
    }
}

[Serializable]
public class BeatThreshold
{
    public Grade result;
    public float tolerance;
}

[CreateAssetMenu(menuName = "ScriptableObjects/Beat Settings", fileName = "Beat Settings")]
public class BeatSettings : ScriptableObject
{
    public int bpm;
    public BeatThreshold[] thresholds;

    public int initialEmptyBeats = 8;
    public bool[] beatPattern = new bool[4];
}