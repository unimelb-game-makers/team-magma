// Author : Peiyu Wang @ Daphatus
// 19 01 2025 01 34

using System.Collections;
using UnityEngine;

namespace Damage
{
    public class AreaDamage : MonoBehaviour
    {
        private float attackAreaRadius = 0.5f;
        private float attackAngle = 90f;

        [Header("Debug / Visualization")]
        [Tooltip("Whether to draw real-time interpolated wind-up range using Gizmos in the scene")]
        [SerializeField] private float windUpDuration = 0.5f; // Duration of the wind-up phase

        // Damage value (exposed via property)
        private float _damage = 1f;
        private GameObject _instigator;
        [SerializeField]  private float time;
        private float startRadius;
        private float startAngle;
        private float targetRadius;
        private float targetAngle;
        // Interpolated radius and angle for visualization
        private float currentRadius;
        private float currentAngle;
        private Vector3 originalScale;
        
        public void InitializeAttack(float damage, float radius, float angle, float windUpTime,GameObject self)
        {
            _damage = damage;
            attackAreaRadius = radius;
            attackAngle = angle;
            _instigator = self;

            // Store the original scale of the prefab
            originalScale = transform.localScale;

            // Initialize wind-up interpolation
            time = windUpTime;
            startRadius = 0f;
            startAngle = 0f;
            targetRadius = attackAreaRadius;
            targetAngle = attackAngle;
            currentRadius = 0f;
            currentAngle = 0f;
        }



        public void DealDamage()
        {
            // 1) Use OverlapSphere to find all colliders in the circular range (can filter with LayerMask)
            Collider[] hits = Physics.OverlapSphere(transform.position, attackAreaRadius);

            foreach (var hit in hits)
            {
                if(hit.gameObject == _instigator) continue; // Skip self

                Vector3 dir = hit.transform.position - transform.position;
                if(IsWithinAttackArea(hit.transform.position))
                {
                    float angleBetween = Vector3.Angle(transform.forward, dir);
                    if (angleBetween <= attackAngle * 0.5f)
                    {
                        // 3) Get the Damageable component and apply damage
                        var damageable = hit.GetComponent<Damageable>();
                        if (damageable)
                        {
                            damageable.TakeDamage(_damage);
                        }
                    }
                }
            }
            Destroy(gameObject, 0.1f);
        }

   
        
        /// <summary>
        /// Wind-up: Interpolates from 0 to attackAreaRadius & 0 to attackAngle over time.
        /// Used to visually represent the attack area expanding during the wind-up phase.
        /// This method handles repeated external calls safely.
        /// </summary>
        public void WindUp()
        {
            time-=Time.deltaTime;
            if (time <= 0) return; // Wind-up is complete
            currentRadius = Mathf.Lerp(startRadius, targetRadius, 1f - time / windUpDuration);
            currentAngle = Mathf.Lerp(startAngle, targetAngle, 1f - time / windUpDuration);
            
            // Change the size of the mesh after the wind-up completes
            transform.localScale = new Vector3(currentRadius * originalScale.x, 
                            originalScale.y, currentRadius * originalScale.z);
        }

        // ----------------------------------
        //   Determines if a target position is within the attack area
        // ----------------------------------
        private bool IsWithinAttackArea(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;
            float distance = direction.magnitude;
            if (distance > attackAreaRadius) return false;

            float angleToTarget = Vector3.Angle(transform.forward, direction);
            return angleToTarget <= attackAngle * 0.5f;
        }

        // ----------------------------------
        //        Debug Gizmos
        // ----------------------------------
        /// <summary>
        /// Draws debug Gizmos in the Scene view to help visualize the attack area.
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            
            // If in play mode and using wind-up visualization, draw the current interpolated values
            // Otherwise, draw the predefined attackAreaRadius / attackAngle
            float radiusToDraw = Application.isPlaying ? currentRadius : attackAreaRadius;
            float angleToDraw = Application.isPlaying ? currentAngle : attackAngle;

            // Draw a circular wireframe to represent the attack radius
            Gizmos.DrawWireSphere(transform.position, radiusToDraw);

            // Draw lines to represent the boundaries of the attack angle
            Gizmos.DrawLine(
                transform.position,
                transform.position + Quaternion.Euler(0, angleToDraw / 2, 0) * transform.forward * radiusToDraw
            );
            Gizmos.DrawLine(
                transform.position,
                transform.position + Quaternion.Euler(0, -angleToDraw / 2, 0) * transform.forward * radiusToDraw
            );
        }
    }
}