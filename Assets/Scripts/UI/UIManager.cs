using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static GameObject existingUI;

    void Awake()
    {
        if (existingUI == null)
        {
            existingUI = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else if (existingUI != gameObject)
        {
            Destroy(gameObject); // Destroy duplicate on scene change
        }
    }
}