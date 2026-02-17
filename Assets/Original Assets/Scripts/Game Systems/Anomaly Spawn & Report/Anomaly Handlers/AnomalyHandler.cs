using System.Collections.Generic;
using UnityEngine;

abstract public class AnomalyHandler : MonoBehaviour, ITagged
{
    [SerializeField] protected AnomalyData data;
    public AnomalyData Data => data;
    public List<Tag> Tags() => data.Tags();

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
