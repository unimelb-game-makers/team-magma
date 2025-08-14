// Author : William Alexander Tang Wai @ Jalapeno
// 12/01/2025 18:10

using System.Collections;
using Utilities.ServiceLocator;
using UnityEngine;
using Tempo;
using System.Collections.Generic;

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

        private static readonly int RippleSpeedID = Shader.PropertyToID("_rippleSpeed"),
                                    WaveSpeedID = Shader.PropertyToID("_waveSpeed"),
                                    RippleDensityID = Shader.PropertyToID("_rippleDensity");

        [SerializeField] private Renderer _waterRenderer;
        private Material _waterMaterial;
        private Coroutine _waterCoroutine;

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

            _waterMaterial = _waterRenderer.material;
        }

        public void Start()
        {
            ServiceLocator.Instance.Register<ISyncable>(this);
        }

        private IEnumerator MoveWaterLevelToHeight(float targetWaterHeight)
        {
            // Get all floatable objects
            GameObject[] floatableItems = GameObject.FindGameObjectsWithTag("Floatable");
            
            // Store original Y offsets from current water surface
            Dictionary<Transform, float> objectOffsets = new Dictionary<Transform, float>();
            Vector3 killAreaStartPos = killArea.transform.position;
            float currentWaterHeight = killAreaStartPos.y;
            
            foreach (GameObject item in floatableItems)
            {
                // Calculate each object's current offset from water surface
                float offset = item.transform.position.y - currentWaterHeight;
                objectOffsets.Add(item.transform, offset);
            }
            
            float elapsedTime = 0f;
            float heightDifference = targetWaterHeight - currentWaterHeight;

            while (elapsedTime < _duration)
            {
                float progress = elapsedTime / _duration;
                float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
                float currentHeight = Mathf.Lerp(currentWaterHeight, targetWaterHeight, easedProgress);
                
                // Move kill area (water surface)
                killArea.transform.position = new Vector3(
                    killAreaStartPos.x,
                    currentHeight,
                    killAreaStartPos.z
                );
                
                // Move floating items maintaining their natural offsets
                foreach (var item in objectOffsets)
                {
                    Transform objTransform = item.Key;
                    float naturalOffset = item.Value;
                    
                    // Calculate new position maintaining offset from moving surface
                    Vector3 newPos = objTransform.position;
                    newPos.y = currentHeight + naturalOffset;
                    objTransform.position = newPos;
                }
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Finalize positions
            killArea.transform.position = new Vector3(
                killAreaStartPos.x,
                targetWaterHeight,
                killAreaStartPos.z
            );
        }

        private void ChangeMaterialDensity(float targetRippleSpeed, float targetWaveSpeed, float targetRippleDensity)
        {
            _waterMaterial.SetFloat(RippleSpeedID, targetRippleSpeed);
            _waterMaterial.SetFloat(WaveSpeedID, targetWaveSpeed);
            _waterMaterial.SetFloat(RippleDensityID, targetRippleDensity);
        }
        
        /**
         * Move the 'KillArea' depending on the TempoMode.
         */
        public override void Affect(TempoMode mode)
        {
            switch (mode)
            {
                case TempoMode.Slow:
                    if (_waterCoroutine != null)
                        StopCoroutine(_waterCoroutine);
                    _waterCoroutine = StartCoroutine(MoveWaterLevelToHeight(height1));
                    ChangeMaterialDensity(slowRippleSpeed, slowWaveSpeed, slowRippleDensity);
                    break;
                case TempoMode.Fast:
                    if (_waterCoroutine != null)
                        StopCoroutine(_waterCoroutine);
                    _waterCoroutine = StartCoroutine(MoveWaterLevelToHeight(height3));
                    ChangeMaterialDensity(fastRippleSpeed, fastWaveSpeed, fastRippleDensity);
                    break;
                case TempoMode.Default:
                    if (_waterCoroutine != null)
                        StopCoroutine(_waterCoroutine);
                    _waterCoroutine = StartCoroutine(MoveWaterLevelToHeight(height2));
                    ChangeMaterialDensity(mediumRippleSpeed, mediumWaveSpeed, mediumRippleDensity);
                    break;
            }
        }
    }

}
