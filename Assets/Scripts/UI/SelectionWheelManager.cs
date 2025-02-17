using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Platforms;
using Tempo;
using Utilities.ServiceLocator;
using Timeline;
using UnityEngine.UI;

public class SelectionWheelManager : MonoBehaviour
{

    public GameObject selectionWheel; // The UI wheel to display
    public string inputName = "Tape"; // Input name as defined in the Input Manager
    private bool isWheelActive = false; // Track if the wheel is active
    public PauseObjectController pauseObjectController;
    public BatteryManager batteryManager;
    public PauseMenuController pauseMenuController;
    public float batteryNeeded = 50;

    [SerializeField] private AudioPlayerTest TapeEffectSoundPlayer;
    private GameObject musicManager;


    void Awake() {
        musicManager = GameObject.FindGameObjectWithTag("RhythmManager");
    }    
    
    void Update()
    {
        if (PauseManager.IsPaused && pauseMenuController.isPauseMenu) 
        {
            if (isWheelActive)
            {
                isWheelActive = !isWheelActive;

                selectionWheel.SetActive(isWheelActive);

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
            PauseManager.PauseGame();
            pauseObjectController.DisableObjects();
        }
        else 
        {
            PauseManager.ResumeGame();
            pauseObjectController.EnableObjects();
        
        }
        
        isWheelActive = !isWheelActive;

        selectionWheel.SetActive(isWheelActive);

        // Blur background
        PostProcessVolume ppVolume = Camera.main.gameObject.GetComponent<PostProcessVolume>();
        ppVolume.enabled = !ppVolume.enabled;
    }

    public void UseTape()
    {
        if (batteryManager.UseBattery(batteryNeeded))
        {
            ToggleWheel();
        }
    }


}

