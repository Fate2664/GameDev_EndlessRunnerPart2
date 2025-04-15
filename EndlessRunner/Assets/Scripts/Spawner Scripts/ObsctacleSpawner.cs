using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.VFX;

public class ObstacleSpawner : MonoBehaviour
{

    float SpawnRate = 3f;
    float Counter = 0f;



    public List<GameObject> Obsctacles;
    public Transform Player;
    public GameObject AmbulanceIndicator;




    public List<float> DistanceAhead;
    private GameObject spawnedObstacle;
    private Vector3 indicatorPosition;
    private Vector3 spawnObstaclePosition;
    private GameObject movingPassTrigger;
    private GameObject staticPassTrigger;
    private GameObject spawnedTrigger;
    private GameObject spawnedIndicator;
    private float yHeightPassTrigger = 8f;
    private Vector3 spawnTriggerPosition;



    private void Start()
    {
        //get the two obstacle triggers
        movingPassTrigger = GameObject.Find("MovingObstaclePassTrigger");       
        staticPassTrigger = GameObject.Find("StaticObstaclePassTrigger");
    }



    // Update is called once per frame
    void Update()
    {
        SpawnObstacle();
        DifficultyScaling();
    }

    private void DifficultyScaling()
    {
        float GameCounter = Time.deltaTime;
        float DiffIncInterval = 5.0f;

        if (GameCounter > DiffIncInterval && SpawnRate > 2)
        {
            SpawnRate += -0.5f;

        }


    }

    private void SpawnObstacle()
    {
        Counter += Time.deltaTime;


        if (Counter >= SpawnRate)
        {

            if (Obsctacles.Count > 0 && Player != null)
            {
                int randomIndex = Random.Range(0, Obsctacles.Count);        //choose a random obstacle from the list

                switch (randomIndex)
                {
                    //Building Spawn
                    case 0:
                        int BuildingSpawnPoint;
                        BuildingSpawnPoint = Random.Range(1, 3);        //choose a random lane/side in which to spawn the obstacle

                        if (BuildingSpawnPoint == 1)
                        {
                            spawnObstaclePosition = new Vector3(-240, 0, Player.position.z - DistanceAhead[0]);     //determine the position for the obstacle to spawn
                            spawnedObstacle = Instantiate(Obsctacles[randomIndex], spawnObstaclePosition, Quaternion.identity);     //create a clone of the obstacle and 
                        }

                        if (BuildingSpawnPoint == 2)
                        {
                            spawnObstaclePosition = new Vector3(340, 0, Player.position.z - DistanceAhead[0]);
                            spawnedObstacle = Instantiate(Obsctacles[randomIndex], spawnObstaclePosition, Quaternion.Euler(0, 180, 0));
                        }
                        spawnTriggerPosition = new Vector3(0, yHeightPassTrigger, Player.position.z - DistanceAhead[0] - 30f);      //Create the position for the trigger to spawn
                        spawnedTrigger = Instantiate(staticPassTrigger, spawnTriggerPosition, Quaternion.identity);                 //Create a clone of the trigger and spawn it
                        spawnedObstacle.transform.SetParent(transform, false);
                        spawnedTrigger.transform.SetParent(transform, false);
                        break;
                    //Truck Spawn
                    case 1:
                        int TruckSpawnPoint;
                        TruckSpawnPoint = Random.Range(1, 3);
                        MovingObstacle.obstacleIndex = 1;       //give the moving obstacle index in order to determine the speed of the trigger

                        if (TruckSpawnPoint == 1)
                        {
                            spawnObstaclePosition = new Vector3(22, 0, Player.position.z - DistanceAhead[1]);
                            spawnTriggerPosition = new Vector3(-22, yHeightPassTrigger, Player.position.z - DistanceAhead[1]);
                        }

                        if (TruckSpawnPoint == 2)
                        {
                            spawnObstaclePosition = new Vector3(-22, 0, Player.position.z - DistanceAhead[1]);
                            spawnTriggerPosition = new Vector3(22, yHeightPassTrigger, Player.position.z - DistanceAhead[1]);
                        }
                        spawnedObstacle = Instantiate(Obsctacles[randomIndex], spawnObstaclePosition, Quaternion.identity);
                        spawnedTrigger = Instantiate(movingPassTrigger, spawnTriggerPosition, Quaternion.identity);
                        spawnedTrigger.transform.SetParent(transform, this);
                        spawnedObstacle.transform.SetParent(transform, this);
                        break;

                    //Ambulance Spawn
                    case 2:
                        int AmbulanceSpawnPoint;
                        AmbulanceSpawnPoint = Random.Range(1, 3);
                        MovingObstacle.obstacleIndex = 0;

                        if (AmbulanceSpawnPoint == 1)
                        {
                            spawnObstaclePosition = new Vector3(-22, 0, Player.position.z + DistanceAhead[2]);
                            spawnTriggerPosition = new Vector3(22, yHeightPassTrigger, Player.position.z + DistanceAhead[2]);
                            indicatorPosition = new Vector3(-22, 45, Player.position.z - DistanceAhead[3]);

                        }

                        if (AmbulanceSpawnPoint == 2)
                        {
                            spawnObstaclePosition = new Vector3(22, 0, Player.position.z + DistanceAhead[2]);
                            spawnTriggerPosition = new Vector3(-22, yHeightPassTrigger, Player.position.z + DistanceAhead[2]);
                            indicatorPosition = new Vector3(22, 45, Player.position.z - DistanceAhead[3]);
                        }
                        spawnedTrigger = Instantiate(movingPassTrigger, spawnTriggerPosition, Quaternion.identity);
                        spawnedIndicator = Instantiate(AmbulanceIndicator, indicatorPosition, Quaternion.identity);
                        spawnedObstacle = Instantiate(Obsctacles[randomIndex], spawnObstaclePosition, Quaternion.Euler(0, 180, 0));
                        spawnedTrigger.transform.SetParent(transform, this);
                        spawnedIndicator.transform.SetParent(transform, this);
                        spawnedObstacle.transform.SetParent(transform, this);
                        break;


                }

                //destroy the clone after 5 increment points
                if (spawnedObstacle != null)
                {
                    Destroy(spawnedObstacle, 5f);
                }
                if (spawnedTrigger != null)
                {
                    Destroy(spawnedTrigger, 5f);
                }
                if (AmbulanceIndicator != null)
                {
                    Destroy(spawnedIndicator, 5f);
                }

                Counter = 0f;
            }




        }



    }

}





