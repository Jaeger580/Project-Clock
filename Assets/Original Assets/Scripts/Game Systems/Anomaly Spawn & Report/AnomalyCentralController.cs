using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AnomalyCentralController : MonoBehaviour
{//controls WHEN and WHERE anomalies spawn (needs access to player position prolly)

    static public AnomalyCentralController Instance;

    [SerializeField] private List<TagSearch> anomalyTypeOrder = new();
    private Queue<TagSearch> anomalyTypeQueue = new();
    private int totalSpawned;

    [SerializeField] private AnimationCurve spawnCurve;
    [SerializeField, ReadOnly] private List<AnomalyRoomManager> managers = new();

    private Timer timer;
    [Header("Loss Settings")]
    [Tooltip("Maximum number of anomalies allowed to stay active before the player dies.")]
    [SerializeField] private int maxAnomaliesAllowed = 4;
    [Tooltip("Grace period before the player dies while max anomalies are active.")]
    [SerializeField] private float graceAtMaxAnomalies = 20f;
    [Tooltip("Whether time still progresses while the maximum number of anomalies is active.")]
    [SerializeField] private bool timeProgressesAtMaxAnomalies = true;
    [Tooltip("If the total of currently spawned anomalies exceeds max, does the player die? (Also controls whether it can even exceed max in the first place.)")]
    [SerializeField] private bool overMaxKills = true;

    private bool gameOver;
    private float graceTimer;

    private int currentlySpawned;
    public int CurrentlySpawned
    {
        get
        {
            return currentlySpawned;
        }
        set
        {
            currentlySpawned = value;
            //print($"CURRENTLY SPAWNED: {currentlySpawned}"); 
            if (currentlySpawned < 0)
            {
                currentlySpawned = 0;
                print("ERR: Somehow you reduced the total by too many, this should never happen. Likely getting double-called?");
            }

            if (overMaxKills && currentlySpawned > maxAnomaliesAllowed) TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        StopAllCoroutines();
        gameOver = true;
    }

    private void Awake()
    {
        if (Instance != null) { Destroy(this); return; }

        Instance = this;
    }

    private void Start()
    {
        //managers.Clear();
        foreach (var anomalyType in anomalyTypeOrder)
            anomalyTypeQueue.Enqueue(anomalyType);

        timer = FindAnyObjectByType<Timer>();
        StartCoroutine(SpawnRoutine());
    }

    private void Update()
    {
        if (currentlySpawned != maxAnomaliesAllowed) return;

        graceTimer += Time.deltaTime;
        if (graceTimer >= graceAtMaxAnomalies)
            TriggerGameOver();
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
            if(!overMaxKills && !timeProgressesAtMaxAnomalies && currentlySpawned == maxAnomaliesAllowed)
            {//if I can't spawn more than max but time does progress while at max, and I *AM* at max, pause the spawner
                yield return null;
                continue;
            }

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
        print("Spawning!");
        graceTimer = 0f;
    }
}
