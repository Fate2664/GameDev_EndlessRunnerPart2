using UnityEngine;
using System.Collections.Generic;
using System.Xml;

// Enum to define the types of spawning
public enum SpawnType
{
    Resindential,
    City
}
public class SpawnManager : MonoBehaviour
{
    // This script manages the spawning of different types of lands and roads in the game


    [SerializeField] private DistanceManager distanceManager;


    RoadSpawner roadSpawner;
    LandSpawner landSpawner;
    ObstacleSpawner obstacleSpawner;
    BossSpawner bossSpawner;

    private int counter = 0; // counter to keep track of how many transition lands have been spawned
    private bool inTransition = false;
    [HideInInspector]
    public SpawnType currentType = SpawnType.Resindential; // current type of land being spawned
    private SpawnType nextType = SpawnType.City;

    void Start()
    {
        //get the scripts for each spawner
        roadSpawner = GetComponent<RoadSpawner>();
        landSpawner = GetComponent<LandSpawner>();
        obstacleSpawner = GetComponent<ObstacleSpawner>();
        bossSpawner = GetComponent<BossSpawner>();
        roadSpawner.InitializeObsSpawner(obstacleSpawner);
    }



    public void SpawnTriggerEntered()
    {
        Invoke(nameof(SpawnRoad), 0.8f);    // call the road spawner
        //call for the land to be spawned and destroyed
        SpawnLand();
        Invoke(nameof(DestroyLands), 1f);
    }

    private void DestroyLands()
    {
        landSpawner.DestroyLand();
    }

    // This method decides whether or not to move a normal road prefab or spawn a construction road prefab
    private void SpawnRoad()
    {
        if (obstacleSpawner.spawningConstrRoad)
        {
            roadSpawner.SpawnNextConstructionRoad();
        }
        else
        if (!obstacleSpawner.spawningSpikeRoad)
        {
            roadSpawner.MoveNormalRoad();
        }
    }

    // this method decides which type of land to spawn
    private void SpawnLand()
    {
        if (inTransition)
        {
            if (counter < landSpawner.TransitionLandCount)
            {
                landSpawner.SpawnLand(landSpawner.TransitionLandPrefabs);
                counter++;
            }
            else
            {
                currentType = nextType; 
                inTransition = false; 
                counter = 0; 
                distanceManager.ResetDistance(); 
                bossSpawner.bossDefeated = false; 
                SpawnLand(); 
               
            }
            return;
        }
        switch (currentType)
        {
            case SpawnType.Resindential:
                landSpawner.SpawnLand(landSpawner.ResidentialLandPrefabs);
                break;
            case SpawnType.City:
                landSpawner.SpawnLand(landSpawner.CityLandPrefabs);
                break;
        }

        if (bossSpawner.bossDefeated && !inTransition)
        {
            inTransition = true; 
            counter = 0;
            nextType = (currentType == SpawnType.Resindential) ? SpawnType.City : SpawnType.Resindential; // toggle between residential and city
        }
    }

    public void ResetTransitionCounter()
    {
        counter = 0;
    }
}


