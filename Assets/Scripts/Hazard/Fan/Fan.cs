// Author : William Alexander Tang Wai @ Jalapeno
// 11/01/2025 22:20

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
        private GameObject fanPushDefault;
        private GameObject fanPushFast;
        /**
         * The 'fanPull' object is the object that detects characters and 
         * calls the pull function in the fan class.
         */
        private GameObject fanPullDefault;
        private GameObject fanPullFast;
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
        [SerializeField] private Vector3 _defaultSpeed = new(0, 0, -500);
        [Tooltip("The speed at which the fan rotates when at fast tempo.")]
        [SerializeField] private Vector3 _fastSpeed = new(0, 0, -1000);
        [Tooltip("The speed at which the fan rotates when at slow tempo.")]
        [SerializeField] private Vector3 _slowSpeed = new(0, 0, -10);

        [Header("Damage and Knockback")]
        [SerializeField] private float _defaultDamage = 10;
        [Tooltip("The damage which the fan deals when at fast tempo.")]
        [SerializeField] private float _fastDamage = 10;
        [Tooltip("The damage which the fan deals when at slow tempo.")]
        [SerializeField] private float _slowDamage = 5;

        [SerializeField] private float _defaultKnockbackForce = 75;
        [Tooltip("The force with which the characters are knockbacked when at fast tempo.")]
        [SerializeField] private float _fastKnockbackForce = 100;
        [Tooltip("The force with which the characters are knockbacked when at slow tempo.")]
        [SerializeField] private float _slowKnockbackForce = 50;

        [Tooltip("The force with which the enemies are knockbacked.")]
        [SerializeField] private float _defaultEnemyKnockbackForce = 5;
        [SerializeField] private float _fastEnemyKnockbackForce = 8;
        [SerializeField] private float _slowEnemyKnockbackForce = 2;

        [Header("Force and Direction")]
        [Tooltip("The force with which the fan pushes/pulls player.")]
        [SerializeField] private float _defaultForceAmount = 800;
        [SerializeField] private float _fastForceAmount = 1000;
        [Tooltip("The 'force' with which the fan pushes/pulls enemies.")]
        [SerializeField] private float _defaultEnemyMovementAmount = 5;
        [SerializeField] private float _fastEnemyMovementAmount = 10;
        [Tooltip("The direction of the force in which the fan pushes/pulls player and enemies.")]
        private Vector3 _forceDirection;

        private Coroutine resetFanCoroutine;

        public void Awake()
        {
            // The 'FanBlades' object is the child of the 'Fan' object.
            fanBlades = transform.Find("FanBlades").gameObject;
            fanBlades.GetComponent<FanRotate>().SetRotationSpeed(_defaultSpeed);

            // Set the direction to the local backward direction so that the fan
            // faces forward correctly.
            _forceDirection = -transform.forward;

            // Set the damage and knockback for each child blade.
            foreach (Transform fanBlade in fanBlades.transform)
            {
                fanBlade.GetComponent<FanDamager>().SetDamage(_defaultDamage);
                fanBlade.GetComponent<FanDamager>().SetKnockbackForce(_defaultKnockbackForce, _defaultEnemyKnockbackForce);
            }

            // The 'FanPush' object is the child of the 'Fan' object.
            fanPushDefault = transform.Find("FanPush_Default").gameObject;
            fanPushDefault.GetComponent<FanArea>().SetForcesAmount(_defaultForceAmount, _defaultEnemyMovementAmount);
            fanPushDefault.GetComponent<FanArea>().SetForceDirection(_forceDirection);

            fanPushFast = transform.Find("FanPush_Fast").gameObject;
            fanPushFast.GetComponent<FanArea>().SetForcesAmount(_fastForceAmount, _fastEnemyMovementAmount);
            fanPushFast.GetComponent<FanArea>().SetForceDirection(_forceDirection);

            // The 'FanPull' object is the child of the 'Fan' object.
            fanPullDefault = transform.Find("FanPull_Default").gameObject;
            fanPullDefault.GetComponent<FanArea>().SetForcesAmount(_defaultForceAmount, _defaultEnemyMovementAmount);
            fanPullDefault.GetComponent<FanArea>().SetForceDirection(_forceDirection);

            fanPullFast = transform.Find("FanPull_Fast").gameObject;
            fanPullFast.GetComponent<FanArea>().SetForcesAmount(_fastForceAmount, _fastEnemyMovementAmount);
            fanPullFast.GetComponent<FanArea>().SetForceDirection(_forceDirection);

            // The 'FanStopper' object is the child of the 'Fan' object.
            fanStopper = transform.Find("FanStopper").gameObject;

            // Only the default areas should be active at the start.
            fanPushFast.SetActive(false);
            fanPullFast.SetActive(false);
        }

        public void Start()
        {
            ServiceLocator.Instance.Register<ISyncable>(this);
        }

        /**
         * Change the fan speed and damage for 'duration' seconds.
         */
        public override void Affect(TapeType tapeType, float duration, float effectValue)
        {
            if(tapeType == TapeType.Slow)
            {
                fanBlades.GetComponent<FanRotate>().SetRotationSpeed(_slowSpeed);
                foreach (Transform fanBlade in fanBlades.transform)
                {
                    fanBlade.GetComponent<FanDamager>().SetDamage(_slowDamage);
                    fanBlade.GetComponent<FanDamager>().SetKnockbackForce(_slowKnockbackForce, _slowEnemyKnockbackForce);
                }
                
                // Disable the 'fanPush' object so no characters are pushed.
                if (fanPushDefault.activeSelf)
                {
                    fanPushDefault.GetComponent<FanArea>().RemoveAllObjects();
                }
                if (fanPushFast.activeSelf)
                {
                    fanPushFast.GetComponent<FanArea>().RemoveAllObjects();
                }
                fanPushDefault.SetActive(false);
                fanPushFast.SetActive(false);

                // Disable the 'fanPull' object so no characters are pushed.
                if (fanPullDefault.activeSelf)
                {
                    fanPullDefault.GetComponent<FanArea>().RemoveAllObjects();
                }
                if (fanPullFast.activeSelf)
                {
                    fanPullFast.GetComponent<FanArea>().RemoveAllObjects();
                }
                fanPullDefault.SetActive(false);
                fanPullFast.SetActive(false);

                // Disable the 'fanStopper' object so characters can pass through.
                fanStopper.SetActive(false);

                // Code for Animations and Sounds.

                // If there was a previous timer to return the fan to default configuration,
                // then reset it.
                if (resetFanCoroutine != null) StopCoroutine(resetFanCoroutine);

                if (useDefaultEffectTimeValues) {
                    resetFanCoroutine = StartCoroutine(AffectTimer(duration));
                } else {
                    resetFanCoroutine = StartCoroutine(AffectTimer(_slowEffectTime));
                }
            }

            // if TapeType.Fast, switch to fanPush_Fast
            if(tapeType == TapeType.Fast)
            {
                fanBlades.GetComponent<FanRotate>().SetRotationSpeed(_fastSpeed);
                foreach (Transform fanBlade in fanBlades.transform)
                {
                    fanBlade.GetComponent<FanDamager>().SetDamage(_fastDamage);
                    fanBlade.GetComponent<FanDamager>().SetKnockbackForce(_fastKnockbackForce, _fastEnemyKnockbackForce);
                }

                if (fanPushDefault.activeSelf)
                {
                    fanPushDefault.GetComponent<FanArea>().RemoveAllObjects();
                }
                fanPushDefault.SetActive(false);
                fanPushFast.SetActive(true);
                
                if (fanPullDefault.activeSelf)
                {
                    fanPullDefault.GetComponent<FanArea>().RemoveAllObjects();
                }
                fanPullDefault.SetActive(false);
                fanPullFast.SetActive(true);

                fanStopper.SetActive(true);

                if (resetFanCoroutine != null) StopCoroutine(resetFanCoroutine);

                if (useDefaultEffectTimeValues) {
                    resetFanCoroutine = StartCoroutine(AffectTimer(duration));
                } else {
                    resetFanCoroutine = StartCoroutine(AffectTimer(_fastEffectTime));
                }
            }
        }

        /**
         * After 'duration' seconds, the fan is returned to its default configuration.
         */
        private IEnumerator AffectTimer(float duration)
        {
            yield return new WaitForSeconds(duration);

            // Code for Animations and Sounds.

            fanBlades.GetComponent<FanRotate>().SetRotationSpeed(_defaultSpeed);
            foreach (Transform fanBlade in fanBlades.transform)
            {
                fanBlade.GetComponent<FanDamager>().SetDamage(_defaultDamage);
                fanBlade.GetComponent<FanDamager>().SetKnockbackForce(_defaultKnockbackForce, _defaultEnemyKnockbackForce);
            }

            fanPushDefault.SetActive(true);
            if (fanPushFast.activeSelf)
            {
                fanPushFast.GetComponent<FanArea>().RemoveAllObjects();
            }
            fanPushFast.SetActive(false);

            fanPullDefault.SetActive(true);
            if (fanPullFast.activeSelf)
            {
                fanPullFast.GetComponent<FanArea>().RemoveAllObjects();
            }
            fanPullFast.SetActive(false);

            fanStopper.SetActive(true);
        }
    }
}