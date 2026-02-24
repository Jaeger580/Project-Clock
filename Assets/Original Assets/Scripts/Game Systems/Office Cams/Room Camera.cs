using UnityEngine;
//using System.Collections.Generic;

public class RoomCamera : MonoBehaviour
{
    [SerializeField]
    private Camera thisCam;



    // Camera Registers itself to the camera manager in a simple manner.
    // Needs to be called after Awake to make sure the Camera Manager is ready.
    private void Start()
    {
        if (CameraManager.instance != null)
        {
            CameraManager.instance.AddCamera(thisCam);
        }
        else 
        {
            Debug.Log("No Manager");
        }
    }

    private void OnDestroy()
    {
        if (CameraManager.instance != null)
        {
            CameraManager.instance.RemoveCamera(thisCam);
        }
    }
}