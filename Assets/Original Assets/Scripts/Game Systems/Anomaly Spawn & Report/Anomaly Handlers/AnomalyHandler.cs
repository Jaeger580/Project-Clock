using UnityEngine;

abstract public class AnomalyHandler : MonoBehaviour
{
    [SerializeField] protected AnomalyData data;

    protected void Start()
    {
        data.OnAnomalyTriggered += EnableAnomaly;
        data.OnAnomalyFixed += DisableAnomaly;
    }

    protected void OnDestroy()
    {
        data.OnAnomalyTriggered -= EnableAnomaly;
        data.OnAnomalyFixed -= DisableAnomaly;
    }

    abstract public void EnableAnomaly();
    abstract public void DisableAnomaly();
}
