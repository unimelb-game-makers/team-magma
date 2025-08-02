using System;
using System.Collections;
using System.Collections.Generic;   
using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
[ExecuteInEditMode]
public class VirtualCamera : MonoBehaviour
{
    //whether the camera is shouldbe initially active 
    [SerializeField] private bool initialActive = false;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField, HideInInspector] private Vector3 cameraPosition = new Vector3(0, 0, 0);
    private Vector3 CameraPosition
    {
        get { return cameraPosition; }
        set
        {
            cameraPosition = value;
            virtualCamera.transform.position = value;
        }
    }
    [SerializeField] private Transform playerTarget;
    [SerializeField, HideInInspector] private Vector3 playerPosition = new Vector3(0, 0, 0);
    private Vector3 PlayerPosition
    {
        get { return playerPosition; }
        set
        {
            playerPosition = value;
            playerTarget.transform.position = value;
        }
    }
    [Tooltip("changes the displacement from camera to player")]

    [SerializeField] Vector3 followOffset = new Vector3(0, 0, 0);
    [SerializeField, HideInInspector] Vector3 previousOffset = new Vector3(0, 0, 0);

    private Vector3 FollowOffset
    {
        get { return followOffset; }
        set
        {
            followOffset = value;
            previousOffset = value;
        }
    }

    [Tooltip("changes the distance from camera to player")]

    [SerializeField] public float distance;
    [SerializeField, HideInInspector] private float Distance
    {
        get { return distance; }
        set
        {
            distance = value;
            previousDistance = value;
        }
    }
    private float previousDistance;
    private Transform player;



    //change the values on the inspector will reflect on the change in camera 
    void OnValidate()
    {

        if (previousOffset != followOffset)
        {
            previousOffset = followOffset;
            Distance = FollowOffset.magnitude;
            CameraPosition = PlayerPosition + FollowOffset;
        }
        if (distance != previousDistance)
        {
            previousDistance = distance;
            FollowOffset = FollowOffset.normalized * distance;
            CameraPosition = PlayerPosition + FollowOffset;
        }


    }   
    private void Update()
    {
#if UNITY_EDITOR
        //change the position of player or camera in editor will change the distance and displacement

        if (!Application.isPlaying)
        {

            if (playerTarget == null)
            {
                Debug.Log("player not assigned");
                return;
            }
            if (playerPosition != playerTarget.transform.position)
            {
                playerPosition = playerTarget.transform.position;
                Debug.Log("player changed");
                FollowOffset = cameraPosition - playerPosition;
                Distance = FollowOffset.magnitude;

            }
            if (cameraPosition != virtualCamera.transform.position)
            {
                cameraPosition = virtualCamera.transform.position;
                Debug.Log("camera changed");

                FollowOffset = cameraPosition - playerPosition;
                Distance = FollowOffset.magnitude;

            }
        }
#endif

    }


    void Start()
    {
        if (Application.isPlaying)
        {

            player = GameManager.Instance.PlayerCharacter.transform;
            playerTarget.gameObject.SetActive(false);
            CameraManager.Instance.register(virtualCamera, initialActive);
        }
    }

    void LateUpdate()
    {   if (Application.isPlaying)
        {
        virtualCamera.transform.position = player.position + followOffset;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (virtualCamera != null)
        {
            CameraManager.Instance.enableCamera(virtualCamera);
        }
    }
    

}
