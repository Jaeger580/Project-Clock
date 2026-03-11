using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalyDetector : MonoBehaviour
{
    [SerializeField] private float radiusToCheck;
    private LayerMask anomalyLayer;
    private bool anomalyHit = false;

    private void Start()
    {
        anomalyLayer = LayerMask.GetMask("Anomaly");
        StartCoroutine(Finish(1f));
    }

    // Start Report
    private void Report()
    {
        //Physics.OverlapSphere(transform.position, radiusToCheck, );
    }

    public bool AnomalyFound()
    {
        return anomalyHit;
    }

    private IEnumerator Finish(float life)
    {
        yield return new WaitForSeconds(life);
        Report();
        Destroy(this.gameObject);
    }
}
