using UnityEngine;
using UnityEngine.UI;

public class Beat : MonoBehaviour
{
    private RectTransform[] hexagons; // Hexagon positions
    private int currentHexagonIndex = 0; // Starting at the first hexagon
    private float stepInterval; // Time interval between steps
    private float timer = 0f; // Timer to track time for movement
    private RectTransform beatRect; // Reference to this beat's RectTransform
    private bool isAtFinalHexagon = false; // Flag to check if beat is at the final hexagon
    private Image beatImage; // Reference to the UI Image component

    public Color highlightColor = Color.yellow; // Color to highlight the beat

    public void Initialize(HexagonManager hexagonManager, float stepInterval)
    {
        this.hexagons = hexagonManager.GetHexagons(); // Retrieve hexagon positions from the manager
        this.stepInterval = stepInterval; // Assign the step interval

        // Set the initial position of the beat
        if (hexagons.Length > 0)
        {
            beatRect = GetComponent<RectTransform>();
            beatRect.position = hexagons[0].position; // Start at the first hexagon
        }
        else
        {
            Debug.LogError("No hexagon positions provided by HexagonManager!");
        }

        // Get the Image component for highlighting
        beatImage = GetComponent<Image>();
        if (beatImage == null)
        {
            Debug.LogError("No Image component found on the Beat GameObject!");
        }
    }

    void Update()
    {
        if (hexagons == null || currentHexagonIndex >= hexagons.Length)
        {
            return; // Stop if we've reached or surpassed the end
        }

        timer += Time.deltaTime;

        // Move the beat to the next hexagon at intervals
        if (timer >= stepInterval)
        {
            timer -= stepInterval; // Reset timer

            if (isAtFinalHexagon)
            {
                // If already at the final hexagon, destroy the beat
                Destroy(gameObject);
                return;
            }

            currentHexagonIndex++; // Move to the next hexagon

            // Update beat's position
            beatRect.position = hexagons[currentHexagonIndex].position;

            // Check if the beat reaches the final hexagon
            if (currentHexagonIndex == hexagons.Length - 1)
            {
                OnBeatReachedFinalHexagon();
            }
        }
    }

    private void OnBeatReachedFinalHexagon()
    {
        Debug.Log("Beat reached the final position!");
        isAtFinalHexagon = true;

        // Highlight the beat
        if (beatImage != null)
        {
            beatImage.color = highlightColor; // Change the color to highlight
        }
    }
}
