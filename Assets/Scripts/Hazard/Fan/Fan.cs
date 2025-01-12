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
        [Header("Fan Children Objects")]
        /**
         * The 'fanPush' object is the object that detects characters and 
         * calls the push function in the fan class.
         */
        private GameObject fanPush;
        /**
         * The 'fanPull' object is the object that detects characters and 
         * calls the pull function in the fan class.
         */
        private GameObject fanPull;
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
        [Tooltip("The speed at which the fan rotates when at fast tempo.")]
        [SerializeField] private Vector3 _fastSpeed = new(0, 0, -1000);
        [Tooltip("The speed at which the fan rotates when at slow tempo.")]
        [SerializeField] private Vector3 _slowSpeed = new(0, 0, -10);

        [Header("Damage and Knockback")]
        [Tooltip("The damage which the fan deals when at fast tempo.")]
        [SerializeField] private float _fastDamage = 10;
        [Tooltip("The damage which the fan deals when at slow tempo.")]
        [SerializeField] private float _slowDamage = 5;
        [Tooltip("The force with which the characters are knockbacked when at fast tempo.")]
        [SerializeField] private float _fastKnockbackForce = 100;
        [Tooltip("The force with which the characters are knockbacked when at slow tempo.")]
        [SerializeField] private float _slowKnockbackForce = 50;

        [Header("Force and Direction")]
        [Tooltip("The force with which the fan pushes/pulls characters.")]
        [SerializeField] private float _forceAmount = 1;
        [Tooltip("The direction of the force in which the fan pushes/pulls characters.")]
        private Vector3 _forceDirection;

        public void Awake()
        {
            // The 'FanBlades' object is the child of the 'Fan' object.
            fanBlades = transform.Find("FanBlades").gameObject;
            fanBlades.GetComponent<FanRotate>().SetRotationSpeed(_fastSpeed);

            // Set the direction to the local backward direction so that the fan
            // faces forward correctly.
            _forceDirection = -transform.forward;

            // Set the damage and knockback for each child blade.
            foreach (Transform fanBlade in fanBlades.transform)
            {
                fanBlade.GetComponent<FanDamager>().SetDamage(_fastDamage);
                fanBlade.GetComponent<FanDamager>().SetKnockbackForce(_fastKnockbackForce);
                fanBlade.GetComponent<FanDamager>().SetKnockbackDirection(_forceDirection);
            }

            // The 'FanPush' object is the child of the 'Fan' object.
            fanPush = transform.Find("FanPush").gameObject;
            fanPush.GetComponent<FanPush>().SetForceAmount(_forceAmount);
            fanPush.GetComponent<FanPush>().SetForceDirection(_forceDirection);

            // The 'FanPull' object is the child of the 'Fan' object.
            fanPull = transform.Find("FanPull").gameObject;
            fanPull.GetComponent<FanPull>().SetForceAmount(_forceAmount);
            fanPull.GetComponent<FanPull>().SetForceDirection(_forceDirection);

            // The 'FanStopper' object is the child of the 'Fan' object.
            fanStopper = transform.Find("FanStopper").gameObject;
        }

        public void Start()
        {
            ServiceLocator.Instance.Register<ISyncable>(this);
        }


        // ***************************************************************************************************
        // Testing Purposes only - This whole block can be deleted safely.
        [SerializeField] private bool isSlowTempo = false;
        public void Update()
        {
            if (isSlowTempo)
            {
                Affect(TapeType.Slow, 5f, 0);
                isSlowTempo = false;
            }
        }
        // ***************************************************************************************************

        /**
         * Switch the fan off for 'duration' seconds.
         */
        public override void Affect(TapeType tapeType, float duration, float effectValue)
        {
            if(tapeType == TapeType.Slow)
            {
                fanBlades.GetComponent<FanRotate>().SetRotationSpeed(_slowSpeed);
                foreach (Transform fanBlade in fanBlades.transform)
                {
                    fanBlade.GetComponent<FanDamager>().SetDamage(_slowDamage);
                    fanBlade.GetComponent<FanDamager>().SetKnockbackForce(_slowKnockbackForce);
                }
                
                // Disable the 'fanPush' object so no characters are pushed.
                fanPush.SetActive(false);
                // Disable the 'fanPull' object so no characters are pushed.
                fanPull.SetActive(false);

                // Disable the 'fanStopper' object so characters can pass through.
                fanStopper.SetActive(false);

                // Code for Animations and Sounds.

                StartCoroutine(AffectTimer(duration));
            }
        }

        /**
         * After 'duration' seconds, the fan is switched on again.
         */
        private IEnumerator AffectTimer(float duration)
        {
            yield return new WaitForSeconds(duration);

            // Code for Animations and Sounds.

            fanBlades.GetComponent<FanRotate>().SetRotationSpeed(_fastSpeed);
            foreach (Transform fanBlade in fanBlades.transform)
            {
                fanBlade.GetComponent<FanDamager>().SetDamage(_fastDamage);
                fanBlade.GetComponent<FanDamager>().SetKnockbackForce(_fastKnockbackForce);
            }

            fanPush.SetActive(true);
            fanPull.SetActive(true);

            fanStopper.SetActive(true);
        }
    }
}
