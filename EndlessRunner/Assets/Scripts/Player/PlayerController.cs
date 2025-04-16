using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class PlayerController : MonoBehaviour
{
    [Space(20)]
    [Header("CAR SETUP")]
    [Space(10)]
    [Range(20, 190)]
    public int maxSpeed = 90; //The maximum speed that the car can reach in km/h.
    [Range(0,10)]
    public float StrafeSpeed = 10.0f;
    [Range(10, 120)]
    public int maxReverseSpeed = 45; //The maximum speed that the car can reach while going on reverse in km/h.
    [Range(1, 10)]
    public int accelerationMultiplier = 2; // How fast the car can accelerate. 1 is a slow acceleration and 10 is the fastest.
    [Space(10)]
    [Range(10, 45)]
    public int maxSteeringAngle = 27; // The maximum angle that the tires can reach while rotating the steering wheel.
    [Range(0.1f, 1f)]
    public float steeringSpeed = 0.5f; // How fast the steering wheel turns.
    [Space(10)]
    [Range(100, 600)]
    public int brakeForce = 350; // The strength of the wheel brakes.
    [Range(1, 10)]
    public int decelerationMultiplier = 2; // How fast the car decelerates when the user is not using the throttle.
    [Range(1, 10)]
    public int handbrakeDriftMultiplier = 5; // How much grip the car loses when the user hit the handbrake.
    [Space(10)]
    public Vector3 bodyMassCenter;

    [Header("WHEELS")]
    public GameObject frontLeftMesh;
    public WheelCollider frontLeftCollider;
    [Space(10)]
    public GameObject frontRightMesh;
    public WheelCollider frontRightCollider;
    [Space(10)]
    public GameObject rearLeftMesh;
    public WheelCollider rearLeftCollider;
    [Space(10)]
    public GameObject rearRightMesh;
    public WheelCollider rearRightCollider;

    [Space(20)]
    [Header("EFFECTS")]
    [Space(10)]
    //The following variable lets you to set up particle systems in your car
    public bool useEffects = false;

    // The following particle systems are used as tire smoke when the car drifts.
    public ParticleSystem RLWParticleSystem;
    public ParticleSystem RRWParticleSystem;

    [Space(10)]
    // The following trail renderers are used as tire skids when the car loses traction.
    public TrailRenderer RLWTireSkid;
    public TrailRenderer RRWTireSkid;

   
    [Space(20)]
    [Header("UI")]
    [Space(10)]
    //The following variable lets you to set up a UI text to display the speed of your car.
    public bool useUI = false;
    public TextMeshPro carSpeedText; // Used to store the UI object that is going to show the speed of the car.

    [Space(20)]
    [Header("Sounds")]
    [Space(10)]
    //The following variable lets you to set up sounds for your car such as the car engine or tire screech sounds.
    public bool useSounds = false;
    public AudioSource carEngineSound; // This variable stores the sound of the car engine.
    public AudioSource tireScreechSound; // This variable stores the sound of the tire screech (when the car is drifting).



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
        /*
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
        */

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