using UnityEngine;

namespace Player
{
    /**
     * Top-down camera for the player.
     */
    public class PlayerCamera : MonoBehaviour
    {
        //camera
        [SerializeField] private Camera _camera;
        //player
        [SerializeField] private Transform _player;
        //camera offset
        [SerializeField] private Vector3 _offset = new Vector3(0, 10, -10);
        //camera rotation
        [SerializeField] private Vector3 _rotation = new Vector3(45, 0, 0);
        // Start is called before the first frame update
        
        private void Start()
        {
            
        }
        
        // Update is called once per frame
        private void Update()
        {
            _camera.transform.position = _player.position + _offset;
            _camera.transform.rotation = Quaternion.Euler(_rotation);
        }
        
    }
}