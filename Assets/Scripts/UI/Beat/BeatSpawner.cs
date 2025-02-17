using UnityEngine;
using Timeline;

public class BeatSpawner : MonoBehaviour
{
    [SerializeField] private GameObject musicManager;
    public GameObject beatPrefab;  // The Beat prefab
    public Transform spawnPoint;  // Where beats spawn
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
        GameObject beat = Instantiate(beatPrefab, spawnPoint.position, Quaternion.identity, transform);

        // Should be the same as parent's rotation otherwise it will be out of shape
        beat.transform.rotation = transform.rotation;
        beat.GetComponent<Rigidbody2D>().velocity = new Vector2(-beatSpeed, 0);  // Move left
    }
}

