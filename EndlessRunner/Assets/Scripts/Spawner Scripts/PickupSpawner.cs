using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    private float xPosLeft = 20f;       //left lane x position
    private float xPosRight = -20f;     //right lane x position
    private float distanceAhead = 700f;
    private float yHeight = 10f;
    private float counter = 0f;
    private float spawnRate = 5f;
    public List<GameObject> pickups;
    private Transform player;
    private Vector3 spawnPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;        //get the player's position in the game
    }

    // Update is called once per frame
    void Update()
    {
        SpawnPickup();
    }

    public void SpawnPickup()
    {
        counter += Time.deltaTime;
        int randomIndex = Random.Range(0, pickups.Count);       //pick a random pickup from the list
        if (counter >= spawnRate)
        {
            if (pickups.Count > 0)
            {
                int pickupSpawnPoint = Random.Range(0, 3);      //randomly choose the lane in which to spawn the pickup
                if (pickupSpawnPoint == 0)
                {
                    spawnPosition = new Vector3(xPosLeft, yHeight, player.position.z - distanceAhead);      //create a vector with the spawn position for the pickup
                }
                else if (pickupSpawnPoint == 1)
                {
                    spawnPosition = new Vector3(xPosRight, yHeight, player.position.z - distanceAhead);
                }

                GameObject pickupSpawned = Instantiate(pickups[randomIndex], spawnPosition, Quaternion.identity);       //create a clone of the pickup that is chosen 
                pickupSpawned.transform.SetParent(transform, false);
                if (pickupSpawned != null)
                {
                    Destroy(pickupSpawned, 5f);      //destroy that clone after 5 increment points have past
                }

                counter = 0f;           //reset the counter
            }
        }
    }
}
