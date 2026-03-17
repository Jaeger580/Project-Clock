using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anomaly_Position : AnomalyHandler_Gradual
{
    [SerializeField] private Vector3 newLocalPosition = new();
    [SerializeField] private List<Transform> objsToMove = new();
    private Dictionary<Transform, Vector3> og_positions = new();

    override protected void Start()
    {
        base.Start();
        foreach (var obj in objsToMove)
        {
            og_positions.Add(obj, obj.localPosition);
        }
    }

    public override void EnableAnomaly()
    {
        if (anomalyEnabled) return;
        base.EnableAnomaly();
        StartCoroutine(EnableAnomalyRoutine());
    }

    public override void DisableAnomaly()
    {
        if (!anomalyEnabled) return;
        base.DisableAnomaly();
        StopAllCoroutines();
        foreach (var kvp in og_positions)
        {
            kvp.Key.localPosition = kvp.Value;
        }
    }

    protected override IEnumerator EnableAnomalyRoutine()
    {
        float journey = 0f;

        while (journey <= duration)
        {
            journey += Time.deltaTime;

            foreach (var obj in objsToMove)
            {
                var newPos = Vector3.Lerp(og_positions[obj], newLocalPosition, gradualCurve.Evaluate(journey / duration));
                obj.localPosition = newPos;
            }

            yield return null;
        }

        if (loopType == LoopType.REPEAT) StartCoroutine(EnableAnomalyRoutine());
        if (loopType != LoopType.PINGPONG) yield break;

        journey = duration;

        while (journey >= 0f)
        {
            journey -= Time.deltaTime;

            foreach (var obj in objsToMove)
            {
                var newPos = Vector3.Lerp(og_positions[obj], newLocalPosition, gradualCurve.Evaluate(journey / duration));
                obj.localPosition = newPos;
            }

            yield return null;
        }

        StartCoroutine(EnableAnomalyRoutine());
    }
}
