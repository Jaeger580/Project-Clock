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

    public List<Tag> Tags()
    {
        return data ? data.Tags() : parentData.Tags();
    }
    protected AnomalyData parentData;

    virtual protected void Start()
    {
        if(!transform.parent.TryGetComponent(out AnomalyRoomManager roomManager))
        {
            print("ERR: Anomaly isn't childed under a room manager, and will be ignored.");
            return;
        }
        if (data == null) return;
        roomManager.SubscribeToManager(this);
        //data.OnAnomalyTriggered += EnableAnomaly;
        data.OnAnomalyTriggered += () => {  AnomalyCentralController.Instance.CurrentlySpawned++; };
        data.OnAnomalyFixed += () => { data.previouslySeen = true; AnomalyCentralController.Instance.CurrentlySpawned--; };
    }

    virtual protected void OnDestroy()
    {
        if (data == null) return;
        data.OnAnomalyTriggered = null;
        data.OnAnomalyFixed = null;
    }

    virtual public void EnableAnomaly()
    {
        anomalyEnabled = true;
        if (data == null) return;
        data.OnAnomalyTriggered?.Invoke();
    }
    virtual public void DisableAnomaly()
    {
        anomalyEnabled = false;
        if (data == null) return;
        data.OnAnomalyFixed?.Invoke();
    }

    public void SetData(AnomalyData parentData)
    {
        this.parentData = parentData;
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
