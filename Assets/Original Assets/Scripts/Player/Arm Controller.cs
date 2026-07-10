using UnityEngine;
using UnityEngine.InputSystem;

public class ArmController : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    // Toggle the arms raised / lower state, to move the report tool.
    public void AdjustArm(InputAction.CallbackContext context) 
    {
        if (context.started) 
        {
            anim.SetTrigger("Alt Fire");
        }
    }
}
