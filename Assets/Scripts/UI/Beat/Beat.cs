using UnityEngine;

public class Beat : MonoBehaviour
{
    private RectTransform rectTransform;
    // Assign the target hexagon of the beat here
    private TargetHexagon targetHexagon;
    private RectTransform target;
    private float speed;
    // How close it needs to be to the target to count as a hit
    private float hitTolerance;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialise(TargetHexagon hexagon, float beatSpeed, float hitDistanceTolerance)
    {
        targetHexagon = hexagon;
        target = targetHexagon.gameObject.GetComponent<RectTransform>();
        speed = beatSpeed;
        hitTolerance = hitDistanceTolerance;
    }

    void Update()
    {
        // Move beat
        rectTransform.anchoredPosition -= new Vector2(speed * Time.deltaTime, 0);

        // If the beat was not hit before it went beyond the target hexagon
        if (IsBeyondLine()) {
            // Call the method to change the hexagon's color temporarily
            if (targetHexagon != null) targetHexagon.ChangeColorTemporary(false);
            Destroy(gameObject);  // Remove beat after it was hit
        }
    }

    private bool IsBeyondLine()
    {
        // x pos of the beat and its target
        float beatX = rectTransform.anchoredPosition.x;
        float targetX = target.anchoredPosition.x;

        // the beat was not hit on time
        if (beatX > 0) {
            return (beatX - targetX) < -hitTolerance;
        } else {
            return (beatX - targetX) > hitTolerance;
        }
    }

    public bool IsHittable()
    {
        if (!rectTransform) return false;

        // x pos of the beat and its target
        float beatX = rectTransform.anchoredPosition.x;
        float targetX = target.anchoredPosition.x;

        return Mathf.Abs(beatX - targetX) <= hitTolerance;
    }

    public void OnHit()
    {
        // Call the method to change the hexagon's color temporarily
        if (targetHexagon != null) targetHexagon.ChangeColorTemporary(true);
        Destroy(gameObject);  // Remove beat after it was hit

        // can add more feedback (e.g., score increment, sound effect)
    }
}


