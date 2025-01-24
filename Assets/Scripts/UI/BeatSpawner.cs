using System.Collections;
using UnityEngine;

public class BeatSpawner : MonoBehaviour
{
    [SerializeField] private GameObject beatPrefab; // The Beat prefab
    [SerializeField] private HexagonManager hexagonManager; // Reference to the HexagonManager
    [SerializeField] private float bpm = 120f; // Beats per minute

    private float stepInterval; // Time interval for each beat to move one step

    void Start()
    {
        // Calculate the interval based on BPM
        stepInterval = 60f / bpm; 
        StartCoroutine(SpawnBeats());
    }

    private IEnumerator SpawnBeats()
    {
        while (true)
        {
            SpawnBeat();
            yield return new WaitForSeconds(stepInterval * hexagonManager.GetHexagons().Length); // Wait for a full cycle of beats
        }
    }

    private void SpawnBeat()
    {
        // Instantiate a new beat at the starting hexagon position
        GameObject beatInstance = Instantiate(beatPrefab, hexagonManager.GetHexagonPosition(0), Quaternion.identity, transform);

        // Initialize the beat with HexagonManager and step interval
        Beat beatScript = beatInstance.GetComponent<Beat>();
        if (beatScript != null)
        {
            beatScript.Initialize(hexagonManager, stepInterval);
        }
    }
}
