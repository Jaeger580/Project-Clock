using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public void ReturnToMenu() 
    {
        SceneManager.LoadScene("Main Menu");
    }
}
