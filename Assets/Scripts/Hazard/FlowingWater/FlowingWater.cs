// Author : William Alexander Tang Wai @ Jalapeno
// 12/01/2025 18:10

using System.Collections;
using System.Collections.Generic;
using Utilities.ServiceLocator;
using UnityEngine;
using Platforms;
using Tempo;

namespace Hazard
{
    public class FlowingWater : Hazard
    {
        [Header("Fan Children Objects")]
        /**
         * The 'killArea' object is the object that detects characters and 
         * kills them.
         */
        private GameObject killArea;
        /**
         * The 'height1' float is the height of the 'KillArea' at slow tempo.
         */
        private float height1;
        /**
         * The 'height2' float is the height of the 'KillArea' at mid tempo.
         */
        private float height2;
        /**
         * The 'height3' float is the height of the 'KillArea' at fast tempo.
         */
        private float height3;

        [Header("Speed")]
        [Tooltip("The duration which the KillArea takes to move between heights when changing tempo.")]
        [SerializeField] private float _duration = 1;

        [Header("Damage")]
        [Tooltip("The damage which the KillArea deals.")]
        [SerializeField] private float _damage = 999;

        private Coroutine resetWaterCoroutine;

        public void Awake()
        {
            // The 'KillArea' object is the child of the 'FlowingWater' object.
            killArea = transform.Find("KillArea").gameObject;
            killArea.GetComponent<FlowingWaterDamager>().SetDamage(_damage);

            // The 'heights' objects are the children of the 'FlowingWater' object.
            height1 = transform.Find("Height1").gameObject.transform.position.y;
            height2 = transform.Find("Height2").gameObject.transform.position.y;
            height3 = transform.Find("Height3").gameObject.transform.position.y;

            // The default position of the killArea should be height2.
            killArea.transform.position = new Vector3(killArea.transform.position.x, height2, killArea.transform.position.z);
        }

        public void Start()
        {
            ServiceLocator.Instance.Register<ISyncable>(this);
        }

        private IEnumerator MoveKillAreaToHeight(float targetHeight)
        {
            Vector3 startPosition = killArea.transform.position;
            Vector3 targetPosition = new Vector3(startPosition.x, targetHeight, startPosition.z);

            float elapsedTime = 0f;

            while (elapsedTime < _duration)
            {
                // Interpolate position over time
                killArea.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / _duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the exact target position is set
            killArea.transform.position = targetPosition;
        }

        /**
         * Move the 'KillArea' depending on the TapeType.
         */
        public override void Affect(TapeType tapeType, float duration, float effectValue)
        {
            if(tapeType == TapeType.Slow)
            {
                // Move the 'KillArea' object to height1.
                StartCoroutine(MoveKillAreaToHeight(height1));

                // Code for Animations and Sounds.

                // If there was a previous timer to return the water to default configuration,
                // then reset it.
                if (resetWaterCoroutine != null) StopCoroutine(resetWaterCoroutine);

                if (useDefaultEffectTimeValues) {
                    resetWaterCoroutine = StartCoroutine(AffectTimer(duration));
                } else {
                    resetWaterCoroutine = StartCoroutine(AffectTimer(_slowEffectTime));
                }
            }

            if(tapeType == TapeType.Fast)
            {
                // Move the 'KillArea' object to height1.
                StartCoroutine(MoveKillAreaToHeight(height3));

                // Code for Animations and Sounds.
                if (resetWaterCoroutine != null) StopCoroutine(resetWaterCoroutine);

                if (useDefaultEffectTimeValues) {
                    resetWaterCoroutine = StartCoroutine(AffectTimer(duration));
                } else {
                    resetWaterCoroutine = StartCoroutine(AffectTimer(_fastEffectTime));
                }
            }
        }

        /**
         * After 'duration' seconds, the KillArea returns to its initial location.
         */
        private IEnumerator AffectTimer(float duration)
        {
            yield return new WaitForSeconds(duration);
            // Code for Animations and Sounds.

            // Move the 'KillArea' object to height3.
            StartCoroutine(MoveKillAreaToHeight(height2));
        }
    }
}
