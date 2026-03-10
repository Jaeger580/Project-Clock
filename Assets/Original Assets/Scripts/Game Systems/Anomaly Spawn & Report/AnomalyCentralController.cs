using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AnomalyCentralController : MonoBehaviour
{//controls WHEN and WHERE anomalies spawn (needs access to player position prolly)
    [SerializeField] private List<TagSearch> anomalyTypeOrder = new();
    private Queue<TagSearch> anomalyTypeQueue = new();
    private int totalSpawned;

    [SerializeField] private AnimationCurve spawnCurve;
    [SerializeField, ReadOnly] private List<AnomalyRoomManager> managers = new();

    private Timer timer;

    private void Start()
    {
        //managers.Clear();
        foreach (var anomalyType in anomalyTypeOrder)
            anomalyTypeQueue.Enqueue(anomalyType);

        StartCoroutine(SpawnRoutine());

        timer = FindAnyObjectByType<Timer>();
    }

    public void SubscribeToController(AnomalyRoomManager manager)
    {
        managers.Add(manager);
    }

    private IEnumerator SpawnRoutine()
    {
        if (anomalyTypeQueue.Count <= 0) yield break;

        float journey = timer.TotalElapsedTime;
        float totalShiftTime = timer.TotalShiftTime;
        float destination = (totalSpawned + 1f) / anomalyTypeOrder.Count;
        float curvedPercent = spawnCurve.Evaluate(journey/totalShiftTime);
        print($"Journey: {journey} || Destination: {destination}");

        while (curvedPercent <= destination && journey <= totalShiftTime)
        {
            journey += Time.deltaTime;
            curvedPercent = spawnCurve.Evaluate(journey / totalShiftTime);
            yield return null;
        }

        TriggerAnomalySpawn();
        StartCoroutine(SpawnRoutine());
    }

    private void TriggerAnomalySpawn()
    {
        if (!anomalyTypeQueue.TryDequeue(out var nextAnomalyType))
        {
            Debug.LogError("No anomaly type found! Are you sure more anomalies are supposed to spawn?");
            return;
        }

        print($"Next anomaly type should be {nextAnomalyType.name}");
        totalSpawned++;

        List<AnomalyRoomManager> shuffledRooms = new();
        foreach(var room in managers)
        {
            if (room.PlayerInRoom) continue;
            shuffledRooms.Add(room);
        }
        shuffledRooms.Shuffle();

        bool spawnComplete = false;
        int index = 0;
        while (!spawnComplete && index < shuffledRooms.Count)
        {
            spawnComplete = shuffledRooms[index].SpawnAnomaly(nextAnomalyType.items, nextAnomalyType.searchType);
            index++;
        }
    }
}
