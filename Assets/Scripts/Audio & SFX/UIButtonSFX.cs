using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // Required for the pointer events

public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private EventReference hoverEventReference;  // FMOD event reference for hover sound
    [SerializeField] private EventReference clickEventReference;  // FMOD event reference for click sound
    private EventInstance hoverEventInstance;  // FMOD EventInstance for hover sound
    private EventInstance clickEventInstance;  // FMOD EventInstance for click sound

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the EventInstances using the EventReferences
        hoverEventInstance = RuntimeManager.CreateInstance(hoverEventReference);
        clickEventInstance = RuntimeManager.CreateInstance(clickEventReference);
    }

    // Set the initial volume based on PlayerPrefs or default value
    private void SetVolumeBasedOnSetting()
    {
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);  // Default to 1 (100%) if not set
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);  // Default to 1 (100%) if not set
        SetSFXVolume(sfxVolume * masterVolume);
    }

    // Set the SFX volume for both hover and click event instances
    private void SetSFXVolume(float value)
    {
        // Apply the volume to the hover and click event instances
        hoverEventInstance.setVolume(value);  // Set volume for hover event
        clickEventInstance.setVolume(value);  // Set volume for click event
    }

    // Trigger hover sound when the mouse enters the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetVolumeBasedOnSetting();
        hoverEventInstance.start();  // Start the hover sound
    }

    // Stop the hover sound when the mouse exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        hoverEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);  // Stop the hover sound immediately
    }

    // Trigger click sound when the button is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        SetVolumeBasedOnSetting();
        clickEventInstance.start();  // Start the click sound
    }

    // Clean up the EventInstances when the script is destroyed (optional but good practice)
    private void OnDestroy()
    {
        hoverEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); // Stop event with fade-out
        hoverEventInstance.release();  // Release the hover event instance
        clickEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); // Stop event with fade-out
        clickEventInstance.release();  // Release the click event instance
    }
}

