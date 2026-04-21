using UnityEngine;

public class Instructions : MonoBehaviour
{
    [SerializeField]
    private GameObject canvas;


    public void TogglePause() 
    {
        if (canvas.activeSelf)
        {
            canvas.SetActive(false);
            Time.timeScale = 1.0f;
        }
        else 
        {
            canvas.SetActive(true);
            Time.timeScale = 0.0f;
        }
    }
}
