using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    //This script manages the spawning of the boss mechanic
    [Header("Settings")]
    [Space(10)]
    [SerializeField] private float distanceToSpawn;//Distance after which the boss will be spawned
    public float DistanceToSpawn => distanceToSpawn;
    [SerializeField] private float distanceToDespawn = 100f; // Distance after which the boss will be despawned
    public float DistanceToDespawn => distanceToDespawn;
    [SerializeField] private float spikeRoadSpawnBuffer = 500f;
    public float SpikeRoadSpawnBuffer => spikeRoadSpawnBuffer;


    [Header("Connections")]
    [Space(10)]
    [SerializeField] private DistanceManager distanceManager;
    public DistanceManager DistanceManager => distanceManager;
    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] private RoadSpawner roadSpawner;
    [SerializeField] private ObstacleConfig trafficConfig;
    [SerializeField] private LandSpawner landSpawner;
    [SerializeField] private SpawnManager spawnManager;

    private Transform playerT;
    private bool spawnBlockRoadCheck = true;
    [HideInInspector]
    public bool bossDefeated = false;
    [HideInInspector]
    public bool isBossActive = false;


    private void Start()
    {
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (!bossDefeated && !bossDefeated && distanceManager.virtualDistanceCovered > distanceToSpawn && spawnBlockRoadCheck && !obstacleSpawner.spawningConstrRoad)
        {
            StartBossPhase();
        }
        else
        if (!bossDefeated && isBossActive)
        {
            SpawnBossObstacles();
            CheckDespawnBoss();
        }

    }

    private void StartBossPhase()
    {
        obstacleSpawner.stopTraffic = true;
        obstacleSpawner.canSpawnConstrRoad = false;
        roadSpawner.SpawnBlockRoad(spawnBlockRoadCheck);
        spawnBlockRoadCheck = false;
        bossDefeated = false;
        isBossActive = true;
    }

    private void SpawnBossObstacles()
    {
        if (spawnManager.currentType == SpawnType.Resindential)
        {
            obstacleSpawner.SpawnBossResidential();
        }
        else if (spawnManager.currentType == SpawnType.City)
        {
            obstacleSpawner.SpawnBossCity();
        }
    }

    private void CheckDespawnBoss()
    {

        float distanceDuringBoss = distanceManager.virtualDistanceCovered - distanceToSpawn;
        if (distanceDuringBoss >= distanceToDespawn)
        {
            isBossActive = false;
            bossDefeated = true;
            spawnManager.ResetTransitionCounter();
            Invoke(nameof(ReEnableTrafficAndConstr), 5f);
            //Invoke(nameof(ReplaceSpikeRoads), 8f);
            spawnBlockRoadCheck = true;
        }

    }

    private void ReEnableTrafficAndConstr()
    {
        obstacleSpawner.stopTraffic = false;
        obstacleSpawner.canSpawnConstrRoad = true;
    }


}
