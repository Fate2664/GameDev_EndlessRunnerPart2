using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/RocketPickup")]
public class RocketEffect : PowerUp_Effect
{
    PowerUp_Effect powerUp;


    public override void ApplyEffect(GameObject target)
    {
        target.GetComponent<PlayerController>().maxSpeed = 150;
        target.GetComponent<PlayerDeath>().PlayerImmune = true;
    }

    public override void DisableEffect(GameObject target)
    {
        target.GetComponent<PlayerController>().maxSpeed = 100;
        target.GetComponent<PlayerDeath>().PlayerImmune = false;
    }
}


