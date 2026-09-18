using UnityEngine;
using UnityEngine.InputSystem;

// Script for pausing the game and opening pause menus

public class PauseUIController : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;
    [SerializeField]
    PlayerInput playerInput;

    private bool isPaused = false;

    public void TogglePause() 
    {
        if (!isPaused) 
        {
            playerInput.SwitchCurrentActionMap("UI");
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            pauseMenu.SetActive(true);
            Time.timeScale = 0.0f;
            isPaused = true;

            
        }
        else 
        {
            playerInput.SwitchCurrentActionMap("Player");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            pauseMenu.SetActive(false);
            Time.timeScale = 1.0f;
            isPaused=false;

            
        }
    }
}
