using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField]
    private CharacterController playerController;

    [SerializeField]
    private PlayerInput playerInput;

    [SerializeField]
    private GameObject playerCamera;

    [SerializeField]
    private float sensitivity = 1;

    private InputAction lookAction;
    private float pitch;
    private float yaw;

    [SerializeField]
    private GameObject flashLight;

    private void Awake()
    {
        lookAction = playerInput.actions.FindAction("Look");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (AnomalyTagChooser.FreeMouse) return;    //TODO: Replace with action map switching later, this is quick and dirty

        MouseLook();
    }

    private void MouseLook() 
    {
        Vector2 input = lookAction.ReadValue<Vector2>();
        pitch -= input.y * sensitivity;
        yaw = input.x * sensitivity;


        // Clamp pitch to avoid issues and sign flips at +/- 90
        pitch = Mathf.Clamp(pitch, -89, 89);

        // Vertical Rotation
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // Horizontal Rotation
        transform.Rotate(Vector3.up * yaw);
    }

    // Toggles the players flashlight on and off
    // might not make the most sense in this script, but don't want a new one for this.
    public void ToggleFlashlight() 
    {
        if (flashLight.activeSelf == true)
        {
            flashLight.SetActive(false);
        }
        else 
        {
            flashLight.SetActive(true);
        }
    }
}
