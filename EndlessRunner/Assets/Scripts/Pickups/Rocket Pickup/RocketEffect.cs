using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/RocketPickup")]
public class RocketEffect : PowerUp_Effect
{
    public override void ApplyEffect(GameObject target)
    {
        target.GetComponent<PlayerController>().maxSpeed = 150;
    }

    public override void DisableEffect(GameObject target)
    {
        target.GetComponent<PlayerController>().maxSpeed = 75;
    }
}
    

