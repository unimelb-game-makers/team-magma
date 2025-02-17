
using System.Collections.Generic;
using UnityEngine;
using Managers.AudioManager;


public class AudioPlayerTest : MonoBehaviour
{
    [SerializeField]
    private FMODUnity.EventReference myEventReference;

    private IAudioEventPlayer audioPlayer;
    private IAudioEventReference reference;

    void Start()
    {
        // Get the player from the AudioManager GameObject
        audioPlayer = FindObjectOfType<AudioEventPlayer>();

        // Create a reference
        IAudioEventReference reference = new FMODAudioEventReference(myEventReference);

        // Play a one-shot event with some parameters
        // var parameters = new List<IAudioParameter>
        // {
        //     new FMODAudioParameter("Intensity", 1.0f),
        //     new FMODAudioParameter("Pitch", 0.8f)
        // };

        int instanceId = audioPlayer.PlayEventInstance(reference, null, releaseOnFinish: true);
    }

    public void Play() 
    {
        int instanceId = audioPlayer.PlayEventInstance(reference, null, releaseOnFinish: true);
    }
}