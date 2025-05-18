using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //This script manages all the spawning scripts together and when they should be executed

    RoadSpawner roadSpawner;
    LandSpawner landSpawner;
    PickupManager pickupManager;
    ObstacleSpawner obstacleSpawner;
    BossSpawner bossSpawner;

    
    void Start()
    {
        //get the scripts for each spawner
        roadSpawner = GetComponent<RoadSpawner>();
        landSpawner = GetComponent<LandSpawner>();
        obstacleSpawner = GetComponent<ObstacleSpawner>();
        bossSpawner = GetComponent<BossSpawner>();
        roadSpawner.InitializeObsSpawener(obstacleSpawner);
    }



    public void SpawnTriggerEntered()
    {
        Invoke(nameof(SpawnRoad), 0.4f);    // call the road spawner
        //call for the land to be spawned and destroyed
        landSpawner.SpawnLand();
        Invoke(nameof(DestroyLands), 1f);

    }


    private void DestroyLands()
    {
        landSpawner.DestroyLand();
    }

    //This method decides wether or not to move a normal road prefab or spawn a construction road prefab
    private void SpawnRoad()
    {
        if (obstacleSpawner.spawningConstrRoad)
        {
            roadSpawner.SpawnNextConstructionRoad();
        }
        else
        {
            roadSpawner.MoveNormalRoad();
        }
    }
}


