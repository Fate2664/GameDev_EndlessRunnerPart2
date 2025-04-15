using UnityEngine;

public class SpeedPickup : MonoBehaviour
{

    private void OnTriggerEnter(Collider collision)
    {
        GameObject speedobj = this.gameObject;
        if (collision.gameObject.CompareTag("Player"))          //check if the player has hit the speed pickup
        {
            PickupManager.pickup = "SpeedPickup";       //assign the pickup that is being affected
            PickupManager.PowerUpCheck = true;          //change the power check to active
            Destroy(speedobj);                          //destroy the speed pickup after it is picked up
        }
    }
}
