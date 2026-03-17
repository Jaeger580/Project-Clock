using System.Collections.Generic;
using UnityEngine;

public class Anomaly_Composite : AnomalyHandler
{
    [SerializeField] private List<AnomalyHandler> underlyingHandlers = new();

    private void Awake()
    {
        foreach (var handler in underlyingHandlers)
            handler.SetData(data);
    }

    public override void EnableAnomaly()
    {
        if (anomalyEnabled) return;
        base.EnableAnomaly();
        foreach (var handler in underlyingHandlers)
            handler.EnableAnomaly();
    }

    public override void DisableAnomaly()
    {
        if (!anomalyEnabled) return;
        base.DisableAnomaly();
        foreach (var handler in underlyingHandlers)
            handler.DisableAnomaly();
    }
}
