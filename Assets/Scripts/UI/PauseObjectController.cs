using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseObjectController : MonoBehaviour
{
    public string[] tagsToDisable = { "Player", "Enemy" };

    public void DisableObjects()
    {
        return;
        // If this is to pause the game while in a menu there is a better way to do this. -Ryan
        /* 
        // Loop through each tag to disable relevant GameObjects
        foreach (string tag in tagsToDisable)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scripts)
                {
                    script.enabled = false; // Disable all scripts on this GameObject
                }
            }
        } 
        */
    }

    public void EnableObjects()
    {
        return;
        // If this is to pause the game while in a menu there are better ways to do this. -Ryan
        /* 
        // Loop through each tag to enable relevant GameObjects
        foreach (string tag in tagsToDisable)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scripts)
                {
                    script.enabled = true; // Enable all scripts on this GameObject
                }
            }
        }
        */
    }
}
