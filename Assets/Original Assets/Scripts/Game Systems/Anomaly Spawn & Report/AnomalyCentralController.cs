using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnomalyCentralController : MonoBehaviour
{//controls WHEN and WHERE anomalies spawn (needs access to player position prolly)
    static public AnomalyCentralController Instance;

    [SerializeField] private List<int> sceneBuildIndices = new();
    [SerializeField] private int maxLoadedRooms;
    [SerializeField] private bool randomizeRooms;

    [SerializeField] private List<TagSearch> anomalyTypeOrder = new();
    private Queue<TagSearch> anomalyTypeQueue = new();
    private int totalSpawned;
    [SerializeField] private Tag unseenTag;
    [SerializeField] private AnimationCurve spawnCurve;
    [SerializeField, ReadOnly] private List<AnomalyRoomManager> managers = new();
    public List<AnomalyRoomManager> Managers => managers;

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

    [SerializeField] private Transform playerTrans;

    private bool gameOver;
    private float graceTimer;

    [SerializeField, ReadOnly] private int currentlySpawned;

    private bool forcingSpeedUp = false;

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
        if (gameOver) return;
        StopAllCoroutines();
        gameOver = true;
        print("GAME OVER, TOO MANY ANOMALIES ACTIVE AT ONCE.");
        SceneManager.LoadScene("Loss");
    }

    private void Awake()
    {
        if (Instance != null) { Destroy(this); return; }

        Instance = this;

        int loadedRooms = 0;

        if (randomizeRooms)
        {//if I'm randomizing them,
            List<int> shuffledScenes = new();
            Dictionary<int, bool> enabledScenes = new();
            foreach (var scene in sceneBuildIndices)
            {
                shuffledScenes.Add(scene);          //add it to a list to shuffle later
                enabledScenes.Add(scene, false);    //add it to a dict for tracking enabled/disabled
            }
            shuffledScenes.Shuffle();

            foreach(var scene in shuffledScenes)
            {//for each scene that just got shuffled,
                if (loadedRooms >= maxLoadedRooms) break;   //if we've already loaded the max, stop

                enabledScenes[scene] = true;
                loadedRooms++;  //mark the scene for enabling it, and increment the number of loaded rooms
            }

            foreach(var scene in sceneBuildIndices)
            {//for all the rooms in the list, if it's supposed to be enabled, spawn it
                if (enabledScenes[scene]) SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            }
        }
        else
        {//if I'm not randomizing it, then
            foreach(var scene in sceneBuildIndices)
            {//for each scene in the list,
                if (loadedRooms >= maxLoadedRooms) break;   //if we've already loaded the max, stop

                SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                loadedRooms++;  //load the scene and add to the total
            }
        }
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
        if (gameOver) return;

        graceTimer += Time.deltaTime;
        if (graceTimer >= graceAtMaxAnomalies)
            TriggerGameOver();
    }

    public void SubscribeToController(AnomalyRoomManager manager)
    {
        managers.Add(manager);
    }

    [ContextMenu("Jump to Next Anomaly")]
    public void JumpToNextAnomaly()
    {
        if (!forcingSpeedUp) forcingSpeedUp = true;
        Time.timeScale = 100f;
    }

    private IEnumerator SpawnRoutine()
    {
        if (anomalyTypeQueue.Count <= 0) yield break;

        float totalShiftTime = timer.TotalShiftTime;
        float destination = (totalSpawned + 1f) / anomalyTypeOrder.Count;
        float curvedPercent = spawnCurve.Evaluate(timer.TotalElapsedTime / totalShiftTime);
        //print($"Journey: {journey} || Destination: {destination}");
        while (curvedPercent <= destination && timer.TotalElapsedTime <= totalShiftTime)
        {
            if(!overMaxKills && !timeProgressesAtMaxAnomalies && currentlySpawned == maxAnomaliesAllowed)
            {//if I can't spawn more than max but time does progress while at max, and I *AM* at max, pause the spawner
                yield return null;
                continue;
            }

            curvedPercent = spawnCurve.Evaluate(timer.TotalElapsedTime / totalShiftTime);
            yield return null;
        }

        if (forcingSpeedUp)
        {
            forcingSpeedUp = false;
            Time.timeScale = 1f;
        }
        TriggerAnomalySpawn();
        StartCoroutine(SpawnRoutine());
    }

    private void TriggerAnomalySpawn()
    {
        if (!anomalyTypeQueue.TryPeek(out var nextAnomalyType))
        {//Deliberately not dequeueing until it's confirmed
            Debug.LogError("No anomaly type found! Are you sure more anomalies are supposed to spawn?");
            return;
        }

        //print($"Next anomaly type should be {nextAnomalyType.name}");
        totalSpawned++;
        List<AnomalyRoomManager> shuffledRooms = new();
        foreach(var room in managers)
        {
            if (nextAnomalyType.items.Contains(unseenTag) &&
                (room.PlayerInRoom(playerTrans) ||
                (CameraManager.instance.PlayerInCams && CameraManager.instance.CamIndex == room.CamIndex)))
            {
                print($"DEBUG: Unseen tag being used, but player can currently see into (or is too close to/within) {room.HumanReadableName()}, so that room will be ignored.");
                continue;
            }
            //^if it's supposed to be unseen and the player is in the room or in this room's cam, drop that room from the list
            shuffledRooms.Add(room);
        }
        shuffledRooms.Shuffle();

        bool spawnComplete = false;
        int index = 0;
        while (!spawnComplete && index < shuffledRooms.Count)
        {//if we spawn something, OR we check every room and nothing spawns, then stop
            spawnComplete = shuffledRooms[index].SpawnAnomaly(nextAnomalyType.items, nextAnomalyType.searchType);
            index++;
        }
        //print("Spawning!");

        graceTimer = 0f;

        if (!spawnComplete)
        {
            float retryTimer = 1f;
            print($"DEBUG: SPAWN INCOMPLETE - trying again in {retryTimer} second(s).");
            Invoke(nameof(TriggerAnomalySpawn), retryTimer);
        }
        else anomalyTypeQueue.TryDequeue(out var _);
    }
}
