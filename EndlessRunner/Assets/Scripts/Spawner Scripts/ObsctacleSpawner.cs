using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObstacleSpawner : MonoBehaviour
{

    [Header("Spawn Details")]
    [SerializeField] private int initialTrafficAmount = 5;
    [Range(0.5f, 10f)]
    [SerializeField] private float obsSpawnRate = 1f;
    [Range(0.5f, 10f)]
    [SerializeField] private float trafficSpawnRate = 1f;
    [Range(0.5f, 10f)]
    [SerializeField] private float constructionRoadSpawnRate = 1f;
    [Space(10)]

    [Header("Obstacles")]
    [SerializeField] private List<GameObject> obsMovingTowardPlr;
    [SerializeField] private List<GameObject> obsMovingPastPlr;
    [SerializeField] private List<GameObject> obsTraffic;
    [Space(10)]

    [Header("Indicators")]
    [SerializeField] private List<GameObject> indicators;
    [Space(10)]

    [Header("Events")]
    public UnityEvent OnConstructionRoadSpawn;

    [HideInInspector]
    public bool spawningConstrRoad = false;
    private float counter = 0f;
    private float counterTraffic = 0f;
    private float counterConstrRoad = 0f;
    private float[] initialTrafficOffsets = { 400, 600, 800, 1000, 1200};
    private Transform playerTransform;
    private LaneManager laneManager;
    private readonly float[] lanePositions = { -40f, 0f, 40f };




    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        laneManager = new LaneManager(lanePositions);

        counterTraffic = Time.deltaTime;
        if (counterTraffic < trafficSpawnRate)
        {
            ObsInitialTrafficSpawn();
            counterTraffic = 0f;
        }

        
    }

    void Update()
    {
        //SpawnObstacle();
        SpawnTraffic();
        ObsConstrRoadSpawn();
        //DifficultyScaling();
    }

    private void DifficultyScaling()
    {
        float difficulyTimer = Time.deltaTime;
        float diffIncInterval = 5.0f;
        if (difficulyTimer >= diffIncInterval && obsSpawnRate > 2f)
        {
            obsSpawnRate -= -0.5f;
            difficulyTimer = 0f;
        }


    }


    private void SpawnObstacle()
    {
        counter += Time.deltaTime;

        if (counter >= obsSpawnRate)
        {
            int randomSpawn = Random.Range(0, 2);

            switch (randomSpawn)
            {
                case 0:
                    ObsMovingPastPlayer();
                    break;
                case 1:
                    ObsMovingTowardsPlayer();
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

        int numObsToSpawn = 1;
        List<int> occupiedLanes = new List<int>();

        GameObject prefab = obsMovingTowardPlr[Random.Range(0, obsMovingTowardPlr.Count)];
        ObstacleLink link = prefab.GetComponent<ObstacleLink>();
        ObstacleConfig config = link.obsConfig;

        while (occupiedLanes.Count < numObsToSpawn)
        {
            int laneIndexObs = Random.Range(0, laneManager.laneCount);
            if (!occupiedLanes.Contains(laneIndexObs))
            {
                laneManager.OccupyLane(laneIndexObs);
                occupiedLanes.Add(laneIndexObs);

                float spawnZ = playerTransform.position.z - config.spawnOffset.z;
                Vector3 spawnPos = new Vector3(laneManager.GetLaneX(laneIndexObs), config.spawnOffset.y, spawnZ);
                Quaternion rotation = config.faceBackward ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

                GameObject spawnedObstacle = Instantiate(prefab, spawnPos, rotation);
                spawnedObstacle.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;
                spawnedObstacle.transform.SetParent(transform, this);
                Destroy(spawnedObstacle, config.lifespan);
            }
        }

        foreach (int laneIndexTrig in laneManager.GetAllFreeLane())
        {
            if (config.triggerPrefab != null)
            {
                float spawnZ = playerTransform.position.z - config.spawnOffset.z;
                Vector3 triggerPos = new Vector3(laneManager.GetLaneX(laneIndexTrig), config.triggerOffset.y, spawnZ);
                GameObject spawnedTrigger = Instantiate(config.triggerPrefab, triggerPos, Quaternion.identity);
                spawnedTrigger.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;
                spawnedTrigger.transform.SetParent(transform, this);
                Destroy(spawnedTrigger, config.lifespan);
            }
        }
    }

    private void ObsMovingPastPlayer()
    {
        if (obsMovingPastPlr.Count == 0 || playerTransform == null) return;

        laneManager.ResetLanes();

        int numObsToSpawn = 1;
        List<int> occupiedLanes = new List<int>();

        GameObject prefab = obsMovingPastPlr[Random.Range(0, obsMovingPastPlr.Count)];
        ObstacleLink link = prefab.GetComponent<ObstacleLink>();
        ObstacleConfig config = link.obsConfig;

        while (occupiedLanes.Count < numObsToSpawn)
        {
            int laneIndexObs = Random.Range(0, laneManager.laneCount);

            if (!occupiedLanes.Contains(laneIndexObs))
            {
                laneManager.OccupyLane(laneIndexObs);
                occupiedLanes.Add(laneIndexObs);

                float spawnZ = playerTransform.position.z + config.spawnOffset.z;
                Vector3 spawnPos = new Vector3(laneManager.GetLaneX(laneIndexObs), config.spawnOffset.y, spawnZ);
                Quaternion rotation = config.faceBackward ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

                GameObject spawnedObstacle = Instantiate(prefab, spawnPos, rotation);
                spawnedObstacle.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;
                spawnedObstacle.transform.SetParent(transform, this);
                Destroy(spawnedObstacle, config.lifespan);

                if (config.indicatorPrefab != null)
                {
                    Vector3 indicatorPos = new Vector3(laneManager.GetLaneX(laneIndexObs), 35f, playerTransform.position.z - 600f);
                    GameObject spawnedIndicator = Instantiate(config.indicatorPrefab, indicatorPos, Quaternion.identity);
                    spawnedIndicator.transform.SetParent(transform, this);
                    Destroy(spawnedIndicator, config.lifespan);
                }
            }
        }

        foreach (int laneIndexTrig in laneManager.GetAllFreeLane())
        {
            if (config.triggerPrefab != null)
            {
                float spawnZ = playerTransform.position.z + config.spawnOffset.z;
                Vector3 triggerPos = new Vector3(laneManager.GetLaneX(laneIndexTrig), config.triggerOffset.y, spawnZ);
                GameObject spawnedTrigger = Instantiate(config.triggerPrefab, triggerPos, Quaternion.identity);
                spawnedTrigger.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;
                spawnedTrigger.transform.SetParent(transform, this);
                Destroy(spawnedTrigger, config.lifespan);
            }
        }
    }

    private void ObsTrafficSpawn()
    {
        if (obsTraffic.Count == 0 || playerTransform == null) return;

        laneManager.ResetLanes();

        int numTraffToSpawn = 1;
        List<int> occupiedLanes = new List<int>();

        GameObject prefab = obsTraffic[Random.Range(0, obsTraffic.Count)];
        ObstacleLink link = prefab.GetComponent<ObstacleLink>();
        ObstacleConfig config = link.obsConfig;

        while (occupiedLanes.Count < numTraffToSpawn)
        {
            int laneIndex = Random.Range(0, laneManager.laneCount);

            if (!occupiedLanes.Contains(laneIndex))
            {
                occupiedLanes.Add(laneIndex);
                laneManager.OccupyLane(laneIndex);

                float spawnZ = playerTransform.position.z - config.spawnOffset.z;
                Vector3 spawnPos = new Vector3(laneManager.GetLaneX(laneIndex), config.spawnOffset.y, spawnZ);
                Quaternion rotation = config.faceBackward ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

                GameObject spawnedTraffic = Instantiate(prefab, spawnPos, rotation);
                spawnedTraffic.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;
                spawnedTraffic.transform.SetParent(transform, this);
                Destroy(spawnedTraffic, config.lifespan);
            }
        }
        foreach (int laneIndexTrig in laneManager.GetAllFreeLane())
        {
            if (config.triggerPrefab != null)
            {
                float spawnZ = playerTransform.position.z - config.spawnOffset.z;
                Vector3 triggerPos = new Vector3(laneManager.GetLaneX(laneIndexTrig), config.triggerOffset.y, spawnZ);
                GameObject spawnedTrigger = Instantiate(config.triggerPrefab, triggerPos, Quaternion.identity);
                spawnedTrigger.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;
                spawnedTrigger.transform.SetParent(transform, this);
                Destroy(spawnedTrigger, config.lifespan);
            }
        }

    }

    private void ObsInitialTrafficSpawn()
    {
        for (int i = 0; i < initialTrafficOffsets.Length; i++)
        {
            if (obsTraffic.Count == 0 || playerTransform == null) return;

            laneManager.ResetLanes();

            int numTraffToSpawn = 1;
            List<int> occupiedLanes = new List<int>();

            GameObject prefab = obsTraffic[Random.Range(0, obsTraffic.Count)];
            ObstacleLink link = prefab.GetComponent<ObstacleLink>();
            ObstacleConfig config = link.obsConfig;


            float initialTrafficOffset = initialTrafficOffsets[i];

            while (occupiedLanes.Count < numTraffToSpawn)
            {
                int laneIndex = Random.Range(0, laneManager.laneCount);

                if (!occupiedLanes.Contains(laneIndex))
                {
                    occupiedLanes.Add(laneIndex);
                    laneManager.OccupyLane(laneIndex);


                    float spawnZ = playerTransform.position.z - initialTrafficOffset;
                    Vector3 spawnPos = new Vector3(laneManager.GetLaneX(laneIndex), config.spawnOffset.y, spawnZ);
                    Quaternion rotation = config.faceBackward ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

                    GameObject spawnedTraffic = Instantiate(prefab, spawnPos, rotation);
                    spawnedTraffic.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;
                    spawnedTraffic.transform.SetParent(transform, this);
                    Destroy(spawnedTraffic, config.lifespan);

                }
            }
            foreach (int laneIndexTrig in laneManager.GetAllFreeLane())
            {
                if (config.triggerPrefab != null)
                {
                    float spawnZ = playerTransform.position.z - initialTrafficOffset;
                    Vector3 triggerPos = new Vector3(laneManager.GetLaneX(laneIndexTrig), config.triggerOffset.y, spawnZ);
                    GameObject spawnedTrigger = Instantiate(config.triggerPrefab, triggerPos, Quaternion.identity);
                    spawnedTrigger.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;
                    spawnedTrigger.transform.SetParent(transform, this);
                    Destroy(spawnedTrigger, config.lifespan);
                }
            }

        }
    }

    private void ObsConstrRoadSpawn()
    {
        counterConstrRoad += Time.deltaTime;

        if (counterConstrRoad >= constructionRoadSpawnRate)
        {
            if (Random.value < 0.3f)
            {
                OnConstructionRoadSpawn?.Invoke();
                spawningConstrRoad = false;
            }
            counterConstrRoad = 0f;
        }
    }
}







