using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CameraManager : MonoBehaviour
{
    private List<CinemachineVirtualCamera> cameras;
    private CinemachineVirtualCamera initialCamera; 
    private int activePriority = 20;
    private int inactivePriority = 10;
    public static CameraManager Instance;
    
    void Awake()
    {
        Instance = this;
        cameras = new List<CinemachineVirtualCamera>();
    }
   
    public void Register(CinemachineVirtualCamera camera, bool initialActive)
    {
        cameras.Add(camera);
        camera.Priority = initialActive ? activePriority : inactivePriority;
        if (initialActive)
        {
            initialCamera = camera;
        }
        else
        {
            Debug.LogWarning($"Multiple cameras have initialActive = true! Pls deactivate Camera");
        }
    }

    public void Delete(CinemachineVirtualCamera camera)
    {
        cameras.Remove(camera);
    }

    public void DisableAllCamera()
    {
        foreach (CinemachineVirtualCamera camera in cameras)
        {
            camera.Priority = inactivePriority;
        }
    }

    public void EnableCamera(CinemachineVirtualCamera camera)
    {
        DisableAllCamera();
        camera.Priority = activePriority;
    }
    public void ResetCamera()
    {
        EnableCamera(initialCamera);
        
    }
}
