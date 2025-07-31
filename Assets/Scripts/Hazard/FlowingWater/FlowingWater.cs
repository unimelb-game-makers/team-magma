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

        [Header("Shader Speed")]
        [Tooltip("The speed of the shader effect on the poison.")]
        [SerializeField] private float slowWaveSpeed = 0.1f;
        [SerializeField] private float mediumWaveSpeed = 0.2f;
        [SerializeField] private float fastWaveSpeed = 0.5f;
        [SerializeField] private float slowRippleSpeed = 1.0f;
        [SerializeField] private float mediumRippleSpeed = 2.0f;
        [SerializeField] private float fastRippleSpeed = 5.0f;
        [SerializeField] private float slowRippleDensity = 10f;
        [SerializeField] private float mediumRippleDensity = 12f;
        [SerializeField] private float fastRippleDensity = 15f;

        [SerializeField] private Material waterMaterial;
        private static readonly int RippleSpeedID = Shader.PropertyToID("_rippleSpeed"),
                                    WaveSpeedID = Shader.PropertyToID("_waveSpeed"),
                                    RippleDensityID = Shader.PropertyToID("_rippleDensity");

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
        private IEnumerator ChangeMaterialDensity(float targetRippleSpeed, float targetWaveSpeed, float targetRippleDensity)
        {
            // Change the material properties over time
            float elapsedTime = 0f;
            float initialRippleSpeed = waterMaterial.GetFloat(RippleSpeedID);
            float initialWaveSpeed = waterMaterial.GetFloat(WaveSpeedID);
            float initialRippleDensity = waterMaterial.GetFloat(RippleDensityID);

            while (elapsedTime < _duration)
            {
                float t = elapsedTime / _duration;

                waterMaterial.SetFloat(RippleSpeedID, Mathf.Lerp(initialRippleSpeed, targetRippleSpeed, t));
                waterMaterial.SetFloat(WaveSpeedID, Mathf.Lerp(initialWaveSpeed, targetWaveSpeed, t));
                waterMaterial.SetFloat(RippleDensityID, Mathf.Lerp(initialRippleDensity, targetRippleDensity, t));

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the exact target values are set
            waterMaterial.SetFloat(RippleSpeedID, targetRippleSpeed);
            waterMaterial.SetFloat(WaveSpeedID, targetWaveSpeed);
            waterMaterial.SetFloat(RippleDensityID, targetRippleDensity);
        }
        /**
         * Move the 'KillArea' depending on the TapeType.
         */
        public override void Affect(TempoMode mode, float duration, float effectValue)
        {
            if (mode == TempoMode.Slow)
            {
                // Move the 'KillArea' object to height1.
                StartCoroutine(MoveKillAreaToHeight(height1));
                StartCoroutine(ChangeMaterialDensity(slowRippleSpeed, slowWaveSpeed, slowRippleDensity));

                // Code for Animations and Sounds.

                // If there was a previous timer to return the water to default configuration,
                // then reset it.
                if (resetWaterCoroutine != null) StopCoroutine(resetWaterCoroutine);

                if (useDefaultEffectTimeValues)
                {
                    resetWaterCoroutine = StartCoroutine(AffectTimer(duration));
                }
                else
                {
                    resetWaterCoroutine = StartCoroutine(AffectTimer(_slowEffectTime));
                }
            }

            if (mode == TempoMode.Fast)
            {
                // Move the 'KillArea' object to height1.
                StartCoroutine(MoveKillAreaToHeight(height3));
                StartCoroutine(ChangeMaterialDensity(fastRippleSpeed, fastWaveSpeed, fastRippleDensity));

                // Code for Animations and Sounds.
                if (resetWaterCoroutine != null) StopCoroutine(resetWaterCoroutine);

                if (useDefaultEffectTimeValues)
                {
                    resetWaterCoroutine = StartCoroutine(AffectTimer(duration));
                }
                else
                {
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
            StartCoroutine(ChangeMaterialDensity(mediumRippleSpeed, mediumWaveSpeed, mediumRippleDensity));
        }
    }

}
