using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    private float currentMulti;

    //[SerializeField]
    //private float jumpStrength = 5;

    [SerializeField]
    private float gravityValue = -9.81f;

    private InputAction moveAction;
    private InputAction sprintAction;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource sfxSource;
    //[SerializeField] private AudioClip[] sfxJump;
    [SerializeField]
    private AudioClip footSteps;
    private bool playWalkSound = false;

    private void Awake()
    {
        moveAction = playerInput.actions.FindAction("Move");
        sprintAction = playerInput.actions.FindAction("Sprint");
    }

    private void Start()
    {
        StartCoroutine(WalkSounds());
    }

    private void Update()
    {
        if (AnomalyTagChooser.FreeMouse) return;    //TODO: Replace with action map switching later, this is quick and dirty

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

        // If there is no input, don't play sounds
        if (moveDirection == Vector2.zero)
        {
            playWalkSound = false;
        }
        else 
        {
            playWalkSound = true;
        }

        Vector3 moveVector = transform.right * moveDirection.x + transform.forward * moveDirection.y;
        moveVector = Vector3.ClampMagnitude(moveVector, 1f);

        //Jump

        // Apply Gravity
        playerVelocity.y += gravityValue;

        // Apply Sprint
        Sprint();

        Vector3 finalMove = moveVector * baseSpeed * currentMulti + Vector3.up * playerVelocity.y;
        playerController.Move(finalMove * Time.deltaTime);
    }

    private void Sprint() 
    {
        if (sprintAction.IsPressed()) 
        {
            currentMulti = sprintMulti;
        }
        else 
        {
            currentMulti = 1;
        }
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    // Handles walking and sprinting sound effects
    private IEnumerator WalkSounds()
    {
        while (true) 
        {
            if (playWalkSound && !sfxSource.isPlaying)
            {
                sfxSource.Play();
            }
            else if(!playWalkSound && sfxSource.isPlaying)
            {
                sfxSource.Pause();
            }
            yield return null;
        }
    }
}
