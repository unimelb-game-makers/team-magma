using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackBox : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f;
    
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}