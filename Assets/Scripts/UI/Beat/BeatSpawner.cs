using UnityEngine;
using Timeline;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class BeatSpawner : MonoBehaviour
{
    public GameObject beatPrefab;  // The Beat prefab
    public Transform spawnPointLeft;  // Where beats spawn on the left
    public Transform spawnPointRight;  // Where beats spawn on the right
    public Transform hexagonLeft;  // Beats spawning on the left move to this
    public Transform hexagonRight;  // Beats spawning on the right move to this
    private List<Beat> beats = new();  // Stores the active beats
    private float beatTravelTime;  // Time for the beat to move from its start to end pos

    [Tooltip("Hit range to hit beats on time")]
    [SerializeField] private float hitTolerance = 25f;

    public void SetTempo(float tempo)
    {
        // The number of seconds between each beat
        float beatInterval = 60f / tempo;
        // The time it takes for this beat to reach the target
        beatTravelTime = beatInterval * 2f;
    }

    public void SpawnBeat()
    {
        if (spawnPointLeft == null)
        {
            Debug.LogError("Spawn point was not set, check, i don't wanna fix this, fix if you can");
            return;
        }
        GameObject beatLeft = Instantiate(beatPrefab, spawnPointLeft.position, Quaternion.identity, transform);
        // Should be the same as parent's rotation otherwise it will be out of shape
        beatLeft.transform.rotation = transform.rotation;
        beatLeft.GetComponent<Beat>().Initialise(hexagonLeft.GetComponent<TargetHexagon>(), beatTravelTime, hitTolerance);

        GameObject beatRight = Instantiate(beatPrefab, spawnPointRight.position, Quaternion.identity, transform);
        // Should be the same as parent's rotation otherwise it will be out of shape
        beatRight.transform.rotation = transform.rotation;
        beatRight.GetComponent<Beat>().Initialise(hexagonRight.GetComponent<TargetHexagon>(), beatTravelTime, hitTolerance);

        beats.Add(beatLeft.GetComponent<Beat>());
        beats.Add(beatRight.GetComponent<Beat>());
    }

    // Check for and remove beats that have been hit
    public bool HitOnBeat()
    {
        float numBeatsHit = 0;
        for (int i = beats.Count - 1; i >= 0; i--)
        {
            if (beats[i].IsHittable())
            {
                beats[i].OnHit();
                beats.RemoveAt(i);

                numBeatsHit++;
                if (numBeatsHit >= 2) return true;
            }
        }

        return false;
    }
}

