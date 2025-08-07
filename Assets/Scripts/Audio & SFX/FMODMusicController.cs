using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODMusicController : IMusicController
{
    private EventInstance musicInstance;
    private EventReference eventReference;
    private float _volume = 1.0f;

    public FMODMusicController(EventReference reference)
    {
        eventReference = reference;
    }

    public void PlayMusic()
    {
        if (!musicInstance.isValid())
        {
            musicInstance = RuntimeManager.CreateInstance(eventReference);
            musicInstance.setVolume(_volume);
            musicInstance.start();
        }
    }

    public void StopMusic()
    {
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
        }
    }

    public void SetMusicVolume(float volume)
    {
        _volume = Mathf.Clamp01(volume);
        if (musicInstance.isValid())
        {
            musicInstance.setVolume(_volume);
        }
    }
}
