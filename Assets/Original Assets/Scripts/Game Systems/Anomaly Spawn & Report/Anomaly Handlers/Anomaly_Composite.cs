using System.Collections.Generic;
using UnityEngine;

public class Anomaly_Composite : AnomalyHandler
{
    [SerializeField] private List<AnomalyHandler> underlyingHandlers = new();

    public override void EnableAnomaly()
    {
        if (anomalyEnabled) return;

        foreach (var handler in underlyingHandlers)
            handler.EnableAnomaly();
    }

    public override void DisableAnomaly()
    {
        if (!anomalyEnabled) return;

        foreach (var handler in underlyingHandlers)
            handler.DisableAnomaly();
    }
}
