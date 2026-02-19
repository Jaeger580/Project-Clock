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

    private void Awake()
    {
        lookAction = playerInput.actions.FindAction("Look");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        MouseLook();
    }

    private void MouseLook() 
    {
        Vector2 input = lookAction.ReadValue<Vector2>();
        pitch -= input.y * sensitivity * Time.deltaTime;
        yaw = input.x * sensitivity;


        // Clamp pitch to avoid issues and sign flips at +/- 90
        pitch = Mathf.Clamp(pitch, -89, 89);

        // Vertical Rotation
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // Horizontal Rotation
        transform.Rotate(Vector3.up * yaw * Time.deltaTime);
    }
}
