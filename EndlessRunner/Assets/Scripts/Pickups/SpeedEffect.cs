using UnityEngine;
[CreateAssetMenu (menuName = "PowerUp/SpeedPickup")]        //add to the menu to create this object
public class SpeedEffect : PowerUp_Effect
{
    public float SpeedNerf;

    public override void ApplyEffect(GameObject target)
    {
        //target.GetComponent<PlayerController>(). = SpeedNerf;       //change the player's movement speed to the given value
    }

    public override void DisableEffect(GameObject target)
    {
        //target.GetComponent<PlayerController>().MoveForwardSpeed = 300;         //change the player's movemennt speed back to the original
    }

}
