using System;
using UnityEngine;
using UI;

namespace Player
{
    public class PlayerCamera : MonoBehaviour
    {
        private Camera _camera;

        [SerializeField] private Transform _player;
        [SerializeField] private Vector3 _offset = new Vector3(0, 5, -10);
        [SerializeField] private float mouseSensitivity = 3f;
        // Only do horizonal rotation
        // [SerializeField] private float verticalClamp = 80f;

        [SerializeField] private bool _canRotate = true;

        private float _yaw = 0f;
        // private float _pitch = 20f;

        public AnimationCurve curve;

        private void Start()
        {
            if (!_canRotate) return;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnEnable()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (_player == null || _camera == null) return;

            if (_canRotate)
            {
                if (PauseManager.IsPaused || DefeatScreenManager.Instance.IsDefeat() || SuccessScreenManager.Instance.IsSuccess())
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    return;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
                _yaw += mouseX;
            }

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

        public void SetYaw(float newYaw)
        {
            _yaw = newYaw;
        }

        public void SetControlEnabled(bool enabled)
        {
            _canRotate = enabled;
        }

    }
}
