using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TargetHexagon : MonoBehaviour
{
    public Color highlightColor = Color.yellow;
    public Color failColor = Color.red;
    private Image image;  // Reference to the Image component
    private Color originalColor;  // To store the original color

    void Start()
    {
        image = GetComponent<Image>();  // Get the Image component attached to this GameObject
        originalColor = image.color;  // Store the original color of the hexagon
    }

    // Method to change the hexagon's color temporarily
    public void ChangeColorTemporary(bool onBeat)
    {
        if (onBeat) {
            Debug.Log("Hit on Beat!");
            StartCoroutine(ChangeColorCoroutine(highlightColor));
            Debug.Log(highlightColor);
        } else {
            StartCoroutine(ChangeColorCoroutine(failColor));
        }
    }

    // Coroutine to handle color change
    private IEnumerator ChangeColorCoroutine(Color color)
    {
        // Change color to color
        image.color = color;

        // Wait for a short duration (adjust as needed)
        yield return new WaitForSeconds(0.2f);

        // Revert to the original color
        image.color = originalColor;
    }
}
