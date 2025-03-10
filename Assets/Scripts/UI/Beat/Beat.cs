using System.Collections;
using UnityEngine;

public class Beat : MonoBehaviour
{
    private RectTransform rectTransform;
    private TargetHexagon targetHexagon;  // The target hexagon of the beat
    private RectTransform targetPos;
    private float travelTime;  // How much time to travel to the hexagon
    private float hitTolerance;  // How close it needs to be to the target to count as a hit

    public void Initialise(TargetHexagon hexagon, float beatTravelTime, float hitDistanceTolerance)
    {
        rectTransform = GetComponent<RectTransform>();
        targetHexagon = hexagon;
        targetPos = targetHexagon.gameObject.GetComponent<RectTransform>();
        travelTime = beatTravelTime;
        hitTolerance = hitDistanceTolerance;
        StartCoroutine(MoveToLine());
    }

    private IEnumerator MoveToLine()
    {
        Vector3 startPos = rectTransform.anchoredPosition;

        Vector3 endPos = targetPos.anchoredPosition;
        // Depending on whether the beat spawned on the left or right, the end position will change
        // x pos of the beat
        float beatX = rectTransform.anchoredPosition.x;
        if (beatX > 0) {
            endPos -= new Vector3(hitTolerance, 0, 0);
        } else {
            endPos += new Vector3(hitTolerance, 0, 0);
        }

        float elapsedTime = 0f;

        while (elapsedTime < travelTime)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, elapsedTime / travelTime);
            yield return null;
        }

        // If the beat has reached its end position, this means the player missed
        OnMiss();
    }

    public bool IsHittable()
    {
        if (!rectTransform) return false;

        // x pos of the beat and its target
        float beatX = rectTransform.anchoredPosition.x;
        float targetX = targetPos.anchoredPosition.x;

        return Mathf.Abs(beatX - targetX) <= hitTolerance;
    }

    public void OnHit()
    {
        // Call the method to change the hexagon's color temporarily
        if (targetHexagon != null) targetHexagon.ChangeColorTemporary(true);
        Destroy(gameObject);  // Remove beat after it was hit

        // can add more feedback (e.g., score increment, sound effect)
    }

    public void OnMiss()
    {
        // Call the method to change the hexagon's color temporarily
        if (targetHexagon != null) targetHexagon.ChangeColorTemporary(false);
        Destroy(gameObject);  // Remove beat

        // can add more feedback (e.g., score increment, sound effect)
    }
}


