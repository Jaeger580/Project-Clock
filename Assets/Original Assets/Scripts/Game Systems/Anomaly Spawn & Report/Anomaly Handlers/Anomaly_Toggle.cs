using System.Collections.Generic;
using UnityEngine;

public class Anomaly_Toggle : AnomalyHandler
{
    [SerializeField] private List<GameObject> objsToToggleOn, objsToToggleOff;

    public override void EnableAnomaly()
    {
        foreach (var obj in objsToToggleOn)
            obj.SetActive(true);
        foreach (var obj in objsToToggleOff)
            obj.SetActive(false);
    }
    public override void DisableAnomaly()
    {
        foreach (var obj in objsToToggleOn)
            obj.SetActive(false);
        foreach (var obj in objsToToggleOff)
            obj.SetActive(true);
    }
}
