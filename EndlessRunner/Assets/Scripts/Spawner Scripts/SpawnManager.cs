using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    RoadSpawner RoadSpawner;
    LandSpawner LandSpawner;
    void Start()
    {
        //get the scripts for each spawner
        RoadSpawner = GetComponent<RoadSpawner>();
        LandSpawner = GetComponent<LandSpawner>();
    }

   

    public void SpawnTriggerEntered()
    {
        //slow down the destruction of the roads if the speed pickup is applied
        if (PickupManager.PowerUpCheck && PickupManager.pickup == "SpeedPickup")
        {
            Invoke(nameof(SpawnRoad), 1f);
        }
        else
        {
            Invoke(nameof(SpawnRoad), 0.4f);    //else call the road spawner
        }
        //call for the land to be spawned and destroyed
        LandSpawner.SpawnLand();
        Invoke(nameof(DestroyLands),5f);

    }

    
    private void DestroyLands()
    {
        LandSpawner.DestroyLand();
    }
    private void SpawnRoad()
    {
        RoadSpawner.MoveRoad();
    }
}


