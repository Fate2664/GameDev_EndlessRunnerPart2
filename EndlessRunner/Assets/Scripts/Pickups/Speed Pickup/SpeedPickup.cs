using UnityEngine;

public class SpeedPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        GameObject speedobj = this.gameObject;
        if (collision.gameObject.CompareTag("Player Hitbox"))          //check if the player has hit the speed pickup
        {
            PickupLink link = GetComponent<PickupLink>();
            FindObjectOfType<PickupManager>().ActivatePickup(link);
            Destroy(speedobj);                          //destroy the speed pickup after it is picked up
        }
    }
}
