using UnityEngine;
using Timeline;
public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    public static void PauseGame(bool isMusicPaused = true)
    {
        IsPaused = true;
        Time.timeScale = 0f;
        if (isMusicPaused) MusicTimeline.instance.PauseMusic();
        if (SoundManager.Instance) SoundManager.Instance.PauseAllSFXSounds(true);
    }

    public static void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        MusicTimeline.instance.ResumeMusic();
        if (SoundManager.Instance) SoundManager.Instance.PauseAllSFXSounds(false);
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

