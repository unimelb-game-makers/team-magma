using System;
using System.Collections;
using System.Collections.Generic;
using Timeline;
using Unity.VisualScripting;
using UnityEngine;

public class SampleButtonTrigger : MonoBehaviour
{
    [SerializeField] private GameObject musicManager;
    [SerializeField] private int _intensity = 0;

    private float flashCooldown = 0.00f;
    private float flashCurrentCooldown = 0.0f;
    private float lastBeat = 0.0f;

    private Material mat;
    private Color[] colors = {Color.black, Color.white};

    void Awake() {
        musicManager = GameObject.FindGameObjectWithTag("RhythmManager");
    }

    // Start is called before the first frame update
    void Start()
    {
        lastBeat = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        MusicTimeline timelineComponent = musicManager.GetComponent<MusicTimeline>();
        if (timelineComponent != null) {
            // When on corresponding intensity flash this object.
            Material mat = GetComponent<MeshRenderer>().material;
            Color matColor = mat.color;
            GetComponent<MeshRenderer>().enabled = true;
            
            if (timelineComponent.GetIntensity() == _intensity) {
                if (timelineComponent.GetOnBeat() == true) {
                    lastBeat = Time.time;
                }
                flashCurrentCooldown = (float)Math.Max(0.0, flashCurrentCooldown - Time.deltaTime);
                if (lastBeat + flashCurrentCooldown < Time.time) {
                        flashCurrentCooldown = flashCooldown;
                        GetComponent<MeshRenderer>().enabled = false;
                } else {
                    GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other==null) return;
        if(other.gameObject.GetComponent<Player.PlayerController>()==null) return;
        // Upon colliding with player activate function.
        Activate(musicManager, _intensity);
        
    }

    private void Activate(GameObject musicManager, int intensity) {
        MusicTimeline timelineComponent = musicManager.GetComponent<MusicTimeline>();;
        // Check if the timeline component exists.
        if (timelineComponent != null) {
            timelineComponent.SetIntensity(intensity);
        }
        print("Activated: Setting Intensity to " + intensity);

    }
}
