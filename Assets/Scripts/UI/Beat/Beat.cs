using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beat : MonoBehaviour
{
    private RectTransform rectTransform;
    private TargetHexagon targetHexagon;  // The target hexagon of the beat
    private RectTransform targetPos;
    private float travelTime;  // How much time to travel to the hexagon
    private float hitTolerance;  // How close it needs to be to the target to count as a hit
    private bool hitted = false;  // Whether the beat has been hit or not
    private Queue<Beat> queue;  // Reference to the BeatSpawner
    public void Initialise(TargetHexagon hexagon, float beatTravelTime, float hitDistanceTolerance, Queue<Beat> queue)
    {
        rectTransform = GetComponent<RectTransform>();
        targetHexagon = hexagon;
        targetPos = targetHexagon.gameObject.GetComponent<RectTransform>();
        travelTime = beatTravelTime;
        hitTolerance = hitDistanceTolerance;
        this.queue = queue;
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
        if (!hitted) OnMiss();

    }

    public bool IsHittable(float tolerance = 0.0f)
    {
        if (!rectTransform) return false;

        // x pos of the beat and its target
        float beatX = rectTransform.anchoredPosition.x;
        float targetX = targetPos.anchoredPosition.x;
        if (tolerance > 0.0f)
        {
            return Mathf.Abs(beatX - targetX) <= tolerance;
        }
        return Mathf.Abs(beatX - targetX) <= hitTolerance;
    }

    public void OnHit()
    {
        // Call the method to change the hexagon's color temporarily
        if (targetHexagon != null) targetHexagon.ChangeColorTemporary(true);
        hitted = true;  // Mark the beat as hit
        queue.Dequeue();  // Remove the beat from the queue
        Destroy(gameObject);  // Remove beat after it was hit

        // can add more feedback (e.g., score increment, sound effect)
    }

    public void OnMiss()
    {
        queue.Dequeue();  // Remove the beat from the queue

        // Call the method to change the hexagon's color temporarily
        if (targetHexagon != null) targetHexagon.ChangeColorTemporary(false);
        Destroy(gameObject);  // Remove beat
        // can add more feedback (e.g., score increment, sound effect)
    }
}


