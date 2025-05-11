using System;
using UnityEngine;

namespace Player
{
    public class PlayerCamera : MonoBehaviour
    {
        private Camera _camera;

        [SerializeField] private Transform _player;
        [SerializeField] private Vector3 _offset = new Vector3(0, 5, -10);
        [SerializeField] private float mouseSensitivity = 3f;
        [SerializeField] private float verticalClamp = 80f;

        private float _yaw = 0f;
        private float _pitch = 20f;

        public AnimationCurve curve;

        private void OnEnable()
        {
            _camera = Camera.main;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (_player == null || _camera == null) return;

            // Only capture horizontal mouse movement
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

            // Only yaw (horizontal angle)
            _yaw += mouseX;

            // Build rotation only around Y axis
            Quaternion rotation = Quaternion.Euler(0, _yaw, 0);
            Vector3 targetPosition = _player.position + rotation * _offset;

            _camera.transform.position = targetPosition;
            _camera.transform.LookAt(_player.position + Vector3.up * 1.5f); // Aim slightly above for head focus
        }

        public void FindActiveCamera()
        {
            _camera = Camera.main;
            if (!_camera)
            {
                Debug.LogError("No camera found in the scene.");
            }
            else
            {
                Debug.Log("Camera found: " + _camera.name);
            }
        }
    }
}
