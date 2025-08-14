using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TapeColors", menuName = "ScriptableObjects/TapeColors")]
public class TapeColors : ScriptableObject
{
    [SerializeField] Color FastTapeColor;
    [SerializeField] Color DefaultTapeColor;
    [SerializeField] Color SlowTapeColor;

    public Color GetColor(TempoMode mode)
    {
        switch (mode)
        {
            case TempoMode.Slow:
                return SlowTapeColor;
            case TempoMode.Fast:
                return FastTapeColor;
            case TempoMode.Default:
                return DefaultTapeColor;
        }
    return DefaultTapeColor;
    }
}