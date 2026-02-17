using System.Collections.Generic;
using UnityEngine;

public class Anomaly_Composite : AnomalyHandler
{
    [SerializeField] private List<AnomalyHandler> underlyingHandlers = new();

    public override void EnableAnomaly()
    {
        foreach (var handler in underlyingHandlers)
            handler.EnableAnomaly();
    }

    public override void DisableAnomaly()
    {
        foreach (var handler in underlyingHandlers)
            handler.DisableAnomaly();
    }
}
