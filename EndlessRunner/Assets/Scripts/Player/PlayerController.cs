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
    [Tooltip("One-shot sound played when the car moves sideways into another lane.")]
    [SerializeField] private AudioClip _laneChangeScreechClip;
    [Range(0f, 1f)]
    [SerializeField] private float _laneChangeScreechVolume = 0.22f;
    [Tooltip("Sideways speed at which the tire screech begins to be audible.")]
    [Range(0f, 100f)]
    [SerializeField] private float _laneChangeScreechMinSidewaysSpeed = 10f;
    [Tooltip("Sideways speed at which the tire screech reaches full volume.")]
    [Range(1f, 150f)]
    [SerializeField] private float _laneChangeScreechMaxSidewaysSpeed = 80f;
    [Range(0.1f, 5f)]
    [SerializeField] private float _laneChangeScreechFadeSpeed = 2.5f;

    [Space(10)]
    [Header("ENGINE SIMULATION")]
    [Tooltip("Number of forward gears used to calculate engine RPM.")]
    [Range(2, 8)]
    [SerializeField] private int _forwardGearCount = 5;
    [Tooltip("Engine speed while the car is stationary.")]
    [Range(500f, 2000f)]
    [SerializeField] private float _idleRpm = 850f;
    [Tooltip("Engine speed reached at the top of each gear.")]
    [Range(3000f, 10000f)]
    [SerializeField] private float _redlineRpm = 6500f;
    [Tooltip("RPM immediately after an automatic upshift.")]
    [Range(1000f, 5000f)]
    [SerializeField] private float _upshiftRpm = 2800f;
    [Tooltip("RPM at which the automatic gearbox changes up.")]
    [Range(2500f, 9500f)]
    [SerializeField] private float _shiftRpm = 6100f;
    [Tooltip("How quickly the sound follows changes in engine RPM.")]
    [Range(1000f, 20000f)]
    [SerializeField] private float _rpmResponse = 9000f;
    [SerializeField] private float _idlePitch = 0.72f;
    [SerializeField] private float _redlinePitch = 1.42f;
    [Range(0f, 1f)]
    [SerializeField] private float _idleVolume = 0.12f;
    [Range(0f, 1f)]
    [SerializeField] private float _maxEngineVolume = 0.28f;

    [Space(10)]
    [Header("CRUISING VARIATION")]
    [Tooltip("Minimum fraction of top speed before simulated cruise shifts can occur.")]
    [Range(0.5f, 1f)]
    [SerializeField] private float _cruiseShiftSpeedFraction = 0.9f;
    [Tooltip("Time spent cruising before another simulated shift is played.")]
    [Range(3f, 30f)]
    [SerializeField] private float _cruiseShiftInterval = 9f;
    [Tooltip("How long the engine takes to rev back to cruising RPM after a simulated shift.")]
    [Range(0.5f, 6f)]
    [SerializeField] private float _cruiseShiftDuration = 2.25f;
    [Tooltip("RPM immediately after a simulated cruise shift.")]
    [Range(1000f, 5000f)]
    [SerializeField] private float _cruiseShiftRpm = 3300f;

    [HideInInspector] public int currentGear = 1;
    [HideInInspector] public float engineRpm;

    [HideInInspector]
    public float carSpeed; // Used to store the speed of the car.
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

    private bool _wasEngineSoundEnabled;
    private float _gearShiftCooldown;
    private float _cruiseShiftTimer;
    private float _cruiseShiftElapsed;
    private bool _isCruiseShifting;
    private AudioSource _laneChangeScreechSource;
    private Vector3 _previousFramePosition;


    private int desiredLane = 0; //0 = left lane; 1 = right lane
    private PauseScreen pauseScreen;
    private PlayerDeath playerDeath;

    private void Awake()
    {
        // The car controller can be on another object (as it is in the gameplay
        // scene) or on this prefab (as it is in the trailer). Cache the physics
        // component before either controller attempts its setup.
        _carRigidbody = GetComponent<Rigidbody>();
        playerDeath = GetComponent<PlayerDeath>();
        SetupLaneChangeScreech();
        ResolveCarController();
    }

    private void Start()
    {
        if (pauseScreenObject != null)
        {
            pauseScreen = pauseScreenObject.GetComponent<PauseScreen>();
        }

        // Keep the cutscene scene independent from gameplay-only references.
        // TrailerScene does not need a pause screen or spawn manager to drive.
        if (playerDeath == null)
        {
            playerDeath = GetComponent<PlayerDeath>();
        }

        if (_carRigidbody == null)
        {
            _carRigidbody = GetComponent<Rigidbody>();
        }

        ResolveCarController();
        _previousFramePosition = transform.position;
        StartEngineSound();
    }

    private bool ResolveCarController()
    {
        if (prometeoCarController == null)
        {
            prometeoCarController = GetComponent<PrometeoCarController>();
        }

        if (prometeoCarController == null)
        {
            Debug.LogError($"{name} needs a {nameof(PrometeoCarController)} to drive.", this);
            return false;
        }

        // Public controller methods can be driven by this component even when
        // the controller itself is disabled on a cutscene prefab.
        prometeoCarController.Initialize(this);
        return true;
    }

    private bool IsPlayerDead()
    {
        return playerDeath != null && playerDeath.isDead;
    }

    private void Update()
    {
        if (IsPlayerDead() || _carRigidbody == null)
        {
            return; // If the player is dead, do not allow movement.
        }

        if (prometeoCarController == null && !ResolveCarController())
        {
            return;
        }

        MoveCharacter();    //call the MoveCharacter method
        UpdateEngineSound();
        UpdateLaneChangeScreech();
        if (!prometeoCarController.isSwitchingLane)
        {
            prometeoCarController.KeepCarInLane();
        }
        PauseGame();
    }



    private void MoveCharacter()
    {
        if (IsPlayerDead() || _carRigidbody == null || prometeoCarController == null)
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

    private void StartEngineSound()
    {
        if (!_useSounds || _carEngineSound == null)
        {
            return;
        }

        _carEngineSound.loop = true;
        engineRpm = Mathf.Clamp(_idleRpm, 1f, _redlineRpm);
        currentGear = 1;
        _cruiseShiftTimer = 0f;
        _cruiseShiftElapsed = 0f;
        _isCruiseShifting = false;
        _carEngineSound.pitch = _idlePitch;
        _carEngineSound.volume = _idleVolume;

        if (!_carEngineSound.isPlaying)
        {
            _carEngineSound.Play();
        }

        _wasEngineSoundEnabled = true;
    }

    private void SetupLaneChangeScreech()
    {
        if (_laneChangeScreechClip == null)
        {
            return;
        }

        _laneChangeScreechSource = gameObject.AddComponent<AudioSource>();
        _laneChangeScreechSource.clip = _laneChangeScreechClip;
        _laneChangeScreechSource.playOnAwake = false;
        _laneChangeScreechSource.loop = true;
        _laneChangeScreechSource.spatialBlend = 0f;
        _laneChangeScreechSource.volume = 0f;
    }

    // Mirrors the tire-smoke condition: screech while changing lanes or while the car is still drifting to correct itself.
    private void UpdateLaneChangeScreech()
    {
        if (_laneChangeScreechSource == null)
        {
            return;
        }

        float frameDuration = Mathf.Max(Time.deltaTime, 0.0001f);
        Vector3 frameVelocity = (transform.position - _previousFramePosition) / frameDuration;
        _previousFramePosition = transform.position;

        float measuredSidewaysSpeed = Mathf.Abs(transform.InverseTransformDirection(frameVelocity).x);
        float physicsSidewaysSpeed = Mathf.Abs(_localVelocityX);
        float sidewaysSpeed = Mathf.Max(measuredSidewaysSpeed, physicsSidewaysSpeed);
        bool isChangingLane = prometeoCarController != null && prometeoCarController.isSwitchingLane;
        bool isMakingTireSmoke = isDrifting;
        float screechAmount = _useSounds && (isChangingLane || isMakingTireSmoke)
            ? Mathf.InverseLerp(_laneChangeScreechMinSidewaysSpeed, _laneChangeScreechMaxSidewaysSpeed, sidewaysSpeed)
            : 0f;
        float targetVolume = _laneChangeScreechVolume * screechAmount;

        if (targetVolume > 0f && !_laneChangeScreechSource.isPlaying)
        {
            _laneChangeScreechSource.Play();
        }

        _laneChangeScreechSource.volume = Mathf.MoveTowards(
            _laneChangeScreechSource.volume,
            targetVolume,
            _laneChangeScreechFadeSpeed * Time.deltaTime);

        if (targetVolume <= 0f && _laneChangeScreechSource.volume <= 0f && _laneChangeScreechSource.isPlaying)
        {
            _laneChangeScreechSource.Stop();
        }
    }

    // Simulates a simple automatic gearbox. RPM rises through each gear and falls on an upshift.
    private void UpdateEngineSound()
    {
        if (!_useSounds || _carEngineSound == null)
        {
            if (_wasEngineSoundEnabled && _carEngineSound != null)
            {
                _carEngineSound.Stop();
            }

            _wasEngineSoundEnabled = false;
            return;
        }

        if (!_wasEngineSoundEnabled)
        {
            StartEngineSound();
        }

        if (!_carEngineSound.isPlaying)
        {
            _carEngineSound.Play();
        }

        float speedKph = Mathf.Abs(transform.InverseTransformDirection(_carRigidbody.linearVelocity).z) * 3.6f;
        float usableMaxSpeed = Mathf.Max(1f, _maxSpeed);
        float gearStartSpeed = (currentGear - 1) * usableMaxSpeed / _forwardGearCount;
        float gearEndSpeed = currentGear * usableMaxSpeed / _forwardGearCount;
        float gearProgress = Mathf.InverseLerp(gearStartSpeed, gearEndSpeed, speedKph);
        float targetRpm = Mathf.Lerp(_upshiftRpm, _redlineRpm, gearProgress);
        if (currentGear == 1)
        {
            targetRpm = Mathf.Lerp(_idleRpm, _redlineRpm, gearProgress);
        }

        targetRpm = Mathf.Clamp(targetRpm, _idleRpm, _redlineRpm);
        engineRpm = Mathf.MoveTowards(engineRpm, targetRpm, _rpmResponse * Time.deltaTime);

        // The car accelerates automatically and can pass several speed bands in a single frame.
        // Shift one gear at a time after each audible rev, rather than jumping directly to top gear.
        _gearShiftCooldown = Mathf.Max(0f, _gearShiftCooldown - Time.deltaTime);
        float shiftPoint = Mathf.Clamp(_shiftRpm, _idleRpm, _redlineRpm);
        if (currentGear < _forwardGearCount && speedKph >= gearEndSpeed && engineRpm >= shiftPoint && _gearShiftCooldown <= 0f)
        {
            currentGear++;
            engineRpm = Mathf.Clamp(_upshiftRpm, _idleRpm, _redlineRpm);
            _gearShiftCooldown = 0.12f;
        }
        else if (currentGear > 1 && speedKph < gearStartSpeed - 2f)
        {
            currentGear--;
        }

        UpdateCruiseShift(speedKph, usableMaxSpeed, ref targetRpm);

        float rpmFraction = Mathf.InverseLerp(_idleRpm, _redlineRpm, engineRpm);
        float targetPitch = Mathf.Lerp(_idlePitch, _redlinePitch, rpmFraction);
        float targetVolume = Mathf.Lerp(_idleVolume, _maxEngineVolume, rpmFraction);
        _carEngineSound.pitch = Mathf.Lerp(_carEngineSound.pitch, targetPitch, Time.deltaTime * 12f);
        _carEngineSound.volume = Mathf.Lerp(_carEngineSound.volume, targetVolume, Time.deltaTime * 8f);
    }

    // Adds variation to the otherwise constant engine note once the endless runner reaches cruising speed.
    private void UpdateCruiseShift(float speedKph, float maxSpeedKph, ref float targetRpm)
    {
        bool canCruiseShift = currentGear == _forwardGearCount && speedKph >= maxSpeedKph * _cruiseShiftSpeedFraction;
        if (!canCruiseShift)
        {
            _cruiseShiftTimer = 0f;
            _cruiseShiftElapsed = 0f;
            _isCruiseShifting = false;
            return;
        }

        if (!_isCruiseShifting)
        {
            _cruiseShiftTimer += Time.deltaTime;
            if (_cruiseShiftTimer < _cruiseShiftInterval)
            {
                return;
            }

            _isCruiseShifting = true;
            _cruiseShiftElapsed = 0f;
            engineRpm = Mathf.Clamp(_cruiseShiftRpm, _idleRpm, _redlineRpm);
        }

        _cruiseShiftElapsed += Time.deltaTime;
        float shiftProgress = Mathf.Clamp01(_cruiseShiftElapsed / _cruiseShiftDuration);
        targetRpm = Mathf.Lerp(_cruiseShiftRpm, _redlineRpm, shiftProgress);
        engineRpm = Mathf.MoveTowards(engineRpm, targetRpm, _rpmResponse * Time.deltaTime);

        if (shiftProgress >= 1f)
        {
            _isCruiseShifting = false;
            _cruiseShiftTimer = 0f;
        }
    }

    //This method will execute when the player comes in contact with a trigger
    private void OnTriggerEnter(Collider collision)
    {

        if (collision.CompareTag("RoadSpawn"))
        {
            spawnManager?.SpawnTriggerEntered();

        }
        else if (collision.CompareTag("StaticObstacleTrigger") || (collision.CompareTag("MovingObstacleTrigger")))
        {
            Score.Instance?.OnScoreIncrement.Invoke(); // Increment the score when the player passes through an obstacle trigger.
        }



    }
    //this method is used in the speed altering pickups to change the cars travel speed
    public void SetMaxSpeed(int Newspeed)
    {
        _maxSpeed = Newspeed;
    }

    private void PauseGame()
    {
        if (pauseScreen != null && Input.GetKey(KeyCode.Escape) && !IsPlayerDead())
        {
            pauseScreen.ActivatePauseScreen();
        }
    }


}
