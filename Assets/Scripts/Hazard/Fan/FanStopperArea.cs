using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanStopperArea : MonoBehaviour
{
    public void OnTriggerStay(Collider other)
    {
        // So that enemies can collide with other objects and be stopped by fan stopper.
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Collider>().isTrigger = false;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Collider>().isTrigger = true;
        }
    }
}
