using UnityEngine;

public class BeatSpawner : MonoBehaviour
{
    public GameObject beatPrefab;  // The Beat prefab
    public Transform spawnPoint;  // Where beats spawn
    public float beatSpeed = 1f;  // Speed of the beats
    public float beatInterval = 1f;  // Time between beats

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= beatInterval)
        {
            SpawnBeat();
            timer -= beatInterval;
        }
    }

    void SpawnBeat()
    {
        Debug.Log("?");
        GameObject beat = Instantiate(beatPrefab, spawnPoint.position, Quaternion.identity, transform);

        // Should be the same as parent's rotation otherwise it will be out of shape
        beat.transform.rotation = transform.rotation;

        beat.GetComponent<Rigidbody2D>().velocity = new Vector2(-beatSpeed, 0);  // Move left
    }
}

