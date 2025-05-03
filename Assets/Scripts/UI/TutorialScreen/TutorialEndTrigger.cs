using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

[RequireComponent(typeof(Collider))]
public class TutorialEndTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                TutorialEndScreenManager.Instance.ShowTutorialEndScreen();
            }
        }
}