using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.LightTransport.PostProcessing;
using System;

public class CameraManager : MonoBehaviour, IInteractable
{
    private List<Camera> roomCameras = new List<Camera>();
    // Allows the camera objects to look for and call this script
    public static CameraManager instance;

    private int camIndex = 0;

    private void Awake()
    {
        roomCameras.Clear();
        instance = this;
    }

    public void Interact(object interactor) 
    {
        SwitchCamera();
    }

    public void AddCamera(Camera roomCam) 
    {
        roomCameras.Add(roomCam);
        Debug.Log("Camera Added");
    }

    public void RemoveCamera(Camera roomCam)
    {
        roomCameras.Remove(roomCam);
    }

    // Toggles which camera is active
    public void SwitchCamera() 
    {
        Debug.Log(roomCameras.Count);
        roomCameras[camIndex].depth = 2;
    }
}
