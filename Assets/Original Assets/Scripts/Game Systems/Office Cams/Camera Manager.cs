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

    private bool playerInCams = false;
    public bool PlayerInCams => playerInCams;

    private int camIndex = 0;
    public int CamIndex => camIndex;

    [SerializeField]
    private PlayerInput PlayerInput;

    [SerializeField]
    private GameObject playerHUD;
    [SerializeField]
    private GameObject cameraHUD;

    [SerializeField] private GameEvent goToCamEvent;

    private void Awake()
    {
        roomCameras.Clear();
        if (instance != null)
            Destroy(instance);
        instance = this;
    }

    public void Interact(object interactor) 
    {
        EnterCamera();
    }


    public int AddCamera(CinemachineCamera roomCam) 
    {
        roomCameras.Add(roomCam);
        return roomCameras.IndexOf(roomCam);
        //Debug.Log("Camera Added");
    }

    public void RemoveCamera(CinemachineCamera roomCam)
    {
        roomCameras.Remove(roomCam);
    }

    // Toggles which camera is active
    public void EnterCamera()
    {
        playerInCams = true;
        camIndex = 0;

        playerHUD.SetActive(false);
        cameraHUD.SetActive(true);

        //roomCameras[camIndex].depth = 2;
        SelectCam(camIndex);

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
        playerInCams = false;
    }

    // Toggles which camera is active
    public void SwitchCamRight(InputAction.CallbackContext context) 
    {
        if (context.performed) 
        {
            if (camIndex < roomCameras.Count - 1)
            {
                SelectCam(++camIndex);
            }
            //else
            //{
            //    ExitCamera();
            //}
        }
    }

    public void SwitchCamLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (camIndex > 0)
            {
                SelectCam(--camIndex);
            }
            //else
            //{
            //    ExitCamera();
            //}
        }
    }

    public void SelectCam(int camDex)
    {
        camIndex = camDex;
        roomCameras[camDex].enabled = true;
        foreach (var cam in roomCameras) if (cam != roomCameras[camDex]) cam.enabled = false;
        goToCamEvent?.Trigger(camDex);
    }

    public void ForceExit(InputAction.CallbackContext context) 
    {
        if (context.started) 
        {
            ExitCamera();
        }
    }
}
