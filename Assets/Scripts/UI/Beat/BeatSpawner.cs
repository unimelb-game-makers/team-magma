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
    public float beatSpeed;  // Speed of the beats (500 is a ok guess for default tempo)
    public float beatInterval;  // Time between beats (0.5 for deault tempo)
    private float beatIntervalDefault = 0.5f;
    private float beatIntervalSlow = 0.5f * (6.0f/7.0f); // 140bpm -> 120bpm
    private float beatIntervalFast = 0.5f * (8.0f/7.0f); // 140bpm -> 160bpm
    
    private float lastBeat = 0.0f;
    public bool useBeatManager = true;
    private float timer;

    void Awake() {
        musicManager = GameObject.FindGameObjectWithTag("RhythmManager");
    }

    void Start()
    {
        lastBeat = Time.time;
    }

    void Update()
    {
        // Get beat info from FMOD, needs some lookahead added.
        if (useBeatManager) {
            MusicTimeline timelineComponent = musicManager.GetComponent<MusicTimeline>();
            if (timelineComponent.GetOnBeat() == true) {
                //Debug.Log("beat");
                if (Time.time >= beatInterval + lastBeat) {
                    SpawnBeat(); // Currently spawns it at it's spawn location but we don't know if it will be on time at target!
                    lastBeat = Time.time - 0.3f;
                }
            }
            
            switch (timelineComponent.GetIntensity()) 
            {
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
        
        } else {
            timer += Time.deltaTime;

            if (timer >= beatInterval)
            {
                SpawnBeat();
                timer -= beatInterval;
            }
        }
    }

    void SpawnBeat()
    {
        GameObject beatLeft = Instantiate(beatPrefab, spawnPointLeft.position, Quaternion.identity, transform);
        // Should be the same as parent's rotation otherwise it will be out of shape
        beatLeft.transform.rotation = transform.rotation;
        beatLeft.GetComponent<Beat>().Initialise(hexagonLeft.GetComponent<TargetHexagon>(), -beatSpeed, hitTolerance);

        GameObject beatRight = Instantiate(beatPrefab, spawnPointRight.position, Quaternion.identity, transform);
        // Should be the same as parent's rotation otherwise it will be out of shape
        beatRight.transform.rotation = transform.rotation;
        beatRight.GetComponent<Beat>().Initialise(hexagonRight.GetComponent<TargetHexagon>(), beatSpeed, hitTolerance);

        beats.Add(beatLeft.GetComponent<Beat>());
        beats.Add(beatRight.GetComponent<Beat>());
    }

    // Check for and remove beats that have been hit
    public bool AnyBeatsHittable()
    {
        bool hitOnBeat = false;

        for (int i = beats.Count - 1; i >= 0; i--)
        {
            if (beats[i].IsHittable())
            {
                beats[i].OnHit();
                beats.RemoveAt(i);
                hitOnBeat = true;
            }
        }

        return hitOnBeat;
    }
}

