using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadMainScene() 
    {
        SceneManager.LoadScene("Main Game");
    }

    public void Exit() 
    {
        Application.Quit();
        Debug.Log("Application should have closed.");
    }
}