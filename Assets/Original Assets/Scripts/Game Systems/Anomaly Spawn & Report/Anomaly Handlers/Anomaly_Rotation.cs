using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anomaly_Rotation : AnomalyHandler_Gradual
{
    [SerializeField] private Vector3 newLocalRotation = new();
    [SerializeField] private List<Transform> objsToMove = new();
    private Dictionary<Transform, Vector3> og_rotations = new();

    override protected void Start()
    {
        base.Start();
        foreach (var obj in objsToMove)
        {
            og_rotations.Add(obj, obj.localEulerAngles);
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
        foreach (var kvp in og_rotations)
        {
            kvp.Key.localEulerAngles = kvp.Value;
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
                var newRot = Vector3.Lerp(og_rotations[obj], newLocalRotation, gradualCurve.Evaluate(journey / duration));
                obj.localEulerAngles = newRot;
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
                var newRot = Vector3.Lerp(og_rotations[obj], newLocalRotation, gradualCurve.Evaluate(journey / duration));
                obj.localEulerAngles = newRot;
            }

            yield return null;
        }

        StartCoroutine(EnableAnomalyRoutine());
    }
}
