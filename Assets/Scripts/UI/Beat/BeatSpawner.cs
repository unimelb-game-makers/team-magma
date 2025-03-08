using UnityEngine;
using Timeline;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class BeatSpawner : MonoBehaviour
{
    [SerializeField] private GameObject musicManager;
    public GameObject beatPrefab;  // The Beat prefab
    public Transform spawnPointLeft;  // Where beats spawn on the left
    public Transform spawnPointRight;  // Where beats spawn on the right
    public Transform hexagonLeft;
    public Transform hexagonRight;
    private List<Beat> beats = new();
    [SerializeField] private float hitTolerance = 25f;
    private float beatTravelTime;  // Time for the beat to move from its start to end pos

    void Awake() {
        musicManager = GameObject.FindGameObjectWithTag("RhythmManager");
    }

    public void SetTempo(float tempo)
    {
        // The number of seconds between each beat
        float beatInterval = 60f / tempo;
        // The time it takes for this beat to reach the target
        beatTravelTime = beatInterval * 2f;
    }

    public void SpawnBeat()
    {
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
    public bool AnyBeatsHittable()
    {
        bool hitOnBeat = false;
        float numBeatsHit = 0;

        for (int i = beats.Count - 1; i >= 0; i--)
        {
            if (beats[i].IsHittable())
            {
                beats[i].OnHit();
                beats.RemoveAt(i);
                hitOnBeat = true;

                numBeatsHit++;
                if (numBeatsHit >= 2) break;
            }
        }

        return hitOnBeat;
    }
}

