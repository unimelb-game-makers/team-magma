using UnityEngine;

public class CombatUISystem : MonoBehaviour
{
    public static CombatUISystem Instance { get; private set; }
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartBeat()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    public void StopBeat()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }
}

