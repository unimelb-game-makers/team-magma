// Author : William Alexander Tang Wai @ Jalapeno
// 15/12/2024 13:09

using System.Collections.Generic;
using Damage;
using UnityEngine;

namespace Hazard
{
    public class SteamVentArea : MonoBehaviour
    {
        private SteamVent steamVent;

        /**
         * Stores the game objects currently in the steam vent area.
         */
        private List<GameObject> objectsInSteamVent = new();

        private float damageInterval;
        private float currentDamageInterval = 0;

        public void Awake()
        {
            // The 'SteamVent' object is the parent of the 'SteamVentArea' object.
            steamVent = transform.parent.gameObject.GetComponent<SteamVent>();

            damageInterval = steamVent.GetDamageInterval();
        }

        /**
         * Call the damage function in the parent 'SteamVent' object after
         * the damage interval has passed, on all objects currently in the steam
         * vent area once.
         */
        public void Update()
        {
            currentDamageInterval += Time.deltaTime;
            if (currentDamageInterval >= damageInterval)
            {
                currentDamageInterval = 0;

                foreach (GameObject obj in objectsInSteamVent)
                {
                    if (obj != null)
                    {
                        if(obj.GetComponent<Damageable>()!=null)
                        {
                            steamVent.DealDamage(obj.GetComponent<Damageable>());
                        }
                    }
                }
            }
        }

        /**
         * When a character enter the steamVentArea, add it to the list.
         */
        public void OnTriggerEnter(Collider collider)
        {
            objectsInSteamVent.Add(collider.gameObject);
            Debug.Log(objectsInSteamVent);
        }

        /**
         * When a character exit the steamVentArea, remove it from the list.
         */
        public void OnTriggerExit(Collider collider)
        {
            objectsInSteamVent.Remove(collider.gameObject);
        }
    }
}
