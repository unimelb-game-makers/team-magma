using System;
using System.Collections;
using Damage;
using Platforms;
using UnityEngine;
using Utilities.ServiceLocator;

namespace Hazard.Electric_Fence
{
    public class ElectricFence : Hazard
    {
        [SerializeField] private GameObject door;
        [SerializeField] private DamageVolume damageVolume;
        [SerializeField] private float doorSpeed = 5f;
        [SerializeField] private float normalDoorSpeed = 5f;
        [SerializeField] private float fastDoorSpeed = 2.5f;
        [SerializeField] private float slowDoorSpeed = 10f;
        
        private Coroutine doorCoroutine;
        
        private void OnEnable()
        {
            if(ServiceLocator.Instance != null)
                ServiceLocator.Instance.Register(this);
        }
        
        private void OnDisable()
        {
            if(ServiceLocator.Instance != null)
                ServiceLocator.Instance.Unregister(this);
        }
        
        private void Start()
        {
            doorSpeed = normalDoorSpeed;
            doorCoroutine = StartCoroutine(DoorRepeat());
        }
        
        public override void Affect(TapeType tapeType, float duration, float effectValue)
        {
            switch (tapeType)
            {
                case TapeType.Slow:
                    StartCoroutine(FastTempo(duration));
                    break;
                case TapeType.Fast:
                    StartCoroutine(SlowTempo(duration));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tapeType), tapeType, null);
            }
        }
        
        private IEnumerator FastTempo(float duration)
        {
            doorSpeed = fastDoorSpeed;
            // Restart the door cycle if already running.
            if (doorCoroutine != null)
                StopCoroutine(doorCoroutine);
            doorCoroutine = StartCoroutine(DoorRepeat());
            yield return new WaitForSeconds(duration);
            doorSpeed = normalDoorSpeed;
        }
        
        private IEnumerator SlowTempo(float duration)
        {
            doorSpeed = slowDoorSpeed;
            yield return new WaitForSeconds(duration);
            doorSpeed = normalDoorSpeed;
        }
        
        /// <summary>
        /// Cycles the electric fence on and off using a continuous loop rather than recursion.
        /// </summary>
        private IEnumerator DoorRepeat()
        {
            while (true)
            {
                Activate();
                yield return new WaitForSeconds(doorSpeed);
                Deactivate();
                yield return new WaitForSeconds(doorSpeed);
            }
        }
        
        private void Activate()
        {
            Debug.Log("Activating Electric Fence");
            door.SetActive(true);
            damageVolume.Activate();
        }
        
        private void Deactivate()
        {
            Debug.Log("Deactivating Electric Fence");
            door.SetActive(false);
            damageVolume.Deactivate();
        }
    }
}
