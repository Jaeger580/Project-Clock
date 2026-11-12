using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    private PauseUIController pauseUIController;

    public void ReturnToMenu() 
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Main Menu");
    }
}