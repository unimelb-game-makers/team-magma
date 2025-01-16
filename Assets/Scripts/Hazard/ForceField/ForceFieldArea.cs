// Author : William Alexander Tang Wai @ Jalapeno
// 15/12/2024 13:09

using UnityEngine;

namespace Hazard
{
    public class ForceFieldArea : MonoBehaviour
    {
        private ForceField forceField;

        public void Awake()
        {
            // The 'ForceField' object is the parent of the 'ForceFieldArea' object.
            forceField = transform.parent.gameObject.GetComponent<ForceField>();
        }

        /**
         * When characters stay in the forceFieldArea, call the push function
         * in the parent 'ForceField' object.
         */
        public void OnTriggerStay(Collider collider)
        {
            forceField.PushCharacters(collider);
        }
    }
}