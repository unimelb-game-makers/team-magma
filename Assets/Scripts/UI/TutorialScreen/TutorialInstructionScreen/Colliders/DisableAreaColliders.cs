using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAreaColliders : MonoBehaviour
{
    [SerializeField] private GameObject areaCollider1;
    [SerializeField] private GameObject areaCollider2;

    void OnDestroy()
    {
        areaCollider1.SetActive(false);
        areaCollider2.SetActive(false);
    }
}
