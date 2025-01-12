using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hazard
{
    public class FanRotate : MonoBehaviour
    {
        private Vector3 rotationSpeed;

        public void SetRotationSpeed(Vector3 speed)
        {
            rotationSpeed = speed;
        }

        public void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }
}