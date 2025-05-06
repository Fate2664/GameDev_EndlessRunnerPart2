using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Profiling;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> pickups;
    [SerializeField] private float spawnRate = 5f;
    [SerializeField] private float distanceAhead = 700f;

    private float[] laneXPositions = { -40f, 0, 40f };
    private float counter = 0f;
    private Transform player;


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
        if (counter >= spawnRate && pickups.Count > 0)
        {
            int randomIndex = Random.Range(0, pickups.Count);       //pick a random pickup from the list
            GameObject selectedPickup = pickups[randomIndex];

            PickupLink link = selectedPickup.GetComponent<PickupLink>();
            
            int pickupSpawnPoint = Random.Range(0, 3);      //randomly choose the lane in which to spawn the pickup
            Vector3 spawnPosition = new Vector3(laneXPositions[pickupSpawnPoint], link.powerUp_Effect.yHeight, player.position.z - distanceAhead);      //create a vector with the spawn position for the pickup
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
