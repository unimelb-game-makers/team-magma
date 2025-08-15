using UnityEngine;

public class PulsingGlow : MonoBehaviour
{
    [SerializeField] private float minIntensity = 1f;
    [SerializeField] private float maxIntensity = 5f;
    [SerializeField] private float pulseSpeed = 1f;

    private Material material;
    private float currentIntensity;

    void Start()
    {
        material = GetComponent<Renderer>().material;
        material.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        currentIntensity = Mathf.Lerp(minIntensity, maxIntensity,
            Mathf.PingPong(Time.time * pulseSpeed, 1f));

        material.SetColor("_EmissionColor", Color.yellow * currentIntensity);
    }
}
