using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float StrafeSpeed = 10.0f;
    public static float moveH;
    public float MoveForwardSpeed = 300.0f;
    public SpawnManager spawnManager;
    public Score scoreManager;
    private int desiredLane = 1; //0 = left lane; 1 = right lane
    public float laneDistance = 20.0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            desiredLane--;      //change the desired lane
            if (desiredLane < 0)
            {
                desiredLane = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            desiredLane++;      //change the desired lane
            if (desiredLane > 1)
            {
                desiredLane = 1;
            }
        }

    }

    void FixedUpdate()
    {
        MoveCharacter();    //call the MoveCharacter method

    }

    private void MoveCharacter()
    {
        Vector3 targetPos = transform.position.z * Vector3.forward; //Make the target position infront of the player
        if (desiredLane == 0)
        {
            targetPos -= Vector3.left * laneDistance;       //change the target position's x position to the corresponding lane
        }
        else if (desiredLane == 1)
        {
            targetPos -= Vector3.right * laneDistance;
        }

        Vector3 moveDirection = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * StrafeSpeed);      //make the switching lanes smoother
        transform.position = new Vector3(moveDirection.x, transform.position.y, transform.position.z - (MoveForwardSpeed * Time.deltaTime));        //move the player to the new postiion
    }

    private void OnTriggerEnter(Collider collision)
    {

        //if the player collides with an object, call the corresponding method for that event
        if (collision.CompareTag("RoadSpawn"))
        {
            spawnManager.SpawnTriggerEntered();

        }
        if (collision.CompareTag("StaticObstacleTrigger"))
        {
            scoreManager.IncrementScore();
        }
        if (collision.CompareTag("MovingObstacleTrigger"))
        {
            scoreManager.IncrementScore();
        }
    }
}