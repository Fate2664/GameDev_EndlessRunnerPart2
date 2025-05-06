using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private PlayerController playerController;
    public DeathScreen deathScreen;
    public bool PlayerImmune;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();        //get the player's controller script
    }



    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && !PlayerImmune)        //if the collision that the player had is with an obstacle
        {
            if (this != null)
            {
                Destroy(this);                        //destroy the player game object
            }
            deathScreen.ShowDeathScreen();              //show the deathscreen
        }

        if (collision.gameObject.CompareTag("Obstacle") && PlayerImmune)
        {
           
            if (this != null)
            {
                Destroy(collision.gameObject); // Destroys Obstacles instead when boost effect is active
             
            }


        }

    }
}
