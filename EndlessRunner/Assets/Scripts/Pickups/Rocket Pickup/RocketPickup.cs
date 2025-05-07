using UnityEngine;

public class RocketPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
   
        GameObject rocketObj = this.gameObject;
        if (collision.gameObject.CompareTag("Player Hitbox"))
        {
            PickupLink link = GetComponent<PickupLink>();
            FindObjectOfType<PickupManager>().ActivatePickup(link);
            Destroy(rocketObj);
        }
    }
}
