// Author : William Alexander Tang Wai @ Jalapeno
// 12/01/2025 14:33

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public void OnTriggerStay(Collider collider)
        {
            // Ignore objects with trigger colliders.
            if (collider.isTrigger) return;

            // Pull player or enemies.
            // Check if the object has a Rigidbody to apply the force.
            Rigidbody rb = collider.attachedRigidbody;
            if (rb != null)
            {
                // Apply the force in the specified direction and magnitude.
                // Negative for the opposite direction of push.
                Vector3 force = forceDirection.normalized * (forceAmount / rb.mass);
                // Clamp velocity to prevent "yeeting"
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, 10f);
                // ForceMode.Force for continuous force.
                rb.AddForce(force, ForceMode.Force);

                Debug.Log("Characters are being pulled!");
            }
        }
    }
}
