using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private PlayerController playerController;
    public DeathScreen deathScreen;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController  = gameObject.GetComponent<PlayerController>();        //get the player's controller script
    }



    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))        //if the collision that the player had is with an obstacle
        {

            playerController.StrafeSpeed = 0;           //Don't allow the player to move
            if (this != null)
            {
                Destroy(this);                        //destroy the player game object
            }
            deathScreen.ShowDeathScreen();              //show the deathscreen

        }
    }
}
