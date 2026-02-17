using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Anomaly Data - ", menuName = "Anomalies/Anomaly Data", order = 0)]
public class AnomalyData : ScriptableObject, ITagged
{
    public List<Tag> tags = new();
    public List<Tag> Tags() => tags;

    public delegate void AnomalyEvent();
    public AnomalyEvent OnAnomalyTriggered, OnAnomalyFixed;

    public bool previouslySeen = false;
}
