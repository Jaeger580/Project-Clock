using UnityEngine;
using UnityEngine.InputSystem;

public class AnomalyRayCheck : MonoBehaviour
{
    [SerializeField]
    private GameObject mainCam;
    private Transform camTransform;

    [SerializeField]
    private GameObject detectorPrefab;

    //private LayerMask layerMask;

    private void Start()
    {
        camTransform = mainCam.transform;
    }

    // Cast a ray and spawn a Dector object at the hit location.
    public void SpawnDetector(InputAction.CallbackContext context) 
    {
        if (context.started) 
        {
            RaycastHit hit;

            if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, Mathf.Infinity))
            {
                // Draw Ray
                Debug.DrawRay(camTransform.position, camTransform.forward * hit.distance, Color.red);

                // Spawn Detector
                Instantiate(detectorPrefab, hit.point, Quaternion.identity);
            }
            else
            {
                // Draw Ray
                Debug.DrawRay(camTransform.position, camTransform.forward * hit.distance, Color.red);
            }
        }
    }

}
