using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;


public class ObstacleSpawner : MonoBehaviour
{

    [Header("Spawn Details")]
    [UnityEngine.Range(1f, 10f)]
    [SerializeField] private float SpawnRate = 3f;
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


    private float counterTowardPlayer = 0f;
    private float counterPastPlayer = 0f;
    private float counterTraffic = 0f;
    private GameObject spawnedObstacle;
    private Vector3 indicatorPosition;
    private Vector3 spawnObstaclePosition;
    private GameObject movingPassTrigger;
    private GameObject staticPassTrigger;
    private GameObject spawnedTrigger;
    private GameObject spawnedIndicator;
    private float yHeightPassTrigger = 8f;
    private float yHeightTraffic = 6f;
    private Vector3 spawnTriggerPosition;
    private Transform playerTransform;



    private void Start()
    {
        //get the two obstacle triggers
        movingPassTrigger = GameObject.Find("MovingObstaclePassTrigger");
        staticPassTrigger = GameObject.Find("StaticObstaclePassTrigger");
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }



    // Update is called once per frame
    void Update()
    {
        SpawnObstacle();
        //DifficultyScaling();
    }

    private void DifficultyScaling()
    {
        float difficulyTimer = Time.deltaTime;
        float diffIncInterval = 5.0f;
        if (difficulyTimer >= diffIncInterval && SpawnRate > 2f)
        {
            SpawnRate -= -0.5f;
            difficulyTimer = 0f;
        }


    }

    private void SpawnObstacle()
    {
        //ObsMovingPastPlayer();
        //ObsMovingTowardsPlayer();
        ObsTrafficSpawn();
    }

    private void ObsMovingTowardsPlayer()
    {
        counterTowardPlayer += Time.deltaTime;

        if (counterTowardPlayer >= SpawnRate)
        {
            if (obsMovingTowardPlr.Count > 0 && playerTransform != null)
            {
                int randomIndex = Random.Range(0, obsMovingTowardPlr.Count);

                switch (randomIndex)
                {

                    //Truck Spawn
                    case 0:
                        int TruckSpawnPoint;
                        TruckSpawnPoint = Random.Range(1, 3);

                        if (TruckSpawnPoint == 1)
                        {
                            spawnObstaclePosition = new Vector3(22, 0, playerTransform.position.z - distanceAhead[0]);
                            spawnTriggerPosition = new Vector3(-22, yHeightPassTrigger, playerTransform.position.z - distanceAhead[0]);
                        }

                        if (TruckSpawnPoint == 2)
                        {
                            spawnObstaclePosition = new Vector3(-22, 0, playerTransform.position.z - distanceAhead[0]);
                            spawnTriggerPosition = new Vector3(22, yHeightPassTrigger, playerTransform.position.z - distanceAhead[0]);
                        }
                        spawnedObstacle = Instantiate(obsMovingTowardPlr[randomIndex], spawnObstaclePosition, Quaternion.identity);
                        spawnedObstacle.GetComponent<MovingObstacle>().obstacleIndex = 1; 
                        spawnedTrigger = Instantiate(movingPassTrigger, spawnTriggerPosition, Quaternion.identity);
                        spawnedTrigger.GetComponent<MovingObstacle>().obstacleIndex = 1;
                        spawnedTrigger.transform.SetParent(transform, this);
                        spawnedObstacle.transform.SetParent(transform, this);
                        break;
                }

                //destroy the clone after 5 seconds
                if (spawnedObstacle != null)
                {
                    Destroy(spawnedObstacle, 5f);
                }
                if (spawnedTrigger != null)
                {
                    Destroy(spawnedTrigger, 5f);
                }
                if (spawnedIndicator != null)
                {
                    Destroy(spawnedIndicator, 5f);
                }

                counterTowardPlayer = 0f;
            }
        }
    }

    private void ObsMovingPastPlayer()
    {
        counterPastPlayer += Time.deltaTime;

        if (counterPastPlayer >= SpawnRate)
        {
            if (obsMovingPastPlr.Count > 0 && playerTransform != null)
            {
                int randomIndex = Random.Range(0, obsMovingPastPlr.Count);

                switch (randomIndex)
                {
                    //Ambulance Spawn
                    case 0:
                        int AmbulanceSpawnPoint;
                        AmbulanceSpawnPoint = Random.Range(1, 3);

                        if (AmbulanceSpawnPoint == 1)
                        {
                            spawnObstaclePosition = new Vector3(-22, 0, playerTransform.position.z + distanceAhead[1]);
                            spawnTriggerPosition = new Vector3(22, yHeightPassTrigger, playerTransform.position.z + distanceAhead[1]);
                            indicatorPosition = new Vector3(-22, 45, playerTransform.position.z - distanceAhead[2]);

                        }

                        if (AmbulanceSpawnPoint == 2)
                        {
                            spawnObstaclePosition = new Vector3(22, 0, playerTransform.position.z + distanceAhead[1]);
                            spawnTriggerPosition = new Vector3(-22, yHeightPassTrigger, playerTransform.position.z + distanceAhead[1]);
                            indicatorPosition = new Vector3(22, 45, playerTransform.position.z - distanceAhead[2]);
                        }
                        spawnedTrigger = Instantiate(movingPassTrigger, spawnTriggerPosition, Quaternion.identity);
                        spawnedTrigger.GetComponent<MovingObstacle>().obstacleIndex = 0;
                        spawnedIndicator = Instantiate(indicators[0], indicatorPosition, Quaternion.identity);
                        spawnedObstacle = Instantiate(obsMovingPastPlr[randomIndex], spawnObstaclePosition, Quaternion.Euler(0, 180, 0));
                        spawnedObstacle.GetComponent<MovingObstacle>().obstacleIndex = 0;
                        spawnedTrigger.transform.SetParent(transform, this);
                        spawnedIndicator.transform.SetParent(transform, this);
                        spawnedObstacle.transform.SetParent(transform, this);
                        break;


                }

                //destroy the clone after 5 seconds
                if (spawnedObstacle != null)
                {
                    Destroy(spawnedObstacle, 5f);
                }
                if (spawnedTrigger != null)
                {
                    Destroy(spawnedTrigger, 5f);
                }
                if (spawnedIndicator != null)
                {
                    Destroy(spawnedIndicator, 5f);
                }

                counterPastPlayer = 0f;
            }
        }
    }

    private void ObsTrafficSpawn()
    {
        counterTraffic += Time.deltaTime;

        if (counterTraffic >= SpawnRate)
        {
            if (obsTraffic.Count > 0 && playerTransform != null)
            {
                int trafficLane = Random.RandomRange(0, 2);
                int randomIndex = Random.Range(0, obsTraffic.Count);
                float laneX = (trafficLane == 0) ? 20f : -20f;
                
                switch (randomIndex)
                {
                    case 0:
                        spawnObstaclePosition = new Vector3(laneX, yHeightTraffic, playerTransform.position.z - distanceAhead[1]);
                        spawnTriggerPosition = new Vector3(-laneX, yHeightPassTrigger, playerTransform.position.z - distanceAhead[1]);
                        spawnedObstacle = Instantiate(obsTraffic[randomIndex], spawnObstaclePosition, Quaternion.identity);
                        spawnedObstacle.GetComponent<MovingObstacle>().obstacleIndex = 2;
                        spawnedTrigger = Instantiate(movingPassTrigger, spawnTriggerPosition, Quaternion.identity);
                        spawnedTrigger.GetComponent<MovingObstacle>().obstacleIndex = 2;
                        spawnedTrigger.transform.SetParent(transform, this);
                        spawnedObstacle.transform.SetParent(transform, this);
                        break;
                }

                if (spawnedObstacle != null)
                {
                    Destroy(spawnedObstacle, 10f);
                }
                if (spawnedTrigger != null)
                {
                    Destroy(spawnedTrigger, 10f);
                }

                counterTraffic = 0f;
            }
        }
    }

}





