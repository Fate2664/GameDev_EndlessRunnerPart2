using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    RoadSpawner roadSpawner;
    LandSpawner landSpawner;
    PickupManager pickupManager;
    ObstacleSpawner obstacleSpawner;
    void Start()
    {
        //get the scripts for each spawner
        roadSpawner = GetComponent<RoadSpawner>();
        landSpawner = GetComponent<LandSpawner>();
        obstacleSpawner = GetComponent<ObstacleSpawner>();
        roadSpawner.Initialize(obstacleSpawner);
    }



    public void SpawnTriggerEntered()
    {
        Invoke(nameof(SpawnRoad), 0.4f);    // call the road spawner
        //call for the land to be spawned and destroyed
        landSpawner.SpawnLand();
        Invoke(nameof(DestroyLands), 5f);

    }


    private void DestroyLands()
    {
        landSpawner.DestroyLand();
    }

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


