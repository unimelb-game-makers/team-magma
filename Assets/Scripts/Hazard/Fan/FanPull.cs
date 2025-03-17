// Author : William Alexander Tang Wai @ Jalapeno
// 12/01/2025 14:33

using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using UnityEngine.AI;

namespace Hazard
{
    public class FanPull : MonoBehaviour
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
         * When characters stay in the fanPull, pull them.
         */
        public void OnTriggerStay(Collider other)
        {
            // Pull player or enemies
            if (other.CompareTag("Player") || other.CompareTag("Enemy"))
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

                    Debug.Log("Player/Enemies are being pulled!");
                }
            }

            // To test later
            // else if (other.CompareTag("Enemy"))
            // {
            //     // Check if the object has a Rigidbody to apply the force.
            //     Rigidbody rb = other.attachedRigidbody;
            //     if (rb != null)
            //     {
            //         // Enemies could be moving at different speeds or be stationary (when attacking)
            //         // All enemies moves at the same speed no matter the state.
            //         float currentMaxSpeed = other.GetComponent<EnemyController>().GetCurrentMaxSpeed();
            //         float pullSpeed = Mathf.Abs(currentMaxSpeed - 2);

            //         rb.velocity += pullSpeed * Time.deltaTime * forceDirection.normalized;

            //         Debug.Log("Enemies are being pulled!");
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
