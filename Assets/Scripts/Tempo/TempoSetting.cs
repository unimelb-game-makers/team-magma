using System.Collections.Generic;

public enum TempoMode
{
    Slow,
    Default,
    Fast
}

public class GameSetting
{
    public float BpmMultiplier { get; set; }
    public float PlayerMovementMultiplier { get; set; }
    public float EnemyMovementMultiplier { get; set; }
}

public static class TempoSetting
{
    public const int TIME_SIGNATURE = 4;

    private static readonly Dictionary<TempoMode, GameSetting> Tempos = new()
    {
        // 140 BPM
        { TempoMode.Slow, new GameSetting { BpmMultiplier = 0.875f, PlayerMovementMultiplier = 1f, EnemyMovementMultiplier = 0.5f } },
        // 160 BPM
        { TempoMode.Default, new GameSetting { BpmMultiplier = 1f, PlayerMovementMultiplier = 1f, EnemyMovementMultiplier = 1f } },
        // 180 BPM
        { TempoMode.Fast, new GameSetting { BpmMultiplier = 1.125f, PlayerMovementMultiplier = 1.5f, EnemyMovementMultiplier = 1f } },
    };
    

    /// <summary>
    /// Gets the playback speed ratio for the specified TempoMode.
    /// </summary>
    public static float GetRatio(TempoMode mode)
    {
        return Tempos[mode].BpmMultiplier;
    }

    public static float GetPlayerMovementMultiplier(TempoMode mode)
    {
        return Tempos[mode].PlayerMovementMultiplier;
    }

    public static float GetEnemyMovementMultiplier(TempoMode mode)
    {
        return Tempos[mode].EnemyMovementMultiplier;
    }
}

