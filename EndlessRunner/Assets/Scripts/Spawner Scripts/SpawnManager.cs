using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    RoadSpawner RoadSpawner;
    LandSpawner LandSpawner;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get the scripts for each spawner
        RoadSpawner = GetComponent<RoadSpawner>();
        LandSpawner = GetComponent<LandSpawner>();
    }

   

    public void SpawnTriggerEntered()
    {
        //slow down the destoruction of the roads if the speed pickup is applied
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
        LandSpawner.DestroyLand();

    }



    private void SpawnRoad()
    {
        RoadSpawner.MoveRoad();
    }
}


