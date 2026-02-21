using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.LightTransport.PostProcessing;
using System;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour, IInteractable
{
    private List<Camera> roomCameras = new List<Camera>();
    // Allows the camera objects to look for and call this script
    public static CameraManager instance;

    private int camIndex = 0;

    [SerializeField]
    private PlayerInput PlayerInput;

    private void Awake()
    {
        roomCameras.Clear();
        instance = this;
    }

    public void Interact(object interactor) 
    {
        EnterCamera();
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
    public void EnterCamera()
    {
        camIndex = 0;

        // Disable current camera
        roomCameras[camIndex].depth = 2;

        // Change player's input actions
        PlayerInput.SwitchCurrentActionMap("Cameras");
    }

    // Toggles which camera is active
    public void SwitchCamRight(InputAction.CallbackContext context) 
    {
        if (context.performed) 
        {

            if (camIndex < roomCameras.Count - 1)
            {
                // Disable current camera
                roomCameras[camIndex].depth = 0;

                // Enable new camera
                roomCameras[camIndex + 1].depth = 2;
                camIndex++;
            }
            else
            {
                // Disable current camera
                roomCameras[camIndex].depth = 0;

                // Change player's input actions
                PlayerInput.SwitchCurrentActionMap("Player");
            }
        }
    }

    public void SwitchCamLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            if (camIndex > 0)
            {
                // Disable current camera
                roomCameras[camIndex].depth = 0;

                // Enable new camera
                roomCameras[camIndex - 1].depth = 2;
                camIndex--;
            }
            else
            {
                // Disable current camera
                roomCameras[camIndex].depth = 0;

                // Change player's input actions
                PlayerInput.SwitchCurrentActionMap("Player");
            }
        }
    }
}
