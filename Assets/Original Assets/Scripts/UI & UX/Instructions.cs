using UnityEngine;
using UnityEngine.InputSystem;

public class Instructions : MonoBehaviour
{
    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private GameObject player;
    private PlayerInput playInput;

    private void Start()
    {
        playInput = player.GetComponent<PlayerInput>();
    }

    public void TogglePause() 
    {
        if (canvas.activeSelf)
        {
            playInput.SwitchCurrentActionMap("Player");
            canvas.SetActive(false);
            Time.timeScale = 1.0f;
        }
        else 
        {
            playInput.SwitchCurrentActionMap("UI");
            canvas.SetActive(true);
            Time.timeScale = 0.0f;
        }
    }
}
