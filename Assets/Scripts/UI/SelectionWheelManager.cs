using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Tempo;
using Utilities.ServiceLocator;
using Timeline;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class SelectionWheelManager : MonoBehaviour
{

    [SerializeField] private GameObject selectionWheel; // The UI wheel to display
    [SerializeField] private GameObject selectionWheelPanel; // The UI wheel to display
    [SerializeField] private string inputName = "Tape"; // Input name as defined in the Input Manager
    [SerializeField] private BatteryManager batteryManager;
    [SerializeField] private float batteryNeeded = 50;
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private AudioPlayer TapeEffectSoundPlayer;
    [SerializeField] private List<Button> tapeButtons;
    [SerializeField] private float scrollSpeed = 1f;  // Set the scroll speed
    [SerializeField] private int tapeEffectDuration = 6;  // Set the effect duration of the tape
    private bool isWheelActive = false; // Track if the wheel is active
    private bool isScrolled = false;
    private int currentIndex = 0; // Index to track the current selection

    private static bool isRightClickBlocked = false;    // For tutorial UI
    public static void BlockRightClick(bool block)
    {
        isRightClickBlocked = block;
    }

    void Awake() {
        selectionWheelPanel.transform.localScale = Vector3.zero;
    }    
    
    void Update()
    {
        if (PauseManager.IsPaused && (PauseMenuController.Instance.isPauseMenu || StartMenuManager.Instance.isStartMenu))
        {
            if (isWheelActive)
            {
                isWheelActive = !isWheelActive;

                selectionWheelPanel.transform.localScale = Vector3.zero;

                // Blur background
                PostProcessVolume ppVolume = Camera.main.gameObject.GetComponent<PostProcessVolume>();
                ppVolume.enabled = !ppVolume.enabled;
            }
            return;
        }

        if (isWheelActive)
        {
            // Get the mouse scroll wheel input (positive for up, negative for down)
            float scroll = Input.mouseScrollDelta.y;

            // If scroll input is detected, update the selection
            if (scroll != 0)
            {
                isScrolled = true;

                // Call OnPointerExit on the previously selected tape (if any)
                if (currentIndex >= 0 && currentIndex < tapeButtons.Count)
                {
                    TapeSelectionHoverEffect previousHoverScript = tapeButtons[currentIndex].GetComponent<TapeSelectionHoverEffect>();
                    if (previousHoverScript != null)
                    {
                        previousHoverScript.OnPointerExit(null);
                    }
                }

                // Update the index based on the scroll direction
                currentIndex += (int)(scroll * scrollSpeed);

                // Loop the index to ensure it stays within bounds of the tapes array
                if (currentIndex >= tapeButtons.Count)
                {
                    currentIndex = 0; // Loop back to the first item
                }
                else if (currentIndex < 0)
                {
                    currentIndex = tapeButtons.Count - 1; // Loop to the last item
                }

                // Update the UI display with the new selection
                TapeSelectionHoverEffect hoverScript = tapeButtons[currentIndex].GetComponent<TapeSelectionHoverEffect>();

                if (hoverScript != null)
                {
                    // Call OnPointerEnter on the newly selected tape
                    hoverScript.OnPointerEnter(null);
                }
            }
        }
        if (!PauseMenuController.Instance.isPauseMenu && !StartMenuManager.Instance.isStartMenu)
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        // if in tutorial ui screen, cannot open wheel selection
        if (isRightClickBlocked) return;

        if (Input.GetButtonDown(inputName) || Input.GetButtonDown("Fire2"))
        {
            if (isWheelActive && isScrolled) {
                switch (currentIndex) 
                {
                    case 0: // Slow Tape
                        UseTapeSlow();
                        return;
                    case 1: // Default Tape
                        UseTapeDefault();
                        return;
                    case 2: // Fast Tape
                        UseTapeFast();
                        return;
                    default:
                        Debug.LogWarning("Invalid tape type: " + currentIndex);
                        return;
                }
            }
            else
            {
                ToggleWheel();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseTapeSlow();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseTapeDefault();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseTapeFast();
        }
    }

    public void UseTapeDefault() {
        if (batteryManager.UseBattery(batteryNeeded))
        {
            MusicTimeline.instance.SetIntensity(2);
            TapeEffectSoundPlayer.Play();
            PlayTapeEffect(TapeType.Slow, 0.01f, 0.5f);
            TapeNotificationManager.Instance.FadeOutUI();

            if (isWheelActive) {
                ToggleWheel();
            }
        }
    }

    public void UseTapeSlow() {
        if (batteryManager.UseBattery(batteryNeeded))
        {
            MusicTimeline.instance.SetIntensity(1);
            TapeEffectSoundPlayer.Play();
            PlayTapeEffect(TapeType.Slow, tapeEffectDuration, 0.5f);
            TapeNotificationManager.Instance.ActivateTapeUI(TapeType.Slow, tapeEffectDuration);
        
            if (isWheelActive) {
                ToggleWheel();
            }
        }
    }

    public void UseTapeFast() {
        if (batteryManager.UseBattery(batteryNeeded))
        {
            MusicTimeline.instance.SetIntensity(3);
            TapeEffectSoundPlayer.Play();
            PlayTapeEffect(TapeType.Fast, tapeEffectDuration, 0.5f);
            TapeNotificationManager.Instance.ActivateTapeUI(TapeType.Fast, tapeEffectDuration);

            if (isWheelActive) {
                ToggleWheel();
            }
        } 
    }

    public void PlayTapeEffect(TapeType Type, float duration, float effectValue)
    {
        //get IAffectServices from service locator
        var affectServices = ServiceLocator.Instance.Get<ISyncable>();
        foreach (var o in affectServices)
        {
            o.Affect(Type, duration, effectValue);
        }
    }

    public void ToggleWheel()
    {
        isScrolled = false;
        if (!isWheelActive)
        {
            // Pause game and disable objects when the wheel is shown
            PauseManager.PauseGame();

            // Fade in the wheel panel and scale it back to normal size
            StartCoroutine(ShowWheel());
        }
        else
        {
            // Resume game and enable objects when the wheel is hidden
            PauseManager.ResumeGame();

            // Fade out the wheel panel and scale it down to 0
            StartCoroutine(HideWheel());
        }

        // Toggle the wheel visibility
        isWheelActive = !isWheelActive;

        // Blur background when the wheel is active/inactive
        PostProcessVolume ppVolume = Camera.main.gameObject.GetComponent<PostProcessVolume>();
        ppVolume.enabled = !ppVolume.enabled;
    }

    private System.Collections.IEnumerator ShowWheel()
    {
        // First, set the scale to 0 and fade in
        selectionWheelPanel.transform.localScale = Vector3.zero;

        // Animate scale back to normal size (for example, (1,1,1))
        selectionWheelPanel.transform.DOScale(Vector3.one, fadeDuration).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Normal, true)  // Use unscaledDeltaTime
            .WaitForCompletion();

        yield return null;
    }

    private System.Collections.IEnumerator HideWheel()
    {
        // Animate scale down to 0
        selectionWheelPanel.transform.DOScale(Vector3.zero, fadeDuration).SetEase(Ease.InOutQuad).SetUpdate(UpdateType.Normal, true)  // Use unscaledDeltaTime
            .WaitForCompletion();

        yield return null;
    }

    public void UseTape()
    {
        if (batteryManager.UseBattery(batteryNeeded) && isWheelActive)
        {
            UseTape();
            ToggleWheel();
        }
    }
}

