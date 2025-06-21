using UnityEngine;

public class RocketPickup : MonoBehaviour
{

    private void OnTriggerEnter(Collider collision)
    {
   
        GameObject rocketObj = this.gameObject;
        if (collision.gameObject.CompareTag("Player Hitbox"))
        {
            PickupLink link = GetComponent<PickupLink>();
            FindFirstObjectByType<PickupManager>().OnRocketPickup.Invoke(link.powerUp_Effect);
            Destroy(rocketObj);
        }
    }
}
