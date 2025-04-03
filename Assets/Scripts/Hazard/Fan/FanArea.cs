// Author : William Alexander Tang Wai @ Jalapeno
// 11/01/2025 22:20

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

namespace Hazard
{
    public class FanArea : MonoBehaviour
    {
        private float movementAmount;
        private float forceAmount;
        private Vector3 forceDirection;
        private Rigidbody player;
        private List<NavMeshAgent> _agents = new();

        public void SetForcesAmount(float forceAmount, float movementAmount)
        {
            this.forceAmount = forceAmount;
            this.movementAmount = movementAmount;
        }

        public void SetForceDirection(Vector3 direction)
        {
            forceDirection = direction;
        }

        public void RemoveAllObjects()
        {
            if (player)
            {
                player.velocity = Vector3.zero;
                player = null;
            }
            _agents.Clear();

            // Debug.Log("All objects have stopped being affected by " + gameObject);
        }

        // The objects should be pushed/pulled in the fixed update
        void FixedUpdate()
        {
            // Apply the force in the specified direction and magnitude for the player.
            Vector3 force = forceAmount * forceDirection.normalized;
            if (player)
            {
                player.AddForce(force, ForceMode.Acceleration);
                // Debug.Log("Player is being pushed/pulled!");
            }

            // How much should the enemies be affected.
            Vector3 moveDirection = Time.fixedDeltaTime * movementAmount * forceDirection.normalized;
            for (int i = _agents.Count - 1; i >= 0; i--)
            {
                if (_agents[i] == null)
                {
                    _agents.RemoveAt(i);
                    continue;
                }

                _agents[i].Move(moveDirection);

                // So the agent is not pulled into the fan.
                if (_agents[i].GetComponent<Collider>().isTrigger)
                {
                    _agents[i].GetComponent<Collider>().isTrigger = false;
                }

                // Debug.Log("Enemies are being pushed/pulled!");
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            // Add player to the 'affected' list.
            if (other.CompareTag("Player") && !player)
            {
                // Get a reference to the player's rb
                Rigidbody rb = other.attachedRigidbody;
                if (rb != null)
                {
                    player = rb;
                }
            }
            // Add enemies to the 'affected' list.
            else if (other.CompareTag("Enemy") && !_agents.Contains(other.GetComponent<NavMeshAgent>()))
            {
                // Get a reference to the enemy's transform.
                _agents.Add(other.GetComponent<NavMeshAgent>());
                // Debug.Log("Enemy has entered the fan area!");
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && player)
            {
                player.velocity = Vector3.zero;
                player = null;
            }

            if (other.CompareTag("Enemy") && _agents.Contains(other.GetComponent<NavMeshAgent>()))
            {
                other.GetComponent<Collider>().isTrigger = true;
                _agents.Remove(other.GetComponent<NavMeshAgent>());
                // Debug.Log("Enemy has left the fan area!");
            }
        }
    }
}