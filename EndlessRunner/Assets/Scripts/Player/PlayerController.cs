using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;
using UnityEditor;
using DG.Tweening;
using Unity.VisualScripting;
public class PlayerController : MonoBehaviour
{
    #region Settings
    [Space(20)]
    [Header("CAR SETUP")]
    [Space(10)]
    [Range(20, 1000)]
    [SerializeField] private int _maxSpeed = 200; //The maximum speed that the car can reach in km/h.
    public int maxSpeed { get { return _maxSpeed; } }
    [Range(0, 10)]
    [SerializeField] private float _strafeSpeed = 10.0f;
    public float StrafeSpeed { get { return _strafeSpeed; } set { _strafeSpeed = value; } }
    [Range(10, 120)]
    [SerializeField] private int _maxReverseSpeed = 45;
    public int maxReverseSpeed { get { return _maxReverseSpeed; } } //The maximum speed that the car can reach while going on reverse in km/h.
    [Range(1, 10000)]
    [SerializeField] private int _accelerationMultiplier = 2; // How fast the car can accelerate. 1 is a slow acceleration and 10 is the fastest.
    public int accelerationMultiplier { get { return _accelerationMultiplier; } }
    [Space(10)]
    [Range(10, 45)]
    [SerializeField] private int _maxSteeringAngle = 27; // The maximum angle that the tires can reach while rotating the steering wheel.
    public int maxSteeringAngle { get { return _maxSteeringAngle; } set { _maxSteeringAngle = value; } }
    [Range(0.1f, 1f)]
    [SerializeField] private float _steeringSpeed = 0.5f; // How fast the steering wheel turns.
    public float steeringSpeed { get { return _steeringSpeed; } set { _steeringSpeed = value; } }
    [Space(10)]
    [Range(100, 600)]
    [SerializeField] private int _brakeForce = 350; // The strength of the wheel brakes.
    public int brakeForce { get { return _brakeForce; } }
    [Range(1, 10)]
    [SerializeField] private int _decelerationMultiplier = 2; // How fast the car decelerates when the user is not using the throttle.
    public int decelerationMultiplier { get { return _decelerationMultiplier; } }
    [Range(1, 10)]
    [SerializeField] private int _handbrakeDriftMultiplier = 5; // How much grip the car loses when the user hit the handbrake.
    public int handbrakeDriftMultiplier { get { return _handbrakeDriftMultiplier; } }
    [Space(10)]
    [SerializeField] private Vector3 _bodyMassCenter;
    public Vector3 bodyMassCenter { get { return _bodyMassCenter; } }

    [Space(10)]
    [Header("LANE SETUP")]
    [Space(10)]
    [Range(10, 100)]
    [SerializeField] private float _laneDistance = 20f;
    public float laneDistance { get { return _laneDistance; } }
    [Range(0, 5)]
    [SerializeField] private float _centeringForce = 0.5f;
    public float centeringForce { get { return _centeringForce; } }
    [Range(0, 1)]
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
    // The following particle systems are used as tire smoke when the car drifts.
    [SerializeField] private ParticleSystem _RLWParticleSystem;
    public ParticleSystem RLWParticleSystem { get { return _RLWParticleSystem; } }
    [SerializeField] private ParticleSystem _RRWParticleSystem;
    public ParticleSystem RRWParticleSystem { get { return _RRWParticleSystem; } }

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

    #endregion


    private Rigidbody _carRigidbody;
    public Rigidbody carRigidbody { get { return _carRigidbody; } }

    private float _localVelocityZ;
    public float localVelocityZ { get { return _localVelocityZ; } }
    private float _localVelocityX;
    public float localVelocityX { get { return _localVelocityX; } }


    private int desiredLane = 0; //0 = left lane; 1 = right lane

    private void Start()
    {
        _carRigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        MoveCharacter();    //call the MoveCharacter method
        if (!prometeoCarController.isSwitchingLane)
        {
            prometeoCarController.KeepCarInLane();
        }
    }

    private void MoveCharacter()
    {
        // We determine the speed of the car.
        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 2000;
        // Save the local velocity of the car in the x axis. Used to know if the car is drifting.
        _localVelocityX = transform.InverseTransformDirection(carRigidbody.linearVelocity).x;
        // Save the local velocity of the car in the z axis. Used to know if the car is going forward or backwards.
        _localVelocityZ = transform.InverseTransformDirection(carRigidbody.linearVelocity).z;

        prometeoCarController.CancelInvoke("DecelerateCar");
        prometeoCarController.deceleratingCar = false;
        prometeoCarController.GoForward();




        if (Input.GetKey(KeyCode.S))
        {
            prometeoCarController.CancelInvoke("DecelerateCar");
            prometeoCarController.deceleratingCar = false;
            prometeoCarController.GoReverse();
        }

        if (Input.GetKey(KeyCode.A) && !prometeoCarController.isSwitchingLane)
        {

            desiredLane--;      //change the desired lane
            if (desiredLane < 0)
            {
                desiredLane = 0;
            }
           
            prometeoCarController.LaneChange(desiredLane);

        }
        if (Input.GetKey(KeyCode.D) && !prometeoCarController.isSwitchingLane)
        {

            desiredLane++;      //change the desired lane
            if (desiredLane > 1)
            {
                desiredLane = 1;
            }
            
            prometeoCarController.LaneChange(desiredLane);

        }

        // We call the method AnimateWheelMeshes() in order to match the wheel collider movements with the 3D meshes of the wheels.
        prometeoCarController.AnimateWheelMeshes();
    }


    private void OnTriggerEnter(Collider collision)
    {
        //if the player collides with an object, call the corresponding method for that event
        if (collision.CompareTag("RoadSpawn"))
        {
            spawnManager.SpawnTriggerEntered();

        }
        else if (collision.CompareTag("StaticObstacleTrigger") || (collision.CompareTag("MovingObstacleTrigger")))
        {
            scoreManager.IncrementScore();
        }


    }


}