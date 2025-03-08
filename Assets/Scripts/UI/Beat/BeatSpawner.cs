using UnityEngine;
using Timeline;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class BeatSpawner : MonoBehaviour
{
    [SerializeField] private GameObject musicManager;
    private int currentIntensity = -1;
    public GameObject beatPrefab;  // The Beat prefab
    public Transform spawnPointLeft;  // Where beats spawn on the left
    public Transform spawnPointRight;  // Where beats spawn on the right
    public Transform hexagonLeft;
    public Transform hexagonRight;
    private List<Beat> beats = new();
    [SerializeField] private float hitTolerance = 25f;
    private float beatTravelTime;  // Time for the beat to move from its start to end pos
    [SerializeField] [Range(-1f, 1f)] private float beatOffsetTime = 0.1f;  // To make the beats on time at target
    public float beatInterval;  // Time between beats (0.5 for deault tempo)
    private float beatIntervalDefault = 0.5f;
    private float beatIntervalSlow = 0.5f * (6.0f/7.0f); // 140bpm -> 120bpm
    private float beatIntervalFast = 0.5f * (8.0f/7.0f); // 140bpm -> 160bpm
    
    private float lastBeat = 0.0f;
    public bool useBeatManager = true;
    private float timer1 = 0f;
    private float timer2 = 0f;

    void Awake() {
        musicManager = GameObject.FindGameObjectWithTag("RhythmManager");
    }

    void Start()
    {
        lastBeat = Time.time;
        timer2 = beatOffsetTime;
    }

    void Update()
    {
        // Start spawning after some time to make beats on time when they reach target
        timer2 -= Time.deltaTime;
        if (timer2 > 0) return;

        // Get beat info from FMOD, needs some lookahead added.
        if (useBeatManager) {
            MusicTimeline timelineComponent = musicManager.GetComponent<MusicTimeline>();
            if (timelineComponent.GetOnBeat() == true) {
                //Debug.Log("beat");
                // If the current time >= time when the next beat should be spawned
                if (Time.time >= beatInterval + lastBeat) {
                    SpawnBeat(); // Currently spawns it at it's spawn location but we don't know if it will be on time at target!
                    lastBeat = Time.time - 0.3f;
                }
            }

            if (currentIntensity != timelineComponent.GetIntensity()) {
                currentIntensity = timelineComponent.GetIntensity();
                switch (currentIntensity) {
                    case 1:
                        beatInterval = beatIntervalSlow;
                        break;
                    case 2:
                        beatInterval = beatIntervalDefault;
                        break;
                    case 3:
                        beatInterval = beatIntervalFast;
                        break;
                }
                // To make beats on time when they reach the target
                beatTravelTime = beatInterval * 2f;

                timer2 = beatOffsetTime;
            }
        
        } else {
            timer1 += Time.deltaTime;

            if (timer1 >= beatInterval)
            {
                SpawnBeat();
                timer1 -= beatInterval;
            }
        }
    }

    void SpawnBeat()
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

