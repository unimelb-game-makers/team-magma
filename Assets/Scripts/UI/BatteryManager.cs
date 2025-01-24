using UnityEngine;
using UnityEngine.UI;

public class BatteryManager : MonoBehaviour
{
    public Image batteryHexagon; // The single hexagon UI element
    public float maxBattery = 100f; // Maximum battery level
    public float recoveryRate = 10f; // Battery recovery rate per second
    private float currentBattery; // Current battery level

    void Start()
    {
        currentBattery = maxBattery; // Start with a full battery
        UpdateBatteryUI();
    }

    void Update()
    {
        RecoverBattery();
    }

    private void RecoverBattery()
    {
        if (currentBattery < maxBattery)
        {
            currentBattery += recoveryRate * Time.deltaTime; // Recover battery over time
            currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery); // Ensure battery doesn't exceed max
            UpdateBatteryUI();
        }
    }

    private void UpdateBatteryUI()
    {
        if (batteryHexagon != null)
        {
            batteryHexagon.fillAmount = currentBattery / maxBattery; // Update the fill based on battery level
        }
    }

    public bool UseBattery(float amount)
    {
        if (currentBattery >= amount)
        {
            currentBattery -= amount; // Deduct battery amount
            UpdateBatteryUI();
            return true; // Battery usage successful
        }

        return false; // Not enough battery
    }
}
