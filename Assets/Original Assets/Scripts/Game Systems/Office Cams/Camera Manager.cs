using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
//using UnityEngine.LightTransport.PostProcessing;
using System;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour, IInteractable
{
    private List<CinemachineCamera> roomCameras = new List<CinemachineCamera>();
    // Allows the camera objects to look for and call this script
    public static CameraManager instance;

    private int camIndex = 0;

    [SerializeField]
    private PlayerInput PlayerInput;

    [SerializeField]
    private GameObject playerHUD;
    [SerializeField]
    private GameObject cameraHUD;

    private void Awake()
    {
        roomCameras.Clear();
        instance = this;
    }

    public void Interact(object interactor) 
    {
        EnterCamera();
    }

    public void AddCamera(CinemachineCamera roomCam) 
    {
        roomCameras.Add(roomCam);
    }

    public void RemoveCamera(CinemachineCamera roomCam)
    {
        roomCameras.Remove(roomCam);
    }

    // Toggles which camera is active
    public void EnterCamera()
    {
        camIndex = 0;

        playerHUD.SetActive(false);
        cameraHUD.SetActive(true);


        //roomCameras[camIndex].depth = 2;
        var nextCam = roomCameras[camIndex];
        nextCam.enabled = true;

        // Change player's input actions
        PlayerInput.SwitchCurrentActionMap("Cameras");
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void ExitCamera() 
    {
        // Disable current camera

        foreach(var cam in roomCameras)
        {
            cam.enabled = false;
        }
        
        // Change player's input actions
        PlayerInput.SwitchCurrentActionMap("Player");

        cameraHUD.SetActive(false);
        playerHUD.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Toggles which camera is active
    public void SwitchCamRight(InputAction.CallbackContext context) 
    {
        if (context.performed) 
        {
            if (camIndex < roomCameras.Count - 1)
            {
                var previousCam = roomCameras[camIndex];
                var nextCam = roomCameras[++camIndex];

                previousCam.enabled = false;
                nextCam.enabled = true;

            }
            else
            {
                ExitCamera();
            }
        }
    }

    public void SwitchCamLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (camIndex > 0)
            {
                var previousCam = roomCameras[camIndex];
                var nextCam = roomCameras[--camIndex];

                previousCam.enabled = false;
                nextCam.enabled = true;

            }
            else
            {
                ExitCamera();
            }
        }
    }

    public void SelectCam(int camDex)
    {
        var previousCam = roomCameras[camIndex];
        var nextCam = roomCameras[camDex];

        previousCam.enabled = false;
        nextCam.enabled = true;
    }
}
