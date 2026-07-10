using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoopType { NONE, REPEAT, PINGPONG}

abstract public class AnomalyHandler : MonoBehaviour, ITagged, DebugTools.IDebug_Name
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

    //[Header("DEBUG")]
    //[Tooltip("ONLY USED FOR DEBUGGING PURPOSES")]
    //[SerializeField] protected string humanReadableName = "[DEBUG NAME NOT SET]";
    public string HumanReadableName() => data.name;
    private string roomName = "[DEBUG NAME NOT SET]";
    private AnomalyRoomManager roomManager;

    virtual protected void Start()
    {
        if(!transform.parent.TryGetComponent(out roomManager))
        {
            print("ERR: Anomaly isn't childed under a room manager, and will be ignored.");
            return;
        }
        if (data == null) return;
        roomManager.SubscribeToManager(this);
        roomName = roomManager.HumanReadableName();
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

    [ContextMenu("Enable Anomaly")]
    virtual public bool TryEnableAnomaly()
    {
        //print($"TRYING TO ENABLE AN ANOMALY | should: {data.tags.Contains(AnomalyCentralController.Instance.UnseenTag)} | is: {AnomalyCentralController.Instance.PlayerSeesSpecificRoom(roomManager)} ");
        if (AnomalyCentralController.Instance.PlayerSeesSpecificRoom(roomManager) &&
            data.tags.Contains(AnomalyCentralController.Instance.UnseenTag))
        {
            print($"DEBUG: {HumanReadableName()} has been rerolled in {roomName} since it should go unseen.");
            return false;
        }
        EnableAnomaly();
        return true;
    }

    virtual public void EnableAnomaly()
    {
        anomalyEnabled = true;
        if (data == null) return;
        data.OnAnomalyTriggered?.Invoke();
        print($"DEBUG: {HumanReadableName()} has been spawned in {roomName}.");
    }

    [ContextMenu("Disable Anomaly")]
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
    [Min(0.00001f)]
    [SerializeField] protected float duration;
    [SerializeField] protected LoopType loopType;
    abstract protected IEnumerator EnableAnomalyRoutine();
}
