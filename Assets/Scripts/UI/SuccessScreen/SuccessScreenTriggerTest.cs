using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

// Note that this is temporary for alpha build

[RequireComponent(typeof(Collider))]
public class SuccessScreenTriggerTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SuccessScreenManager.Instance.ShowSuccessScreen();
            }
        }
}