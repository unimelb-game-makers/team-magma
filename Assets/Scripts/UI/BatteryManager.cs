using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class BatteryManager : MonoBehaviour
{
    public Image batteryHexagon; // The single hexagon UI element
    public Image batteryHexagonBg;
    public float maxBattery = 100f; // Maximum battery level
    public float recoveryRate = 10f; // Battery recovery rate per second
    private float currentBattery; // Current battery level
    private Tween flashTween;
    private Color originalColor;


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
            currentBattery -= amount;
            UpdateBatteryUI();
            return true;
        }

        FlashBatteryWarning(); // Notify player
        return false;
    }

    private void FlashBatteryWarning()
    {
        if (batteryHexagonBg == null) return;

        if (originalColor == default)
            originalColor = batteryHexagonBg.color;

        if (flashTween != null && flashTween.IsActive())
        {
            flashTween.Kill(); // Cancel any ongoing tween
        }

        flashTween = DOTween.Sequence()
            .SetUpdate(true) // Use unscaled time
            .Append(batteryHexagonBg.DOColor(Color.red, 0.1f))
            .AppendInterval(0.1f)
            .Append(batteryHexagonBg.DOColor(originalColor, 0.1f))
            .AppendInterval(0.1f)
            .Append(batteryHexagonBg.DOColor(Color.red, 0.1f))
            .AppendInterval(0.1f)
            .Append(batteryHexagonBg.DOColor(originalColor, 0.1f));
    }
}
