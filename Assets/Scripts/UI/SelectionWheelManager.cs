using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SelectionWheelManager : MonoBehaviour
{
    public GameObject selectionWheel; // The UI wheel to display
    public string inputName = "Tape"; // Input name as defined in the Input Manager
    private bool isWheelActive = false; // Track if the wheel is active
    public PauseObjectController pauseObjectController;
    public BatteryManager batteryManager;
    public float batteryNeeded = 50;

    void Update()
    {
        if (PauseManager.IsPaused) 
        {
            if (isWheelActive)
            {
                ToggleWheel();
            }
            return;
        }
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetButtonDown(inputName)) // Check for input using the name 'Tape'
        {
            ToggleWheel();
        }
    }

    public void ToggleWheel()
    {
        if (!isWheelActive) 
        {
            Time.timeScale = 0;
            pauseObjectController.DisableObjects();
        }
        else 
        {
            Time.timeScale = 1;
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

