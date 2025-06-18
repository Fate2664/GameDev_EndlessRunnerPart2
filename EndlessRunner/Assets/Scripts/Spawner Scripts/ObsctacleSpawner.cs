using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ObstacleSpawner : MonoBehaviour
{
    //This script controls the spawning of all the obstacles within the game

    //This enum tracks the state of the current construction road to spawn
    public enum ConstrRoadState
    {
        None,
        Start,
        Middle,
        End
    }

    [Header("Spawn Details")]
    [Range(0.5f, 10f)]
    [SerializeField] private float obsSpawnRate = 5f;
    [Range(0.5f, 10f)]
    [SerializeField] private float trafficSpawnRate = 1f;
    [Range(5f, 30f)]
    [SerializeField] private float constructionRoadSpawnRate = 10f;
    [Range(5f, 30f)]
    [SerializeField] private float spikeRoadSpawnRate = 10f;
    [Space(10)]

    [Header("Obstacles")]
    [SerializeField] private List<GameObject> obsMovingTowardPlr;
    [SerializeField] private List<GameObject> obsMovingPastPlr;
    [SerializeField] private List<GameObject> obsTraffic;
    [Space(10)]

    [Header("Indicators")]
    [SerializeField] private List<GameObject> indicators;
    [Space(10)]

    [Header("Connections")]
    [SerializeField] private RoadSpawner roadSpawner;


    private float counter = 0f;
    private float counterTraffic = 0f;
    private float counterConstrRoad = 0f;
    private float[] initialTrafficOffsets = { 400, 600, 800, 1000, 1200 };
    private Transform playerTransform;
    [HideInInspector]
    public LaneManager laneManager;
    private readonly float[] lanePositions = { -40f, 0f, 40f };
    [HideInInspector]
    public bool spawningConstrRoad = false;
    [HideInInspector]
    public bool spawningSpikeRoad = false;
    [HideInInspector]
    public int middleConstrRemaining = 0;
    [HideInInspector]
    public ConstrRoadState constrRoadState = ConstrRoadState.None;
    [HideInInspector]
    public int constrSide = 0;
    [HideInInspector]
    public bool stopTraffic = false;
    [HideInInspector]
    public bool canSpawnConstrRoad = true;




    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        laneManager = new LaneManager(lanePositions);

        counterTraffic = Time.deltaTime;
        //Spawn the initial traffic bunch
        if (counterTraffic < trafficSpawnRate)
        {
            ObsInitialTrafficSpawn();
            counterTraffic = 0f;
        }
        canSpawnConstrRoad = true;
    }

    void Update()
    {
        if (!stopTraffic)
        {
            SpawnTraffic();
        }
        ObsConstrRoadSpawn();
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

    //This method controls the spawning of the moving obstacles that aren't traffic
    public void SpawnBossResidential()
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

    public void SpawnBossCity()
    {
        counter += Time.deltaTime;

        if (counter >= obsSpawnRate)
        {
            
            int randomSpawn = Random.Range(0, 3);

            switch (randomSpawn)
            {
                case 0:
                    ObsSpikeRoadSpawn();
                    break;
                case 1:
                    ObsSpikeRoadSpawn();
                    break;
                case 2:
                    ObsSpikeRoadSpawn();
                    break;

            }
            counter = 0f;
            
         
        }
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

    //This method controls the spawning of the obstacles moving towards the player (during the boss)
    private void ObsMovingTowardsPlayer()
    {
        if (obsMovingTowardPlr.Count == 0 || playerTransform == null) return;


        int numObsToSpawn = Random.Range(1, 2);
        List<int> occupiedLanes = new List<int>();

        //Get a random prefab from the list and get its scriptable object for that obstacle
        GameObject prefab = obsMovingTowardPlr[Random.Range(0, obsMovingTowardPlr.Count)];
        ObstacleLink link = prefab.GetComponent<ObstacleLink>();
        ObstacleConfig config = link.obsConfig;

        //This loop spawns the obstacles in a free lane
        while (occupiedLanes.Count < numObsToSpawn)
        {
            int laneIndexObs = Random.Range(0, laneManager.laneCount);
            if (!occupiedLanes.Contains(laneIndexObs) && laneManager.IsLaneFree(laneIndexObs)) //if the lane is not occupied already
            {
                //mark it as occupied
                laneManager.OccupyLane(laneIndexObs);
                occupiedLanes.Add(laneIndexObs);

                //Get the spawning details
                float spawnZ = playerTransform.position.z - config.spawnOffset.z;
                Vector3 spawnPos = new Vector3(laneManager.GetLaneX(laneIndexObs), config.spawnOffset.y, spawnZ);
                Quaternion rotation = config.faceBackward ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

                //Spawn the prefab
                GameObject spawnedObstacle = Instantiate(prefab, spawnPos, rotation);
                spawnedObstacle.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;
                spawnedObstacle.transform.SetParent(transform, this);
                Destroy(spawnedObstacle, config.lifespan);
            }
        }
        //This loop gets all the free lanes and adds a trigger box for incrementing the score
        foreach (int laneIndexTrig in laneManager.GetAllFreeLanes())
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
        laneManager.ResetLanes();
    }

    //This method controls the spawning of the obstacles moving past the player (during the boss)
    private void ObsMovingPastPlayer()
    {
        if (obsMovingPastPlr.Count == 0 || playerTransform == null) return;


        int numObsToSpawn = Random.Range(1, 2);
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

                //This loop spawns the indicator prefab infront of the obstacle
                if (config.indicatorPrefab != null)
                {
                    Vector3 indicatorPos = new Vector3(laneManager.GetLaneX(laneIndexObs), 35f, playerTransform.position.z - 600f);
                    GameObject spawnedIndicator = Instantiate(config.indicatorPrefab, indicatorPos, Quaternion.identity);
                    spawnedIndicator.transform.SetParent(transform, this);
                    Destroy(spawnedIndicator, config.lifespan);
                }
            }
        }

        foreach (int laneIndexTrig in laneManager.GetAllFreeLanes())
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
        laneManager.ResetLanes();
    }

    //This method controls the spawning of the traffic
    private void ObsTrafficSpawn()
    {
        if (obsTraffic.Count == 0 || playerTransform == null) return;


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
                float spawnZ = playerTransform.position.z - config.spawnOffset.z;
                Vector3 spawnPos = new Vector3(laneManager.GetLaneX(laneIndex), config.spawnOffset.y, spawnZ);
                Quaternion rotation = config.faceBackward ? prefab.transform.rotation : Quaternion.identity;

                Collider[] overlaps = Physics.OverlapBox(spawnPos, new Vector3(1f, 1f, 1f), Quaternion.identity, LayerMask.GetMask("Default"));
                bool inNoSpawnZone = false;
                //This loop makes sure that the traffic don't spawn in a no spawn trigger zone
                foreach (Collider coll in overlaps)
                {
                    if (coll.CompareTag("NoSpawnTrigger"))
                    {
                        inNoSpawnZone = true;
                        break;
                    }
                }

                if (inNoSpawnZone)
                {
                    continue;
                }

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
        //spawn the trigger boxes for all the free lanes
        foreach (int laneIndexTrig in laneManager.GetAllFreeLanes())
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

    //This method is the same as the other spawning traffic method but this one spawns the initial bunch of traffic
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
                    Quaternion rotation = config.faceBackward ? prefab.transform.rotation : Quaternion.identity;

                    GameObject spawnedTraffic = Instantiate(prefab, spawnPos, rotation);
                    spawnedTraffic.GetComponent<MovingObstacle>().obstacleIndex = config.movementSpeedIndex;
                    if (config.faceBackward && config.movementSpeedIndex == 2)
                    {
                        spawnedTraffic.GetComponent<MovingObstacle>().obstacleIndex = 3;
                    }

                    spawnedTraffic.transform.SetParent(transform, this);
                    Destroy(spawnedTraffic, config.lifespan);

                }
            }
            foreach (int laneIndexTrig in laneManager.GetAllFreeLanes())
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

    //This method controls when the spawning of the contruction should happen
    private void ObsConstrRoadSpawn()
    {
        counterConstrRoad += Time.deltaTime;

        if (counterConstrRoad >= constructionRoadSpawnRate && constrRoadState == ConstrRoadState.None && canSpawnConstrRoad)
        {
            spawningConstrRoad = true;
            constrRoadState = ConstrRoadState.Start;
            constrSide = Random.Range(0, 2);

            counterConstrRoad = 0f;
        }
    }

    private void ObsSpikeRoadSpawn()
    {
        if (!spawningSpikeRoad)
        {
            spawningSpikeRoad = true;
            roadSpawner.SpawnSpikeRoad();

        }
    }
}







