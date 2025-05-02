using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;


public class ObstacleSpawner : MonoBehaviour
{

    [Header("Spawn Details")]
    [UnityEngine.Range(1f, 10f)]
    [SerializeField] private float obsSpawneRate = 5f;
    [SerializeField] private float trafficSpawnRate = 2f;
    [SerializeField] private List<float> distanceAhead;
    [Space(10)]

    [Header("Obstacles")]
    [SerializeField] private List<GameObject> obsMovingTowardPlr;
    [SerializeField] private List<GameObject> obsMovingPastPlr;
    [SerializeField] private List<GameObject> obsTraffic;
    [Space(10)]

    [Header("Indicators")]
    [SerializeField] private List<GameObject> indicators;
    [Space(10)]


    private float counter = 0f;
    private float counterTraffic = 0f;
    private Transform playerTransform;
    private LaneManager laneManager;
    private readonly float[] lanePositions = { -40f, 0f, 40f };



    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        laneManager = new LaneManager(lanePositions);
    }



    // Update is called once per frame
    void Update()
    {
        //SpawnObstacle();
        //SpawnTraffic();
        //DifficultyScaling();
    }

    private void DifficultyScaling()
    {
        float difficulyTimer = Time.deltaTime;
        float diffIncInterval = 5.0f;
        if (difficulyTimer >= diffIncInterval && obsSpawneRate > 2f)
        {
            obsSpawneRate -= -0.5f;
            difficulyTimer = 0f;
        }


    }

    private void SpawnObstacle()
    {
        counter += Time.deltaTime;

        if (counter >= obsSpawneRate)
        {
            int randomSpawn = Random.Range(0, 3);

            switch (randomSpawn)
            {
                case 0:
                    ObsMovingPastPlayer();
                    break;
                case 1:
                    ObsMovingTowardsPlayer();
                    break;
                case 2:
                    ObsTrafficSpawn();
                    break;
            }
            counter = 0f;
        }
    }

    private void SpawnTraffic()
    {
        counterTraffic += Time.deltaTime;

        if (counterTraffic >= trafficSpawnRate)
        {
            ObsTrafficSpawn();
            counterTraffic = 0f;
        }
    }

    private void ObsMovingTowardsPlayer()
    {
        if (obsMovingTowardPlr.Count == 0 || playerTransform == null) return;

        laneManager.ResetLanes();

        int laneToLeaveFree = Random.Range(0, laneManager.laneCount);
        laneManager.OccupyLane(laneToLeaveFree);

        int obsToSpawn = laneManager.laneCount - 1;

        for (int i = 0; i < obsToSpawn; i++)
        {
            int laneIndex;
            do
            {
                laneIndex = Random.Range(0, laneManager.laneCount);
            } while (laneIndex == laneToLeaveFree); //loop only exist when we find a lane that is free

            laneManager.OccupyLane(laneIndex);

            GameObject prefab = obsMovingTowardPlr[Random.Range(0, obsMovingTowardPlr.Count)];
            int trafficLane = Random.RandomRange(0, 2);
            ObstacleLink link = prefab.GetComponent<ObstacleLink>();
            if (link == null || link.obsConfig == null) continue;

            ObstacleConfig config = link.obsConfig;

            float laneX = (trafficLane == 0) ? 20f : -20f;
            float spawnZ = playerTransform.position.z - config.spawnOffset.z;
            Vector3 spawnPos = new Vector3(laneManager.GetLaneX(laneIndex), config.spawnOffset.y, spawnZ);
            Quaternion rotation = config.faceBackward ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

            GameObject spawnedObstacle = Instantiate(prefab, spawnPos, rotation);
            spawnedObstacle.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;
            spawnedObstacle.transform.SetParent(transform, this);

            if (config.triggerPrefab != null)
            {
                Vector3 triggerPos = new Vector3(-laneManager.GetLaneX(laneIndex), config.triggerOffset.y, spawnZ);
                GameObject spawnedTrigger = Instantiate(config.triggerPrefab, triggerPos, Quaternion.identity);
                spawnedTrigger.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;
                spawnedTrigger.transform.SetParent(transform, this);
                Destroy(spawnedTrigger, config.lifespan);
            }

            Destroy(spawnedObstacle, config.lifespan);
        }
    }

    private void ObsMovingPastPlayer()
    {
        if (obsMovingPastPlr.Count == 0 || playerTransform == null) return;

        laneManager.ResetLanes();

        int laneToLeaveFree = Random.Range(0, laneManager.laneCount);
        laneManager.OccupyLane(laneToLeaveFree);

        int obsToSpawn = laneManager.laneCount - 1;

        for (int i = 0; i < obsToSpawn; i++)
        {
            int laneIndex;
            do
            {
                laneIndex = Random.Range(0, laneManager.laneCount);
            } while (laneIndex == laneToLeaveFree); //loop only exist when we find a lane that is free

            laneManager.OccupyLane(laneIndex);

            GameObject prefab = obsMovingPastPlr[Random.Range(0, obsMovingPastPlr.Count)];
            ObstacleLink link = prefab.GetComponent<ObstacleLink>();
            if (link == null || link.obsConfig == null) continue;

            ObstacleConfig config = link.obsConfig;

            float spawnZ = playerTransform.position.z + config.spawnOffset.z;
            Vector3 spawnPos = new Vector3(laneManager.GetLaneX(laneIndex), config.spawnOffset.y, spawnZ);
            Quaternion rotation = config.faceBackward ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

            GameObject spawnedObstacle = Instantiate(prefab, spawnPos, rotation);
            spawnedObstacle.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;
            spawnedObstacle.transform.SetParent(transform, this);

            if (config.triggerPrefab != null)
            {
                Vector3 triggerPos = new Vector3(-laneManager.GetLaneX(laneIndex), config.triggerOffset.y, spawnZ);
                GameObject spawnedTrigger = Instantiate(config.triggerPrefab, triggerPos, Quaternion.identity);
                spawnedTrigger.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;
                spawnedTrigger.transform.SetParent(transform, this);
                Destroy(spawnedTrigger, config.lifespan);
            }

            if (config.indicatorPrefab != null)
            {
                Vector3 indicatorPos = new Vector3(laneManager.GetLaneX(laneIndex), 45f, playerTransform.position.z - 600f);
                GameObject spawnedIndicator = Instantiate(config.indicatorPrefab, indicatorPos, Quaternion.identity);
                spawnedIndicator.transform.SetParent(transform, this);
                Destroy(spawnedIndicator, config.lifespan);
            }

            Destroy(spawnedObstacle, config.lifespan);
        }
    }

    private void ObsTrafficSpawn()
    {
        if (obsTraffic.Count == 0 || playerTransform == null) return;

        laneManager.ResetLanes();

        int laneToLeaveFree = Random.Range(0, laneManager.laneCount);
        laneManager.OccupyLane(laneToLeaveFree);

        int trafficToSpawn = laneManager.laneCount - 1;

        for (int i = 0; i < trafficToSpawn; i++)
        {
            int laneIndex;
            do
            {
                laneIndex = Random.Range(0, laneManager.laneCount);
            } while (laneIndex == laneToLeaveFree); //loop only exist when we find a lane that is free

            laneManager.OccupyLane(laneIndex);


            GameObject prefab = obsTraffic[Random.Range(0, obsTraffic.Count)];
            ObstacleLink link = prefab.GetComponent<ObstacleLink>();
            if (link == null || link.obsConfig == null) continue;

            ObstacleConfig config = link.obsConfig;

            float spawnZ = playerTransform.position.z - config.spawnOffset.z;
            Vector3 spawnPos = new Vector3(laneManager.GetLaneX(laneIndex), config.spawnOffset.y, spawnZ);
            Quaternion rotation = config.faceBackward ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

            GameObject spawnedTraffic = Instantiate(prefab, spawnPos, rotation);
            spawnedTraffic.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;
            spawnedTraffic.transform.SetParent(transform, this);

            if (config.triggerPrefab != null)
            {
                Vector3 triggerPos = new Vector3(-laneManager.GetLaneX(laneIndex), config.triggerOffset.y, spawnZ);
                GameObject spawnedTrigger = Instantiate(config.triggerPrefab, triggerPos, Quaternion.identity);
                spawnedTrigger.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;
                spawnedTrigger.transform.SetParent(transform, this);
                Destroy(spawnedTrigger, config.lifespan);
            }

            Destroy(spawnedTraffic, config.lifespan);
        }
    }

}





