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
        float destination = (totalSpawned + 1f) / anomalyTypeOrder.Count;  //next percentage of the anomalies that should be spawned
        float curvedPercent = spawnCurve.Evaluate(journey/totalNightTime);    //gives the ACTUAL percent that should've spawned at this point
        //destination = curvedPercent * totalNightTime;   //currently giving percent of the night that should be taken between spawns
        print($"Journey: {journey} || Destination: {destination}");

        while (curvedPercent <= destination && journey <= totalNightTime)
        {
            journey += Time.deltaTime;
            curvedPercent = spawnCurve.Evaluate(journey / totalNightTime);
            yield return null;
        }

        //while(nightTimer <= spawnCurve.Evaluate(totalSpawned + 1f / anomalyTypeOrder.Count) * totalNightTime)
        //{
        //    yield return null;
        //}


        /*
         * spawnPlotting = percent total of anomalies that should've spawned by percentage through the night (x-axis = percent through night)
         * curve = percentSpawned/percentTime
         * curve = currentSpawned/maxSpawned / currentTime/maxTime
         * curve = cS/mS * mT/cT
         * cT = cS/mS * mT/curve
         */
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
