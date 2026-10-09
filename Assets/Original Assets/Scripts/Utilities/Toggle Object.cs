using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleObject : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToToggle;

    public void ToggleTheObject(InputAction.CallbackContext context)
    {
        if (context.started) 
        {
            if (objectToToggle.activeSelf)
            {
                objectToToggle.SetActive(false);
            }
            else
            {
                objectToToggle.SetActive(true);
            }
        }
    }
}
