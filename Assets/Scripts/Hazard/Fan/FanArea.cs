// Author : William Alexander Tang Wai @ Jalapeno
// 11/01/2024 22:20

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hazard
{
    public class FanArea : MonoBehaviour
    {
        private Fan fan;

        public void Awake()
        {
            // The 'Fan' object is the parent of the 'FanArea' object.
            fan = transform.parent.gameObject.GetComponent<Fan>();
        }

        /**
         * When characters stay in the fanArea, call the push function
         * in the parent 'Fan' object.
         */
        public void OnTriggerStay(Collider collider)
        {
            fan.PushCharacters(collider);
        }
    }
}