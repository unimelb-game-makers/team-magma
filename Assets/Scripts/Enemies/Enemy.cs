using System;
using Player;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        private PlayerController target;
        //pathfinding
        private NavMeshAgent agent;

        [SerializeField] private GameObject projectilePrefab;

        [SerializeField] private float shootRate = 1f;

        private float _nextShoot;

        private void Start()
        {
            target = FindObjectOfType<PlayerController>();
            agent = GetComponent<NavMeshAgent>();

            _nextShoot = Time.time + shootRate;
        }

        void Update()
        {
            if (target != null) {
                // If you want to update the destination dynamically
                if (Vector3.Distance(agent.destination, target.transform.position) > 0.1f)
                {
                    agent.SetDestination(target.transform.position);
                }

                if (Time.time > _nextShoot) {
                    // Update the time when the enemy can shoot next
                    _nextShoot = Time.time + shootRate;

                    Shoot();
                }
            }
        }

        private void Shoot()
    {
        
        //spawn a projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        //get the Projectile component from the projectile object
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        //check if the Projectile component exists
        if (projectileComponent != null)
        {
            //set the initial direction of the projectile
            projectileComponent.SetInitialDirection((target.transform.position - transform.position).normalized);
        }
    }
    }
}