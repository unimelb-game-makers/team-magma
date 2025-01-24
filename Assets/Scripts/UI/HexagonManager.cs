using UnityEngine;

public class HexagonManager : MonoBehaviour
{
    public RectTransform[] hexagons; // Store references to all hexagon UI positions

    private void Awake()
    {
        if (hexagons == null || hexagons.Length == 0)
        {
            Debug.LogError("HexagonManager: Hexagon array is empty! Please assign hexagons in the inspector.");
        }
    }

    // Method to get a specific hexagon's position
    public Vector3 GetHexagonPosition(int index)
    {
        if (index >= 0 && index < hexagons.Length)
        {
            return hexagons[index].position;
        }
        else
        {
            Debug.LogError("HexagonManager: Index out of range!");
            return Vector3.zero;
        }
    }

    // Optionally, highlight a specific hexagon (e.g., on player input)
    public void HighlightHexagon(int index, Color highlightColor)
    {
        if (index >= 0 && index < hexagons.Length)
        {
            hexagons[index].GetComponent<UnityEngine.UI.Image>().color = highlightColor;
        }
    }

    public RectTransform[] GetHexagons()
    {
        return hexagons;
    }
}


