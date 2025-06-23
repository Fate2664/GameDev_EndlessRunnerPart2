using UnityEngine;

public class DoublePointsPickup : MonoBehaviour
{
    //This script will be on the double points prefab
    //Once the player comes into contact with it, it calls to activate the pickup passing the double points scriptable object
    private void OnTriggerEnter(Collider collision)
    {
        GameObject DoublePointsObj = this.gameObject;
        if (collision.gameObject.CompareTag("Player Hitbox"))
        {
            PickupLink link = GetComponent<PickupLink>();
            FindFirstObjectByType<PickupManager>().OnDoublePointsPickup.Invoke(link.powerUp_Effect);
            AudioManager.Instance?.PlaySFX("DoublePointsPickup");
            Destroy(DoublePointsObj);
        }
    }
}
