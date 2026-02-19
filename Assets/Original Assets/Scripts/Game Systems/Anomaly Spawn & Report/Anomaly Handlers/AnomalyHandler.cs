using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoopType { NONE, REPEAT, PINGPONG}

abstract public class AnomalyHandler : MonoBehaviour, ITagged
{
    [SerializeField] protected AnomalyData data;
    public AnomalyData Data => data;
    public List<Tag> Tags() => data.Tags();

    virtual protected void Start()
    {
        data.OnAnomalyTriggered += EnableAnomaly;
        data.OnAnomalyFixed += DisableAnomaly;
    }

    virtual protected void OnDestroy()
    {
        data.OnAnomalyTriggered -= EnableAnomaly;
        data.OnAnomalyFixed -= DisableAnomaly;
    }

    abstract public void EnableAnomaly();
    abstract public void DisableAnomaly();
}

abstract public class AnomalyHandler_Gradual : AnomalyHandler
{
    [SerializeField] protected AnimationCurve gradualCurve;
    [Tooltip("Duration before its gradual effect either ends or, if selected, loops.")]
    [SerializeField] protected float duration;
    [SerializeField] protected LoopType loopType;
    abstract protected IEnumerator EnableAnomalyRoutine();
}
