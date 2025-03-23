// Author : William Alexander Tang Wai @ Jalapeno
// 11/01/2025 22:20

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Hazard
{
    public class FanPush : MonoBehaviour
    {
        private float forceAmount;
        private Vector3 forceDirection;

        public void SetForceAmount(float amount)
        {
            forceAmount = amount;
        }

        public void SetForceDirection(Vector3 direction)
        {
            forceDirection = direction;
        }

        /**
         * When characters stay in the fanPush, push them.
         */
        public void OnTriggerStay(Collider other)
        {
            // Push player or enemies.
            if (other.CompareTag("Player"))
            {
                // Check if the object has a Rigidbody to apply the force.
                Rigidbody rb = other.attachedRigidbody;
                if (rb != null)
                {
                    // Apply the force in the specified direction and magnitude.
                    Vector3 force = forceDirection.normalized * forceAmount;
                    // Clamp velocity to prevent "yeeting"
                    rb.velocity = Vector3.ClampMagnitude(rb.velocity, 10f);
                    // ForceMode.Force for continuous force.
                    rb.AddForce(force, ForceMode.Acceleration);

                    Debug.Log("Player/Enemies are being pushed!");
                }
            }

            // To test later
            // else if (other.CompareTag("Enemy"))
            // {
            //     // Check if the object has a Rigidbody to apply the force.
            //     Rigidbody rb = other.attachedRigidbody;
            //     if (rb != null)
            //     {
            //         var maxSpeed = rb.GetComponent<NavMeshAgent>().speed;
            //         rb.velocity += 10 * Time.deltaTime * forceDirection.normalized;

            //         Debug.Log("Enemies are being pushed!");
            //     }
            // }
        }

        public void OnTriggerExit(Collider other)
        {
            // Check if the object has a Rigidbody to remove the force.
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
            }
        }
    }
}