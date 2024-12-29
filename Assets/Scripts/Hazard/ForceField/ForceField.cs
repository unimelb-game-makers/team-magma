// Author : William Alexander Tang Wai @ Jalapeno
// 15/12/2024 13:09

using Platforms;
using Tempo;
using Utilities.ServiceLocator;
using UnityEngine;
using System.Collections;
using System;

namespace Hazard
{
    public class ForceField : Hazard
    {
        /**
         * The 'forceFieldArea' object is the object that detects characters and 
         * calls the push function in the steamvent class.
         */
        private GameObject forceFieldArea;

        /**
         * The force with which the forceField pushes characters.
         */
        [SerializeField] private float _forceAmount = 1;
        /**
         * The direction of the force in which the forceField pushes characters.
         */
        [SerializeField] private Vector3 _forceDirection;

        public void Awake()
        {
            // The 'ForceFieldArea' object is the child of the 'ForceField' object.
            forceFieldArea = transform.Find("ForceFieldArea").gameObject;
        }

        public void Start()
        {
            ServiceLocator.Instance.Register<ISyncable>(this);
        }

        /**
         * Pushes the characters that enter the 'ForceFieldArea' object.
         */
        public void PushCharacters(Collider collider)
        {
            // Ignore objects with trigger colliders.
            if (collider.isTrigger) return;

            // Push player or enemies.
            // Check if the object has a Rigidbody to apply the force.
            Rigidbody rb = collider.attachedRigidbody;
            if (rb != null)
            {
                // Apply the force in the specified direction and magnitude.
                Vector3 force = _forceDirection.normalized * _forceAmount;
                // ForceMode.Force for continuous force.
                rb.AddForce(force, ForceMode.Force);

                Debug.Log("Characters are being pushed!");
            }
        }

        /**
         * Switch the forceField off for 'duration' seconds.
         */
        public override void Affect(TapeType tapeType, float duration, float effectValue)
        {
            if(tapeType == TapeType.Slow)
            {
                // Disable the 'forceFieldArea' object so no damage is dealt.
                forceFieldArea.SetActive(false);

                // Code for Animations and Sounds.

                StartCoroutine(AffectTimer(duration));
            }
        }

        /**
         * After 'duration' seconds, the 'forceFieldArea' object is switched on again.
         */
        private IEnumerator AffectTimer(float duration)
        {
            yield return new WaitForSeconds(duration);

            // Code for Animations and Sounds.
            
            forceFieldArea.SetActive(true);
        }
    }
}