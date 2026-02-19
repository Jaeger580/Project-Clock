using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anomaly_Size : AnomalyHandler_Gradual
{
    [SerializeField] private Vector3 intendedScale = new();
    [SerializeField] private List<Transform> objsToChange = new();
    private Dictionary<Transform, Vector3> og_scales = new();

    override protected void Start()
    {
        base.Start();
        foreach (var obj in objsToChange)
        {
            og_scales.Add(obj, obj.localScale);
        }
    }

    public override void EnableAnomaly()
    {
        StartCoroutine(EnableAnomalyRoutine());
    }
    public override void DisableAnomaly()
    {

    }

    protected override IEnumerator EnableAnomalyRoutine()
    {
        float journey = 0f;

        while (journey <= duration)
        {
            journey += Time.deltaTime;

            foreach (var obj in objsToChange)
            {
                var newScale = Vector3.Lerp(og_scales[obj], intendedScale, gradualCurve.Evaluate(journey / duration));

                obj.localScale = newScale;
            }

            yield return null;
        }

        if (loopType == LoopType.REPEAT) StartCoroutine(EnableAnomalyRoutine());
        if (loopType != LoopType.PINGPONG) yield break;

        journey = 0f;

        while (journey <= duration)
        {
            journey += Time.deltaTime;

            foreach (var obj in objsToChange)
            {
                var newScale = Vector3.Lerp(og_scales[obj], intendedScale, gradualCurve.Evaluate(journey / duration));

                obj.localScale = newScale;
            }

            yield return null;
        }

        StartCoroutine(EnableAnomalyRoutine());
    }
}