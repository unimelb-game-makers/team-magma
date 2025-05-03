using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TutorialInfo : MonoBehaviour
{
    public GameObject messageCanvas;
    void Start()
    {
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && messageCanvas.activeSelf){
            messageCanvas.SetActive(false);
            Time.timeScale = 1;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (! messageCanvas.activeSelf){
            messageCanvas.SetActive(true);
            Time.timeScale = 0; 

        }

    }
}
