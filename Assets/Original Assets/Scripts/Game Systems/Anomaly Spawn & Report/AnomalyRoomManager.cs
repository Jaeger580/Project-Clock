using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnomalyRoomManager : MonoBehaviour, DebugTools.IDebug_Name
{//controls WHICH anomaly spawns
    //[SerializeField] protected AnomalyDataSet anomaliesInRoomOLD;
    [SerializeField] protected int roomID;
    public int RoomID => roomID;

    [SerializeField] protected CinemachineCamera roomCam;
    private int camIndex = -1;
    public int CamIndex => camIndex;
    [SerializeField, ReadOnly] protected List<AnomalyHandler> anomaliesInRoom = new();
    public List<AnomalyHandler> AnomaliesInRoom => anomaliesInRoom;

    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private Collider[] roomCheckColliders;

    [Header("DEBUG")]
    [Tooltip("ONLY USED FOR DEBUGGING PURPOSES")]
    [SerializeField] protected string humanReadableName = "[DEBUG NAME NOT SET]";
    public string HumanReadableName() => humanReadableName;

    //[ContextMenu("UPDATESTUFF")]
    //public void PlayerLayerFix()
    //{
    //    playerLayer.value = LayerMask.GetMask("Player");
    //    UnityEditor.EditorUtility.SetDirty(this);
    //}

    public bool PlayerInRoom(Transform player)
    {
        if (player == null) return false;
        bool inRoom = false;
        foreach(var col in roomCheckColliders)
        {
            if (!col.bounds.Contains(player.position)) continue;
            inRoom = true;
        }
        return inRoom;
    }

    private void Start()
    {
        //var anomalyCentralController = FindFirstObjectByType<AnomalyCentralController>();
        var anomalyCentralController = AnomalyCentralController.Instance;
        anomalyCentralController.SubscribeToController(this);
        AnomalyResolver.Instance.SubscribeToResolver(this);

        if (CameraManager.instance != null)
        {
            camIndex = CameraManager.instance.AddCamera(roomCam);
        }
        else
        {
            Debug.Log("No Manager");
        }

        roomCheckColliders = GetComponents<Collider>();
        foreach(var col in roomCheckColliders)
        {
            if (!col.isTrigger)
            {
                Debug.LogWarning("WARNING: A room collider wasn't set to trigger - fixing for runtime only.", this);
                col.isTrigger = true;
            }
        }
    }

    private void OnDestroy()
    {
        if (CameraManager.instance != null)
        {
            CameraManager.instance.RemoveCamera(roomCam);
        }
    }

    public void SubscribeToManager(AnomalyHandler handler)
    {
        anomaliesInRoom.Add(handler);
    }

    public bool SpawnAnomaly(List<Tag> tagsToMatch, MatchType matchType = MatchType.ANY, bool mustMatch = false)
    {
        var validAnomalies = TagOperator.MatchQuery(tagsToMatch, anomaliesInRoom, matchType);

        List<AnomalyHandler> validPool = new();

        bool TryTriggerPicked(AnomalyHandler picked)
        {
            if (picked == null) return false;
            if (picked.AnomalyEnabled) return false;
            picked.EnableAnomaly();
            //picked.Data.OnAnomalyTriggered?.Invoke();
            return true;
        }

        foreach (var anomaly in validAnomalies)
        {//(re)populate tempPool
            validPool.Add(anomaly);
        }

        validPool.Shuffle();

        #region ROUND 1 : Unseen Valid Anomaly

        foreach(var option in validPool)
        {//go down the shuffled list until you hit one you haven't seen before
            if (option.Data.previouslySeen) continue;
            if (TryTriggerPicked(option)) return true;
        }

        #endregion
        #region ROUND 2 : Seen Valid Anomaly

        foreach (var option in validPool)
        {//go down the shuffled list until you find a valid, not-already-spawned one
            if (TryTriggerPicked(option)) return true;
        }

        #endregion

        if (mustMatch) return false;    //if we got here without an anomaly, then try a different room

        #region ROUND 3 : Unseen Anomaly in Room
        //expand the search to ANY anomaly, not just valid anomalies
        validPool.Clear();
        foreach (var anomaly in anomaliesInRoom)
        {//(re)populate temp pool
            validPool.Add(anomaly);
        }
        validPool.Shuffle();

        foreach (var option in validPool)
        {//go down the shuffled list until you hit one you haven't seen before
            if (option.Data.previouslySeen) continue;
            if (TryTriggerPicked(option)) return true;
        }

        #endregion
        #region ROUND 4 : Any Anomaly in Room

        foreach (var option in validPool)
        {//go down the shuffled list until you find one that isn't currently spawned
            if (TryTriggerPicked(option)) return true;
        }

        #endregion

        Debug.LogWarning($"An anomaly was requested but no anomalies were found. Likely an empty list.", this);
        return false;
    }
}