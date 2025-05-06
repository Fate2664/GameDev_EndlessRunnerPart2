using UnityEngine;

public class DoublePointsPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        GameObject DoublePointsObj = this.gameObject;
        if (collision.gameObject.CompareTag("Player Hitbox"))
        {
            PickupLink link = GetComponent<PickupLink>();
            FindObjectOfType<PickupManager>().ActivatePickup(link);
            Destroy(DoublePointsObj);
        }
    }
}
