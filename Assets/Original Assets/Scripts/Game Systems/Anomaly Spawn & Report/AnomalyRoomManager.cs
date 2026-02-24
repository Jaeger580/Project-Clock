using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnomalyRoomManager : MonoBehaviour
{//controls WHICH anomaly spawns
    [SerializeField] protected AnomalyDataSet anomaliesInRoom;

    private void Start()
    {
        var anomalyCentralController = FindFirstObjectByType<AnomalyCentralController>();
        anomalyCentralController.SubscribeToController(this);
    }

    public bool SpawnAnomaly(List<Tag> tagsToMatch, MatchType matchType = MatchType.ANY)
    {
        var validAnomalies = TagOperator.MatchQuery(tagsToMatch, anomaliesInRoom.items, matchType);

        AnomalyData pickedAnomaly = null;
        List<AnomalyData> validPool = new();

        foreach (var anomaly in validAnomalies)
        {//(re)populate tempPool
            validPool.Add(anomaly);
        }

        #region ROUND 1 : Unseen Valid Anomaly
        while (validPool.Count > 0)
        {//while there's stuff in the pool, try to pull a random anomaly
            var index = Random.Range(0, validPool.Count);

            if (!validPool[index].previouslySeen)
            {//if it hasn't been seen before, pick it
                pickedAnomaly = validPool[index];
                break;
            }
            validPool.RemoveAt(index);
        }

        if (pickedAnomaly != null)
        {//if we picked an anomaly, trigger it
            pickedAnomaly.OnAnomalyTriggered?.Invoke();
            return true;
        }
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

        if (pickedAnomaly != null)
        {//if we picked an anomaly, trigger it
            pickedAnomaly.OnAnomalyTriggered?.Invoke();
            return true;
        }
        #endregion
        #region ROUND 3 : Unseen Anomaly in Room
        int checkedCount = 0;
        var roomAnomalies = anomaliesInRoom.items;
        while (pickedAnomaly == null && checkedCount < roomAnomalies.Count)
        {//while I haven't picked an unseen anomaly and haven't iterated through the full list
            checkedCount++;
            int index = Random.Range(0, roomAnomalies.Count);
            if (!roomAnomalies[index].previouslySeen)
                pickedAnomaly = roomAnomalies[index];
        }
        if (pickedAnomaly != null)
        {//if we picked an anomaly, trigger it
            pickedAnomaly.OnAnomalyTriggered?.Invoke();
            return true;
        }

        #endregion
        #region ROUND 4 : Any Anomaly in Room

        if (roomAnomalies.Count > 0)
            pickedAnomaly = roomAnomalies[Random.Range(0, roomAnomalies.Count)];

        if (pickedAnomaly != null)
        {//if we picked an anomaly, trigger it
            pickedAnomaly.OnAnomalyTriggered?.Invoke();
            return true;
        }
        #endregion

        Debug.LogError($"An anomaly was requested but no anomalies were found. Likely an empty list.", this);
        return false;
    }
}