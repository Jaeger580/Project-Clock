using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalyDetector : MonoBehaviour
{
    private bool anomalyHit = false;

    private void Start()
    {
        StartCoroutine(Finish(1f));
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if we collided with an anomaly
        // Not sure if Itagged interface is a good way to check for anomaly, but I assume it would work.
        if (other.TryGetComponent<ITagged>(out ITagged component)) 
        {
            anomalyHit=true;

            Debug.Log("Anomaly Found!");

            // Need to actually get the anomaly and return infor somewhere somehow?
        }
        else 
        {
            Debug.Log("No Anomaly!");
        }

    }

    // Start Report
    private void Report()
    {
        // Open Report UI stuff and Pass it the anomaly info? IDK
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
