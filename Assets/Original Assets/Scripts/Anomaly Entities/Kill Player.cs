using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    // If this collides with the player, kill the player

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.name);

        if (collision.transform.tag.Equals("Player")) 
        {
            var accInst = AnomalyCentralController.Instance;

            accInst.TriggerAltEnd();

        }
    }
}