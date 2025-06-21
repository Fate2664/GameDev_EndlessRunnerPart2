using UnityEngine;
using UnityEngine.Events;

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

    private bool spawnBlockRoadCheck = true;
    [HideInInspector]
    public bool bossDefeated = false;
    [HideInInspector]
    public bool isBossActive = false;

  



    private void Start()
    {
       distanceManager.BossDistanceReached.AddListener(StartBossPhase);
       distanceManager.BossExitDistanceReached.AddListener(CheckDespawnBoss);
    }


    private void StartBossPhase()
    {
        //if (!obstacleSpawner.spawningConstrRoad)
        {
            obstacleSpawner.stopTraffic = true;
            obstacleSpawner.canSpawnConstrRoad = false;
            roadSpawner.SpawnBlockRoad(spawnBlockRoadCheck);
            spawnBlockRoadCheck = false;
            bossDefeated = false;
            isBossActive = true; 
        }
    }

    public void SpawnBossObstacles()
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
