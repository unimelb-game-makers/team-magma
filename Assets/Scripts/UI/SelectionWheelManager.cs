using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Platforms;
using Tempo;
using Utilities.ServiceLocator;
using Timeline;
using UnityEngine.UI;
using DG.Tweening;

public class SelectionWheelManager : MonoBehaviour
{

    public GameObject selectionWheel; // The UI wheel to display
    public GameObject selectionWheelPanel; // The UI wheel to display
    public string inputName = "Tape"; // Input name as defined in the Input Manager
    private bool isWheelActive = false; // Track if the wheel is active
    public PauseObjectController pauseObjectController;
    public BatteryManager batteryManager;
    public PauseMenuController pauseMenuController;
    public float batteryNeeded = 50;
    public float fadeDuration = 0.2f;
    [SerializeField] private AudioPlayerTest TapeEffectSoundPlayer;
    private GameObject musicManager;


    void Awake() {
        selectionWheelPanel.transform.localScale = Vector3.zero;

        musicManager = GameObject.FindGameObjectWithTag("RhythmManager");
    }    
    
    void Update()
    {
        if (PauseManager.IsPaused && pauseMenuController.isPauseMenu) 
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
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetButtonDown(inputName))
        {
            ToggleWheel();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            ToggleWheel();
        }

        if (isWheelActive)
        {
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
    }

    public void UseTapeDefault() {
        MusicTimeline timelineComponent = musicManager.GetComponent<MusicTimeline>();
        timelineComponent.SetIntensity(2);
        UseTape();
        PlayTapeEffect(TapeType.Slow, 0.01f, 0.5f);
    }

    public void UseTapeSlow() {
        MusicTimeline timelineComponent = musicManager.GetComponent<MusicTimeline>();
        timelineComponent.SetIntensity(1);
        
        TapeEffectSoundPlayer.Play();
        // Play Slow Tape Effect
        //get IAffectServices from service locator
        var affectServices = ServiceLocator.Instance.Get<ISyncable>();
        foreach (var o in affectServices)
        {
            o.Affect(TapeType.Slow, 5, 0.5f); // Why is TapeType in Platforms namespace?
        }
    
        UseTape();
    }

    public void UseTapeFast() {

        MusicTimeline timelineComponent = musicManager.GetComponent<MusicTimeline>();
        timelineComponent.SetIntensity(3);
        UseTape();
        PlayTapeEffect(TapeType.Fast, 5, 0.5f);
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
        if (!isWheelActive)
        {
            // Pause game and disable objects when the wheel is shown
            PauseManager.PauseGame();
            pauseObjectController.DisableObjects();

            // Fade in the wheel panel and scale it back to normal size
            StartCoroutine(ShowWheel());
        }
        else
        {
            // Resume game and enable objects when the wheel is hidden
            PauseManager.ResumeGame();
            pauseObjectController.EnableObjects();

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
        if (batteryManager.UseBattery(batteryNeeded))
        {
            ToggleWheel();
        }
    }
}

