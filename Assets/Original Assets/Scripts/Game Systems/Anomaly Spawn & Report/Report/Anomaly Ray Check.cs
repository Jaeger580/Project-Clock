using UnityEngine;
using UnityEngine.InputSystem;

public class AnomalyRayCheck : MonoBehaviour
{
    [SerializeField]
    private GameObject mainCam;
    private Transform camTransform;

    [SerializeField]
    private AnomalyTagger detectorPrefab;
    [SerializeField, ReadOnly] public Tag anomalyType;

    private float cooldownTimer;

    private void Start()
    {
        camTransform = mainCam.transform;
    }

    private void Update()
    {
        if (cooldownTimer <= 0) return;
        cooldownTimer -= Time.deltaTime;
    }

    // Cast a ray and spawn a Dector object at the hit location.
    public void SpawnDetector(InputAction.CallbackContext context) 
    {
        if (AnomalyTagChooser.FreeMouse) return;    //TODO: Replace with action map switching later, this is quick and dirty
        if (cooldownTimer > 0) return;
        if (context.started) 
        {
            RaycastHit hit;

            if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, Mathf.Infinity))
            {
                // Draw Ray
                Debug.DrawRay(camTransform.position, camTransform.forward * hit.distance, Color.red);

                // Spawn Detector
                var tagger = Instantiate(detectorPrefab, hit.point, Quaternion.identity);
                tagger.SetTags(anomalyType);
            }
            else
            {
                // Draw Ray
                Debug.DrawRay(camTransform.position, camTransform.forward * hit.distance, Color.red);
            }
        }
        cooldownTimer = AnomalyResolver.Instance.DetectorSpawnCooldown;
    }

}
