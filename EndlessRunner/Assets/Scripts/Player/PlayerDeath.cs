using Unity.VisualScripting;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    //This script controlls when the player should die
    private PlayerController playerController;
    public DeathScreen deathScreen;
    public bool PlayerImmune;
    [HideInInspector]
    public bool isDead =false;



    void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();        //get the player's controller script
    }



    private void OnTriggerEnter(Collider collision)
    {
        if ((collision.gameObject.CompareTag("Obstacle")) || (collision.gameObject.CompareTag("NoSpawnTrigger")) && !PlayerImmune)        //if the collision that the player had is with an obstacle
        {
            if (this != null)
            {
                isDead = true;
                Destroy(this);                        //destroy the player game object
            }
            deathScreen.ShowDeathScreen();              //show the deathscreen
        }
        else
        if (collision.gameObject.CompareTag("Obstacle") && PlayerImmune)
        {
           
            if (this != null)
            {
                Destroy(collision.gameObject); // Destroys Obstacles instead when boost effect is active
             
            }


        }

    }
}
