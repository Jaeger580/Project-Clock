using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController playerController;

    [SerializeField]
    private PlayerInput playerInput;

    private Vector3 playerVelocity;

    [SerializeField]
    private float baseSpeed = 5;

    [SerializeField]
    private float sprintMulti = 1;

    [SerializeField]
    private float jumpStrength = 5;

    [SerializeField]
    private float gravityValue = -9.81f;

    private InputAction moveAction;

    private void Awake()
    {
        moveAction = playerInput.actions.FindAction("Move");
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        // Ground Check
        bool touchingGround = playerController.isGrounded;

        if (touchingGround)
        {
            // Slight downward velocity to keep grounded stable
            if (playerVelocity.y < -2f)
                playerVelocity.y = -2f;
        }

        // Read Input
        Vector2 moveDirection = moveAction.ReadValue<Vector2>();
        Vector3 moveVector = transform.right * moveDirection.x + transform.forward * moveDirection.y;
        moveVector = Vector3.ClampMagnitude(moveVector, 1f);

        //Jump

        // Apply Gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        Vector3 finalMove = moveVector * baseSpeed + Vector3.up * playerVelocity.y;
        playerController.Move(finalMove * Time.deltaTime);
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }
}
