using Unity.VisualScripting;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    //This script controlls when the player should die
    public DeathScreen deathScreen;
    public bool playerImmune = false;
    [HideInInspector]
    public bool isDead =false;



    private void OnTriggerEnter(Collider collision)
    {
        if ((collision.gameObject.CompareTag("Obstacle") && !playerImmune) || (collision.gameObject.CompareTag("NoSpawnTrigger")) && !playerImmune)        //if the collision that the player had is with an obstacle
        {
            if (this != null)
            {
                isDead = true;
            }
            deathScreen.ShowDeathScreen();              //show the deathscreen
        }
        else
        if ((collision.gameObject.CompareTag("Obstacle") && playerImmune) || (collision.gameObject.CompareTag("NoSpawnTrigger")&& playerImmune))
        {
           
            if (this != null)
            {
                Destroy(collision.gameObject); // Destroys Obstacles instead when boost effect is active
             
            }


        }

    }
}
