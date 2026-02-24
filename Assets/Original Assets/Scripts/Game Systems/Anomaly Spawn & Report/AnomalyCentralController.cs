using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnomalyCentralController : MonoBehaviour
{//controls WHEN and WHERE anomalies spawn (needs access to player position prolly)
    [SerializeField] private List<TagSearch> anomalyTypeOrder = new();
    private Queue<TagSearch> anomalyTypeQueue = new();
    private int totalSpawned;

    [Tooltip("Total seconds the night should take to complete.")]
    [SerializeField] private float totalNightTime = 600f;
    private float nightTimer;
    private bool nightComplete;
    [SerializeField] private AnimationCurve spawnCurve;
    [SerializeField, ReadOnly] private List<AnomalyRoomManager> managers = new();

    private void Start()
    {
        managers.Clear();
        foreach (var anomalyType in anomalyTypeOrder)
            anomalyTypeQueue.Enqueue(anomalyType);

        StartCoroutine(SpawnRoutine());
    }

    public void SubscribeToController(AnomalyRoomManager manager)
    {
        managers.Add(manager);
    }

    private void Update()
    {
        if (nightComplete) return;
        nightTimer += Time.deltaTime;
        if (nightTimer >= totalNightTime)
        {
            nightComplete = true;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        if (anomalyTypeQueue.Count <= 0) yield break;

        float journey = nightTimer;
        float destination = (totalSpawned + 1f) / anomalyTypeOrder.Count;
        float curvedPercent = spawnCurve.Evaluate(journey/totalNightTime);
        print($"Journey: {journey} || Destination: {destination}");

        while (curvedPercent <= destination && journey <= totalNightTime)
        {
            journey += Time.deltaTime;
            curvedPercent = spawnCurve.Evaluate(journey / totalNightTime);
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
        //TODO: randomly pick a room the player isn't in
        //var randomRoom = managers[Random.Range(0, managers.Count)];
        
        //randomRoom.SpawnAnomaly(nextAnomalyType.items, nextAnomalyType.searchType);
    }
}
