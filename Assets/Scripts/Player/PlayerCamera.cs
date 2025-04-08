using System;
using UnityEngine;

namespace Player
{
    /**
     * Top-down camera for the player.
     */
    public class PlayerCamera : MonoBehaviour
    {
        //camera
        private Camera _camera;
        //player
        [SerializeField] private Transform _player;
        //camera offset
        [SerializeField] private Vector3 _offset = new Vector3(0, 10, -10);
        //camera rotation
        [SerializeField] private Vector3 _rotation = new Vector3(45, 0, 0);
        public AnimationCurve curve;
        // Start is called before the first frame update

        private void OnEnable()
        {
            //find a camera in the scene
            _camera = Camera.main;
        }

        // Update is called once per frame
        private void Update()
        {
            if (_player == null || _camera == null) return;
            {
                _camera.transform.position = _player.position + _offset;
                _camera.transform.rotation = Quaternion.Euler(_rotation);
            }
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