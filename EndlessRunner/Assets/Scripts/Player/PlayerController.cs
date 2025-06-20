using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    //This script holds all the settings and setup for how the player's car will be controlled
    #region Settings
    [Space(20)]
    [Header("CAR SETUP")]
    [Space(10)]
    [Range(20, 1000)]
    [SerializeField] private int _maxSpeed = 200; //The maximum speed that the car can reach in km/h.
    public int maxSpeed { get { return _maxSpeed; } set { _maxSpeed = value; } }
    [Range(1, 10000)]
    [SerializeField] private int _accelerationMultiplier = 2; // How fast the car can accelerate. 
    public int accelerationMultiplier { get { return _accelerationMultiplier; } }
    [Range(10, 45)]
    [SerializeField] private int _maxSteeringAngle = 27; // The maximum angle that the tires can reach while rotating the steering wheel.
    public int maxSteeringAngle { get { return _maxSteeringAngle; } set { _maxSteeringAngle = value; } }
    [Space(10)]
    [SerializeField] private Vector3 _bodyMassCenter;
    public Vector3 bodyMassCenter { get { return _bodyMassCenter; } }

    [Space(10)]
    [Header("LANE SETUP")]
    [Space(10)]
    [Range(10, 100)]
    [SerializeField] private float _laneDistance = 20f;
    public float laneDistance { get { return _laneDistance; } }
    [Range(0, 10)]
    [SerializeField] private float _centeringForce = 0.5f;
    public float centeringForce { get { return _centeringForce; } }
    [Range(0, 0.05f)]
    [SerializeField] private float _dampingForce = 0.5f;
    public float dampingForce { get { return _dampingForce; } }


    [Header("WHEELS")]
    [Space(10)]
    [SerializeField] private GameObject _frontLeftMesh;
    public GameObject frontLeftMesh { get { return _frontLeftMesh; } }
    [SerializeField] private WheelCollider _frontLeftCollider;
    public WheelCollider frontLeftCollider { get { return _frontLeftCollider; } }
    [Space(10)]
    [SerializeField] private GameObject _frontRightMesh;
    public GameObject frontRightMesh { get { return _frontRightMesh; } }
    [SerializeField] private WheelCollider _frontRightCollider;
    public WheelCollider frontRightCollider { get { return _frontRightCollider; } }
    [Space(10)]
    [SerializeField] private GameObject _rearLeftMesh;
    public GameObject rearLeftMesh { get { return _rearLeftMesh; } }
    [SerializeField] private WheelCollider _rearLeftCollider;
    public WheelCollider rearLeftCollider { get { return _rearLeftCollider; } }
    [Space(10)]
    [SerializeField] private GameObject _rearRightMesh;
    public GameObject rearRightMesh { get { return _rearRightMesh; } }
    [SerializeField] private WheelCollider _rearRightCollider;
    public WheelCollider rearRightCollider { get { return _rearRightCollider; } }

    [Space(20)]
    [Header("EFFECTS")]
    [Space(10)]
    //The following variable lets you to set up particle systems in your 
    [SerializeField] private bool _useEffects = false;
    public bool useEffects { get { return _useEffects; } }
    // The following particle systems are used as tire smoke when the car drifts and exhaust flames when the player picks up a rocket boost.
    [SerializeField] private ParticleSystem _RLWParticleSystem;
    public ParticleSystem RLWParticleSystem { get { return _RLWParticleSystem; } }
    [SerializeField] private ParticleSystem _RRWParticleSystem;
    public ParticleSystem RRWParticleSystem { get { return _RRWParticleSystem; } }

    [Space(10)]
    [SerializeField] private ParticleSystem _LeftExhaustFlame;
    public ParticleSystem LeftExhaustFlame { get { return _LeftExhaustFlame; } }
    [SerializeField] private ParticleSystem _RightExhaustFlame;
    public ParticleSystem RightExhaustFlame { get { return _RightExhaustFlame; } }
    [SerializeField] private GameObject shield;
    public GameObject Shield { get { return shield; } }

    [Space(10)]
    // The following trail renderers are used as tire skids when the car loses traction.
    [SerializeField] private TrailRenderer _RLWTireSkid;
    public TrailRenderer RLWTireSkid { get { return _RLWTireSkid; } }
    [SerializeField] private TrailRenderer _RRWTireSkid;
    public TrailRenderer RRWTireSkid { get { return _RRWTireSkid; } }


    [Space(20)]
    [Header("UI")]
    [Space(10)]
    //The following variable lets you to set up a UI text to display the speed of your car.
    [SerializeField] private bool _useUI = false;
    public bool useUI { get { return _useUI; } }
    [SerializeField] private TextMeshPro _carSpeedText; // Used to store the UI object that is going to show the speed of the car.
    public TextMeshPro carSpeedText { get { return _carSpeedText; } }

    [Space(20)]
    [Header("Sounds")]
    [Space(10)]
    //The following variable lets you to set up sounds for your car such as the car engine or tire screech sounds.
    [SerializeField] private bool _useSounds = false;
    public bool useSounds { get { return _useSounds; } }
    [SerializeField] private AudioSource _carEngineSound; // This variable stores the sound of the car engine.
    public AudioSource carEngineSound { get { return _carEngineSound; } }
    [SerializeField] private AudioSource _tireScreechSound; // This variable stores the sound of the tire screech (when the car is drifting).
    public AudioSource tireScreechSound { get { return _tireScreechSound; } }

    [HideInInspector]
    public float carSpeed; // Used to store the speed of the car.
    [HideInInspector]
    public bool isDrifting; // Used to know whether the car is drifting or not.
    [HideInInspector]
    public bool isTractionLocked; // Used to know whether the traction of the car is locked or not.
    [Space(20)]

    [Header("Connections")]
    [Space(10)]
    [SerializeField] private PrometeoCarController prometeoCarController;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private Score scoreManager;
    [SerializeField] private GameObject pauseScreenObject;

    #endregion


    private Rigidbody _carRigidbody;
    public Rigidbody carRigidbody { get { return _carRigidbody; } }

    private float _localVelocityZ;
    public float localVelocityZ { get { return _localVelocityZ; } }
    private float _localVelocityX;
    public float localVelocityX { get { return _localVelocityX; } }


    private int desiredLane = 0; //0 = left lane; 1 = right lane
    private PauseScreen pauseScreen;
    private PlayerDeath playerDeath;
    private void Start()
    {
         pauseScreen = pauseScreenObject.GetComponent<PauseScreen>();
        playerDeath = this.GetComponent<PlayerDeath>();
        _carRigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (playerDeath.isDead)
        {
            return; // If the player is dead, do not allow movement.
        }

        MoveCharacter();    //call the MoveCharacter method
        if (!prometeoCarController.isSwitchingLane)
        {
            prometeoCarController.KeepCarInLane();
        }
        PauseGame();
    }



    private void MoveCharacter()
    {
        if (playerDeath.isDead)
        {
            return; // If the player is dead, do not allow movement.
        }

        // We determine the speed of the car.
        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 2000;
        // Save the local velocity of the car in the x axis. Used to know if the car is drifting.
        _localVelocityX = transform.InverseTransformDirection(carRigidbody.linearVelocity).x;
        // Save the local velocity of the car in the z axis. Used to know if the car is going forward or backwards.
        _localVelocityZ = transform.InverseTransformDirection(carRigidbody.linearVelocity).z;

        prometeoCarController.CancelInvoke("DecelerateCar");
        prometeoCarController.deceleratingCar = false;
        prometeoCarController.GoForward();

        //This is to change the desired lane variable when the player wants to change lanes
        if (Input.GetKey(KeyCode.A) && !prometeoCarController.isSwitchingLane && prometeoCarController.canChangeLanes)
        {

            desiredLane--;      //change the desired lane
            if (desiredLane < 0)
            {
                desiredLane = 0;
            }

            prometeoCarController.LaneChange(desiredLane);

        }
        if (Input.GetKey(KeyCode.D) && !prometeoCarController.isSwitchingLane && prometeoCarController.canChangeLanes)
        {

            desiredLane++;      //change the desired lane
            if (desiredLane > 2)
            {
                desiredLane = 2;
            }

            prometeoCarController.LaneChange(desiredLane);

        }

        // We call the method AnimateWheelMeshes() in order to match the wheel collider movements with the 3D meshes of the wheels.
        prometeoCarController.AnimateWheelMeshes();
    }

    //This method will execute when the player comes in contact with a trigger
    private void OnTriggerEnter(Collider collision)
    {

        if (collision.CompareTag("RoadSpawn"))
        {
            spawnManager.SpawnTriggerEntered();

        }
        else if (collision.CompareTag("StaticObstacleTrigger") || (collision.CompareTag("MovingObstacleTrigger")))
        {
            scoreManager.IncrementScore();
        }



    }
    //this method is used in the speed altering pickups to change the cars travel speed
    public void SetMaxSpeed(int Newspeed)
    {
        _maxSpeed = Newspeed;
    }

    private void PauseGame()
    {
        if (Input.GetKey(KeyCode.Escape) && !playerDeath.isDead)
        {
           pauseScreen.ActivatePauseScreen();
        }
    }


}