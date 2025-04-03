using UnityEngine;
using FMODUnity; // Required for FMOD EventReference
using Managers.AudioManager;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    private EventReference eventReference; // FMOD Event Reference

    private IAudioEventPlayer audioPlayer; // Audio event player interface
    private IAudioEventReference reference; // Audio event reference

    private FMOD.Studio.EventInstance eventInstance; // Event instance declaration

    void Start()
    {
        // Get the AudioEventPlayer from the scene
        audioPlayer = FindObjectOfType<AudioEventPlayer>();

        if (audioPlayer == null)
        {
            Debug.LogError("AudioPlayerTest: No AudioEventPlayer found in the scene.");
            return;
        }

        // Create the audio event reference from the FMOD EventReference
        reference = new FMODAudioEventReference(eventReference);

        // Create the FMOD event instance from the EventReference
        eventInstance = FMODUnity.RuntimeManager.CreateInstance(eventReference);
    }

    public void Play()
    {
        if (audioPlayer == null || reference == null)
        {
            Debug.LogError("AudioPlayerTest: Missing audioPlayer or reference.");
            return;
        }

        int instanceId = audioPlayer.PlayEventInstance(reference, null, releaseOnFinish: true);
    }

    public void SetVolume(float volume)
    {
        // Ensure volume is between 0.0 (mute) and 1.0 (full volume)
        if (eventInstance.isValid())
        {
            eventInstance.setVolume(Mathf.Clamp(volume, 0.0f, 1.0f)); // Set volume
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

