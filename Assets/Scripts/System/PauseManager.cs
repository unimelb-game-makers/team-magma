using UnityEngine;
using Timeline;
public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    public static void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        MusicTimeline.instance.PauseMusic();
    }

    public static void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        MusicTimeline.instance.ResumeMusic();
    }

    public static void TogglePause()
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
}

