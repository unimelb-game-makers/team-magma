using UnityEngine;

public class Beat : MonoBehaviour
{
    public string hittableTagName = "HitTarget";
    private bool canBeHit;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(hittableTagName))
        {
            canBeHit = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(hittableTagName))
        {
            canBeHit = false;
            Destroy(gameObject);  // Remove beat after it passes
        }
    }

    public bool IsHittable()
    {
        return canBeHit;
    }

    public void OnHit()
    {
        // Call the method to change the hexagon's color temporarily
        var hexagon = FindObjectOfType<TargetHexagon>(); // Assuming there is only one hexagon
        if (hexagon != null)
        {
            hexagon.ChangeColorTemporary();
        }

        // can add more feedback (e.g., score increment, sound effect)
    }
}


