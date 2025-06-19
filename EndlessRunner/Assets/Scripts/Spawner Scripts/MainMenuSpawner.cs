using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class MainMenuObstacleSpawner : MonoBehaviour
{
    //This script controls the spawning of all the obstacles during the main menu scene

    [Header("Spawn Details")]
    [Range(0.5f, 10f)]
    [SerializeField] private float trafficSpawnRate = 1f;
    [Header("Obstacles")]
    [SerializeField] private List<GameObject> obsTraffic;

    private float counterTraffic = 0f;
    private Transform spawnerTransform;
    [HideInInspector]
    public LaneManager laneManager;
    private readonly float[] lanePositions = { -40f, 0f, 40f };

    private void Start()
    {
        laneManager = new LaneManager(lanePositions);
        spawnerTransform = GameObject.FindGameObjectWithTag("TrafficSpawner").transform;
        counterTraffic = Time.deltaTime;
        //Spawn the initial traffic bunch
        if (counterTraffic < trafficSpawnRate)
        {
            counterTraffic = 0f;
        }
    }

    void Update()
    {
        SpawnTraffic();
    }

    //This method controls the spawning of the traffic
    private void SpawnTraffic()
    {
        counterTraffic += Time.deltaTime;

        if (counterTraffic >= trafficSpawnRate)
        {
            ObsTrafficSpawn();
            counterTraffic = 0f;
        }
    }

    //This method controls the spawning of the traffic
    private void ObsTrafficSpawn()
    {

        int numTraffToSpawn = 1;
        List<int> occupiedLanes = new List<int>();
        GameObject prefab = obsTraffic[Random.Range(0, obsTraffic.Count)];
        ObstacleLink link = prefab.GetComponent<ObstacleLink>();
        ObstacleConfig config = link.obsConfig;

        laneManager.ResetLanes();


        while (occupiedLanes.Count < numTraffToSpawn)
        {
            int laneIndex = Random.Range(0, laneManager.laneCount);
            if (!occupiedLanes.Contains(laneIndex))
            {
                occupiedLanes.Add(laneIndex);
                laneManager.OccupyLane(laneIndex);

                //Get the settings for spawning the traffic
                float spawnZ = spawnerTransform.position.z;
                Vector3 spawnPos = new Vector3(laneManager.GetLaneX(laneIndex), config.spawnOffset.y, spawnZ);
                Quaternion rotation = config.faceBackward ? prefab.transform.rotation : Quaternion.identity;

                //Spawn the traffic prefab
                GameObject spawnedTraffic = Instantiate(prefab, spawnPos, rotation);
                spawnedTraffic.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;

                //If the prefab is faced backwards give it an index of three
                if (config.faceBackward)
                {
                    spawnedTraffic.GetComponent<MovingObstacle>().obstacleIndex = 3;
                }
                spawnedTraffic.transform.SetParent(transform, this);

                Destroy(spawnedTraffic, config.lifespan);
            }
        }

    }


}







