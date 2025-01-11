// Author : William Alexander Tang Wai @ Jalapeno
// 11/01/2024 22:20

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.ServiceLocator;
using Platforms;
using Tempo;

namespace Hazard
{
    public class Fan : Hazard
    {
        /**
         * The 'fanArea' object is the object that detects characters and 
         * calls the push function in the fan class.
         */
        private GameObject fanArea;
        /**
         * The 'fanBlades' object is the fan itself that needs to rotate.
         */
        private GameObject fanBlades;
        /**
         * The 'fanStopper' object prevents characters from passing through
         * active fans.
         */
        private GameObject fanStopper;

        [Header("Rotation Speed")]
        /**
         * The speed with which the fan rotates when at fast tempo.
         */
        [SerializeField] private Vector3 _fastSpeed = new Vector3(0, 0, -1000);
        /**
         * The speed with which the fan rotates when at slow tempo.
         */
        [SerializeField] private Vector3 _slowSpeed = new Vector3(0, 0, -10);
        private Vector3 currentSpeed;

        [Header("Force and Direction")]
        /**
         * The force with which the fan pushes characters.
         */
        [SerializeField] private float _forceAmount = 1;
        /**
         * The direction of the force in which the fan pushes characters.
         */
        [SerializeField] private Vector3 _forceDirection;

        public void Awake()
        {
            // The 'FanArea' object is the child of the 'Fan' object.
            fanArea = transform.Find("FanArea").gameObject;
            // The 'FanBlades' object is the child of the 'Fan' object.
            fanBlades = transform.Find("FanBlades").gameObject;
            // The 'FanStopper' object is the child of the 'Fan' object.
            fanStopper = transform.Find("FanStopper").gameObject;
            currentSpeed = _fastSpeed;
        }

        public void Start()
        {
            ServiceLocator.Instance.Register<ISyncable>(this);
        }

        public void Update()
        {
            RotateBlades();
        }

        /*
         * Rotate the blades of the fan at the current speed.
         */
        private void RotateBlades()
        {
            fanBlades.transform.Rotate(currentSpeed * Time.deltaTime);
        }

        /**
         * Pushes the characters that enter the 'FanArea' object.
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
         * Switch the fan off for 'duration' seconds.
         */
        public override void Affect(TapeType tapeType, float duration, float effectValue)
        {
            if(tapeType == TapeType.Slow)
            {
                // Disable the 'fanArea' object so no characters are pushed.
                fanArea.SetActive(false);
                // Disable the 'fanStopper' object so characters can pass through.
                fanStopper.SetActive(false);
                currentSpeed = _slowSpeed;

                // Code for Animations and Sounds.

                StartCoroutine(AffectTimer(duration));
            }
        }

        /**
         * After 'duration' seconds, the 'fanArea' object is switched on again.
         */
        private IEnumerator AffectTimer(float duration)
        {
            yield return new WaitForSeconds(duration);

            // Code for Animations and Sounds.
            
            fanArea.SetActive(true);
            fanStopper.SetActive(true);
            currentSpeed = _fastSpeed;
        }
    }
}
