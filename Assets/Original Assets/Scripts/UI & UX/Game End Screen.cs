using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndScreen : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void Restart() 
    {
        SceneManager.LoadScene("Main Game");
    }

    public void Quit() 
    {
        Application.Quit();
    }
}
