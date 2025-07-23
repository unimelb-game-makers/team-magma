public enum TempoMode
{
    Slow,
    Default,
    Fast
}

public static class TempoSetting
{
    public const float SlowRatio    = 0.875f; // ~140 BPM
    public const float DefaultRatio = 1.0f;   // 160 BPM
    public const float FastRatio    = 1.125f; // ~180 BPM

    /// <summary>
    /// Gets the playback speed ratio for the specified TempoMode.
    /// </summary>
    public static float GetRatio(TempoMode mode)
    {
        return mode switch
        {
            TempoMode.Slow    => SlowRatio,
            TempoMode.Fast    => FastRatio,
            _                 => DefaultRatio
        };
    }
}

