using System;
using System.Collections;
using System.Collections.Generic;   
using Cinemachine;
using UnityEngine;

public class VirtualCamera : MonoBehaviour
{
    // Start is called before the first frame update

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineTransposer transposer;
    [SerializeField] private bool initialActive = false;
    [SerializeField] Vector3 followOffset = new Vector3(0, 0, 0);
    private Transform player;
    void Awake()
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.Log("no virtual camera found");
            Destroy(gameObject);
            return;
        }

    }

    void Start()
    {
        player = GameManager.Instance.PlayerCharacter.transform;
        CameraManager.Instance.register(virtualCamera, initialActive);
    }

    void LateUpdate()
    {   
        virtualCamera.transform.position = player.position + followOffset;

    }

    void OnTriggerEnter(Collider other)
    {
        if (virtualCamera != null)
        {
            CameraManager.Instance.enableCamera(virtualCamera);
        }
    }
}
