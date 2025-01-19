using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f; public float Speed { get => speed; set => speed = value; }
    [SerializeField] private float lifetime = 2f;
    private GameObject _player;
    public bool IsHoming { get; set; } = false;
    
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
    
    public void SetInitialDirection(Vector3 direction)
    {
        transform.forward = direction;
    }
    
    public void SetInitialDirection(Vector3 direction, GameObject player)
    {
        _player = player;
        transform.forward = direction;
    }
    
    private void Update()
    {
        if(IsHoming)
        {
            HomingProjectileMovement();
        }
        else
        {
            ProjectileMovement();
        }
    }

    private void HomingProjectileMovement()
    {
        if (_player == null)
        {
            ProjectileMovement();
        }
        Vector3 direction = (_player.transform.position - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    /**
     * Moves the projectile forward
     */
    private void ProjectileMovement()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
