using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAreaColliders : MonoBehaviour
{
    [SerializeField] private GameObject areaCollider1;
    [SerializeField] private GameObject areaCollider2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            areaCollider1.SetActive(true);
            areaCollider2.SetActive(true);
        }
    }
}
