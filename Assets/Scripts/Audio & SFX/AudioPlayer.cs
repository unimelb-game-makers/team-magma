using UnityEngine;
using FMODUnity; // Required for FMOD EventReference

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private EventReference eventReference; // FMOD Event Reference
    private FMOD.Studio.EventInstance eventInstance; // Event instance declaration

    void Start()
    {
        // Create the FMOD event instance from the EventReference
        eventInstance = FMODUnity.RuntimeManager.CreateInstance(eventReference);
    }

    public void Play()
    {
        SetVolumeBasedOnSetting();
        eventInstance.start();
    }

    // Set the initial volume based on PlayerPrefs or default value
    private void SetVolumeBasedOnSetting()
    {
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);  // Default to 1 (100%) if not set
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);  // Default to 1 (100%) if not set

        if (eventInstance.isValid())
        {
            eventInstance.setVolume(sfxVolume * masterVolume); // Set volume
        }

        if (gameObject.name == "SettingsPanel") {
            SoundManager.Instance.SetGameObjectsSFXVolume();
        }
    }

    void OnDestroy()
    {
        // Stop and release the event when the object is destroyed
        if (eventInstance.isValid())
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); // Stop event with fade-out
            eventInstance.release(); // Release the event instance
        }
    }
}

