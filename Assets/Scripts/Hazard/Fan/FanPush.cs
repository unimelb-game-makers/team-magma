// Author : William Alexander Tang Wai @ Jalapeno
// 11/01/2025 22:20

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public void OnTriggerStay(Collider collider)
        {
            // Ignore objects with trigger colliders.
            if (collider.isTrigger) return;

            // Push player or enemies.
            // Check if the object has a Rigidbody to apply the force.
            Rigidbody rb = collider.attachedRigidbody;
            if (rb != null)
            {
                // Apply the force in the specified direction and magnitude.
                Vector3 force = forceDirection.normalized * (forceAmount / rb.mass);
                // Clamp velocity to prevent "yeeting"
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, 10f);
                // ForceMode.Force for continuous force.
                rb.AddForce(force, ForceMode.Force);

                Debug.Log("Characters are being pushed!");
            }
        }
    }
}