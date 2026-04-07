using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anomaly_Position : AnomalyHandler_Gradual
{
    [SerializeField] private Vector3 newLocalPosition = new();
    [SerializeField] private bool treatAsOffset = false;
    [SerializeField] private bool ignoreAtNegativeThousand = false;
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
                Vector3 newPos;
                var og_pos = og_positions[obj];
                if (!treatAsOffset)
                {
                    newPos = Vector3.Lerp(og_pos, newLocalPosition, gradualCurve.Evaluate(journey / duration));
                }
                else
                {
                    newPos = og_pos + Vector3.Lerp(Vector3.zero, newLocalPosition, gradualCurve.Evaluate(journey/duration));
                }

                if (ignoreAtNegativeThousand)
                {
                    newPos.x = newLocalPosition.x == -1000f ? og_pos.x : newPos.x;
                    newPos.y = newLocalPosition.y == -1000f ? og_pos.y : newPos.y;
                    newPos.z = newLocalPosition.z == -1000f ? og_pos.z : newPos.z;
                }

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
                Vector3 newPos;
                var og_pos = og_positions[obj];

                if (!treatAsOffset)
                {
                    newPos = Vector3.Lerp(og_pos, newLocalPosition, gradualCurve.Evaluate(journey / duration));
                }
                else
                {
                    newPos = og_pos + Vector3.Lerp(Vector3.zero, newLocalPosition, gradualCurve.Evaluate(journey / duration));
                }

                if (ignoreAtNegativeThousand)
                {
                    newPos.x = newLocalPosition.x == -1000f ? og_pos.x : newPos.x;
                    newPos.y = newLocalPosition.y == -1000f ? og_pos.y : newPos.y;
                    newPos.z = newLocalPosition.z == -1000f ? og_pos.z : newPos.z;
                }

                obj.localPosition = newPos;
            }

            yield return null;
        }

        StartCoroutine(EnableAnomalyRoutine());
    }
}