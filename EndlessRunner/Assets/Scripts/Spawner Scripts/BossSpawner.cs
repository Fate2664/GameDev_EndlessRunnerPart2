using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    //This script manages the spawning of the boss mechanic
    [Header("Settings")]
    [Space(10)]
    [SerializeField] private float distanceToSpawn;//Distance after which the boss will be spawned
    public float DistanceToSpawn => distanceToSpawn;
    [SerializeField] private float distanceToDespawn = 100f; // Distance after which the boss will be despawned


    [Header("Connections")]
    [Space(10)]
    [SerializeField] private DistanceManager distanceManager;
    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] private RoadSpawner roadSpawner;
    [SerializeField] private ObstacleConfig trafficConfig;
    [SerializeField] private LandSpawner landSpawner;
    [SerializeField] private SpawnManager spawnManager;

    private Transform playerT;
    private bool spawnBlockRoadCheck = true;
    [HideInInspector]
    public bool bossDefeated = false;


    private void Start()
    {
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (!bossDefeated && distanceManager.virtualDistanceCovered > distanceToSpawn && spawnBlockRoadCheck)
        {
            Debug.Log("Spawning Boss");
            SpawnBoss();
        }
        else
        if (!bossDefeated)
        {
            CheckDespawnBoss();
        }

    }

    //This method calls the spawning of the road block and the new obstacles
    private void SpawnBoss()
    {
        obstacleSpawner.stopTraffic = true;
        obstacleSpawner.canSpawnConstrRoad = false;
        roadSpawner.SpawnBlockRoad(spawnBlockRoadCheck);
        if (spawnManager.currentType == SpawnType.Resindential)
        {
            Invoke(nameof(SpawnCityBoss), 5f);
        }
        else if (spawnManager.currentType == SpawnType.City)
        {
            Invoke(nameof(SpawnCityBoss), 5f);
        }
        spawnBlockRoadCheck = false;
        bossDefeated = false;

    }

    private void CheckDespawnBoss()
    {

        float distanceDuringBoss = distanceManager.virtualDistanceCovered - distanceToSpawn;
        if (distanceDuringBoss >= distanceToDespawn)
        {
            bossDefeated = true;
            spawnManager.ResetTransitionCounter();
            Invoke(nameof(ReEnableTrafficAndConstr), 5f);
            Invoke(nameof(roadSpawner.ReplaceSpikeRoads), 5f);
            spawnBlockRoadCheck = true;
        }

    }

    private void SpawnResidentialBoss()
    {
        obstacleSpawner.SpawnBossResidential();
    }

    private void SpawnCityBoss()
    {
        obstacleSpawner.SpawnBossCity();
    }

    private void ReEnableTrafficAndConstr()
    {
        obstacleSpawner.stopTraffic = false;
        obstacleSpawner.canSpawnConstrRoad = true;
    }
}
