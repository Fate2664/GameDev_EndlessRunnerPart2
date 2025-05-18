using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    //This script manages the spawning of the boss mechanic
    [Header("Settings")]
    [Space(10)]
    [SerializeField] private float distanceToSpawn;


    [Header("Connections")]
    [Space(10)]
    [SerializeField] private DistanceManager distanceManager;
    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] private RoadSpawner roadSpawner;
    [SerializeField] private ObstacleConfig trafficConfig;

    private Transform playerT;
    private bool spawnCheck = true;


    private void Start()
    {
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        SpawnBoss();
    }

    //This method calls the spawning of the road block and the new obstacles
    private void SpawnBoss()
    {
        if (distanceManager.distanceCovered > distanceToSpawn && !obstacleSpawner.spawningConstrRoad)
        {
            obstacleSpawner.stopTraffic = true;
            obstacleSpawner.canSpawnConstrRoad = false;
            //float blockRoadPos = playerT.position.z - trafficConfig.spawnOffset.z;
            roadSpawner.SpawnBlockRoad(spawnCheck);
            Invoke(nameof(SpawningObstacles), 5f);
            spawnCheck = false;
        }
    }

    private void SpawningObstacles()
    {
        obstacleSpawner.SpawnObstacle();
    }
}
