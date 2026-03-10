using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoopType { NONE, REPEAT, PINGPONG}

abstract public class AnomalyHandler : MonoBehaviour, ITagged
{
    [SerializeField] protected AnomalyData data;
    public AnomalyData Data => data;

    [SerializeField] protected bool anomalyEnabled;
    public bool AnomalyEnabled => anomalyEnabled;

    public List<Tag> Tags() => data.Tags();

    virtual protected void Start()
    {
        if(!transform.parent.TryGetComponent(out AnomalyRoomManager roomManager))
        {
            print("ERR: Anomaly isn't childed under a room manager, and will be ignored.");
            return;
        }

        roomManager.SubscribeToManager(this);
        //data.OnAnomalyTriggered += EnableAnomaly;
        data.OnAnomalyTriggered += () => { anomalyEnabled = true; };
        data.OnAnomalyFixed += () => { data.previouslySeen = true; };
    }

    virtual protected void OnDestroy()
    {
        data.OnAnomalyTriggered = null;
        data.OnAnomalyFixed = null;
    }

    virtual public void EnableAnomaly()
    {
        data.OnAnomalyTriggered?.Invoke();
    }
    virtual public void DisableAnomaly()
    {
        data.OnAnomalyFixed?.Invoke();
    }
}

abstract public class AnomalyHandler_Gradual : AnomalyHandler
{
    [SerializeField] protected AnimationCurve gradualCurve;
    [Tooltip("Duration before its gradual effect either ends or, if selected, loops.")]
    [SerializeField] protected float duration;
    [SerializeField] protected LoopType loopType;
    abstract protected IEnumerator EnableAnomalyRoutine();
}
