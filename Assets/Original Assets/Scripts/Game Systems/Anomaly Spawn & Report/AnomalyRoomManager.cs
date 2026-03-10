using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnomalyRoomManager : MonoBehaviour
{//controls WHICH anomaly spawns
    //[SerializeField] protected AnomalyDataSet anomaliesInRoomOLD;
    [SerializeField] protected int roomID;
    public int RoomID => roomID;
    [SerializeField, ReadOnly] protected List<AnomalyHandler> anomaliesInRoom = new();
    public List<AnomalyHandler> AnomaliesInRoom => anomaliesInRoom;

    private bool playerInRoom;
    public bool PlayerInRoom => playerInRoom;
    [SerializeField] private LayerMask playerLayer;

    private void Start()
    {
        var anomalyCentralController = FindFirstObjectByType<AnomalyCentralController>();
        anomalyCentralController.SubscribeToController(this);
        AnomalyResolver.Instance.SubscribeToResolver(this);
    }

    public void SubscribeToManager(AnomalyHandler handler)
    {
        anomaliesInRoom.Add(handler);
    }

    public bool SpawnAnomaly(List<Tag> tagsToMatch, MatchType matchType = MatchType.ANY)
    {
        var validAnomalies = TagOperator.MatchQuery(tagsToMatch, anomaliesInRoom, matchType);

        AnomalyHandler pickedAnomaly = null;
        List<AnomalyHandler> validPool = new();

        bool TryTriggerPicked(AnomalyHandler picked)
        {
            if (picked == null) return false;
            picked.EnableAnomaly();
            picked.Data.OnAnomalyTriggered?.Invoke();
            return true;
        }

        foreach (var anomaly in validAnomalies)
        {//(re)populate tempPool
            validPool.Add(anomaly);
        }

        #region ROUND 1 : Unseen Valid Anomaly
        while (validPool.Count > 0)
        {//while there's stuff in the pool, try to pull a random anomaly
            var index = Random.Range(0, validPool.Count);

            if (!validPool[index].Data.previouslySeen)
            {//if it hasn't been seen before, pick it
                pickedAnomaly = validPool[index];
                break;
            }
            validPool.RemoveAt(index);
        }

        if (TryTriggerPicked(pickedAnomaly)) return true;

        #endregion
        #region ROUND 2 : Seen Valid Anomaly
        foreach (var anomaly in validAnomalies)
        {//(re)populate tempPool
            validPool.Add(anomaly);
        }
        if (validPool.Count > 0)
        {
            pickedAnomaly = validPool[Random.Range(0, validPool.Count)];
        }

        if (TryTriggerPicked(pickedAnomaly)) return true;

        #endregion
        #region ROUND 3 : Unseen Anomaly in Room
        int checkedCount = 0;
        var roomAnomalies = anomaliesInRoom;
        while (pickedAnomaly == null && checkedCount < roomAnomalies.Count)
        {//while I haven't picked an unseen anomaly and haven't iterated through the full list
            checkedCount++;
            int index = Random.Range(0, roomAnomalies.Count);
            if (!roomAnomalies[index].Data.previouslySeen)
                pickedAnomaly = roomAnomalies[index];
        }

        if (TryTriggerPicked(pickedAnomaly)) return true;

        #endregion
        #region ROUND 4 : Any Anomaly in Room

        if (roomAnomalies.Count > 0)
            pickedAnomaly = roomAnomalies[Random.Range(0, roomAnomalies.Count)];

        if (TryTriggerPicked(pickedAnomaly)) return true;

        #endregion

        Debug.LogError($"An anomaly was requested but no anomalies were found. Likely an empty list.", this);
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if((playerLayer.value & (1 << other.gameObject.layer)) > 0)
            playerInRoom = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if ((playerLayer.value & (1 << other.gameObject.layer)) > 0)
            playerInRoom = false;
    }
}