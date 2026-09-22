
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class PrometeoCarController : MonoBehaviour
{
    //This script is the main script on controlling the car's physics when driving

    //PRIVATE VARIABLES

    private PlayerController playerController;
    [HideInInspector]
    public float steeringAxis; // Used to know whether the steering wheel has reached the maximum value. It goes from -1 to 1.
    private float steeringSpeed;
    private float throttleAxis; // Used to know whether the throttle has reached the maximum value. It goes from -1 to 1.
    private float initialCarEngineSoundPitch; // Used to store the initial pitch of the car engine sound.
    private int currentLane = 0;
    [HideInInspector]
    private bool _canChangeLanes = true;
    public bool canChangeLanes { get { return _canChangeLanes; } }
    private bool canTurnLeft = true;
    private bool canTurnRight = true;
    private float smoothPitch = 1f;
    private static GameObject playerNose;
    [HideInInspector]
    public bool isSwitchingLane = false;
    [HideInInspector]
    public bool deceleratingCar;

    /*
    The following variables are used to store information about sideways friction of the wheels (such as
    extremumSlip,extremumValue, asymptoteSlip, asymptoteValue and stiffness). We change this values to
    make the car to start drifting.
    */
    WheelFrictionCurve FLwheelFriction;
    float FLWextremumSlip;
    WheelFrictionCurve FRwheelFriction;
    float FRWextremumSlip;
    WheelFrictionCurve RLwheelFriction;
    float RLWextremumSlip;
    WheelFrictionCurve RRwheelFriction;
    float RRWextremumSlip;

    void Start()
    {
        //In this part, we set the 'carRigidbody' value with the Rigidbody attached to this
        //gameObject. Also, we define the center of mass of the car with the Vector3 given
        //in the inspector.
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        playerController.carRigidbody.centerOfMass = playerController.bodyMassCenter;
        playerNose = GameObject.FindWithTag("Player Nose");
        //Initial setup to calculate the drift value of the car. This part could look a bit
        //complicated, but do not be afraid, the only thing we're doing here is to save the default
        //friction values of the car wheels so we can set an appropiate drifting value later.
        FLwheelFriction = new WheelFrictionCurve();
        FLwheelFriction.extremumSlip = playerController.frontLeftCollider.sidewaysFriction.extremumSlip;
        FLWextremumSlip = playerController.frontLeftCollider.sidewaysFriction.extremumSlip;
        FLwheelFriction.extremumValue = playerController.frontLeftCollider.sidewaysFriction.extremumValue;
        FLwheelFriction.asymptoteSlip = playerController.frontLeftCollider.sidewaysFriction.asymptoteSlip;
        FLwheelFriction.asymptoteValue = playerController.frontLeftCollider.sidewaysFriction.asymptoteValue;
        FLwheelFriction.stiffness = playerController.frontLeftCollider.sidewaysFriction.stiffness;
        FRwheelFriction = new WheelFrictionCurve();
        FRwheelFriction.extremumSlip = playerController.frontRightCollider.sidewaysFriction.extremumSlip;
        FRWextremumSlip = playerController.frontRightCollider.sidewaysFriction.extremumSlip;
        FRwheelFriction.extremumValue = playerController.frontRightCollider.sidewaysFriction.extremumValue;
        FRwheelFriction.asymptoteSlip = playerController.frontRightCollider.sidewaysFriction.asymptoteSlip;
        FRwheelFriction.asymptoteValue = playerController.frontRightCollider.sidewaysFriction.asymptoteValue;
        FRwheelFriction.stiffness = playerController.frontRightCollider.sidewaysFriction.stiffness;
        RLwheelFriction = new WheelFrictionCurve();
        RLwheelFriction.extremumSlip = playerController.rearLeftCollider.sidewaysFriction.extremumSlip;
        RLWextremumSlip = playerController.rearLeftCollider.sidewaysFriction.extremumSlip;
        RLwheelFriction.extremumValue = playerController.rearLeftCollider.sidewaysFriction.extremumValue;
        RLwheelFriction.asymptoteSlip = playerController.rearLeftCollider.sidewaysFriction.asymptoteSlip;
        RLwheelFriction.asymptoteValue = playerController.rearLeftCollider.sidewaysFriction.asymptoteValue;
        RLwheelFriction.stiffness = playerController.rearLeftCollider.sidewaysFriction.stiffness;
        RRwheelFriction = new WheelFrictionCurve();
        RRwheelFriction.extremumSlip = playerController.rearRightCollider.sidewaysFriction.extremumSlip;
        RRWextremumSlip = playerController.rearRightCollider.sidewaysFriction.extremumSlip;
        RRwheelFriction.extremumValue = playerController.rearRightCollider.sidewaysFriction.extremumValue;
        RRwheelFriction.asymptoteSlip = playerController.rearRightCollider.sidewaysFriction.asymptoteSlip;
        RRwheelFriction.asymptoteValue = playerController.rearRightCollider.sidewaysFriction.asymptoteValue;
        RRwheelFriction.stiffness = playerController.rearRightCollider.sidewaysFriction.stiffness;

        SetupTraction(playerController.frontLeftCollider);
        SetupTraction(playerController.frontRightCollider);
        SetupTraction(playerController.rearLeftCollider);
        SetupTraction(playerController.rearRightCollider);


        // We save the initial pitch of the car engine sound.
        if (playerController.carEngineSound != null)
        {
            initialCarEngineSoundPitch = playerController.carEngineSound.pitch;
        }


        if (playerController.useUI)
        {
            InvokeRepeating("CarSpeedUI", 0f, 0.1f);
        }
        else if (!playerController.useUI)
        {
            if (playerController.carSpeedText != null)
            {
                playerController.carSpeedText.text = "0";
            }
        }

        if (playerController.useSounds)
        {
            InvokeRepeating("CarSounds", 0f, 0.1f);
        }
        else if (!playerController.useSounds)
        {
            if (playerController.carEngineSound != null)
            {
                playerController.carEngineSound.Stop();
            }
            if (playerController.tireScreechSound != null)
            {
                playerController.tireScreechSound.Stop();
            }
        }

        if (!playerController.useEffects)
        {
            if (playerController.RLWParticleSystem != null)
            {
                playerController.RLWParticleSystem.Stop();
            }
            if (playerController.RRWParticleSystem != null)
            {
                playerController.RRWParticleSystem.Stop();
            }
            if (playerController.RLWTireSkid != null)
            {
                playerController.RLWTireSkid.emitting = false;
            }
            if (playerController.RRWTireSkid != null)
            {
                playerController.RRWTireSkid.emitting = false;
            }
        }




    }



    //
    //Lane Methods
    //

    //This IEnumerator moves the car sideways smoothly to the desired lane position
    IEnumerator SmoothLaneChange(Vector3 targetPos, float duration)
    {
        Vector3 startPos = playerController.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            float currentZ = playerController.transform.position.z;
            float targetX = Mathf.Lerp(startPos.x, targetPos.x, t);
            Vector3 newPos = new Vector3(targetX, startPos.y, currentZ);
            playerController.carRigidbody.MovePosition(newPos);

            //Limit the slip angle of the car
            Vector3 velocity = playerController.carRigidbody.linearVelocity;
            //This clamp effects how much the car drifts when changing lanes
            velocity.x = Mathf.Clamp(velocity.x, -50f, 50f);
            playerController.carRigidbody.linearVelocity = velocity;

            ApplySteering();
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isSwitchingLane = false;
    }

    private Tween steeringAxisTween;

    //This method calculates the how and where the car should turn depending on the desired lane
    public void LaneChange(int desiredLane)
    {
        //only allow lane changes when it is possible
        if (!canChangeLanes || isSwitchingLane)
            return;

        _canChangeLanes = false;

        float[] laneXPositions = new float[]
        {
            playerController.laneDistance,
            0f,
            -playerController.laneDistance
        };
        float targetX = laneXPositions[desiredLane];
        float targetSteeringAxis = 0f;

        switch (desiredLane)
        {
            case 0: // Left Lane
                if (!canTurnLeft)
                    return;
                else
                {
                    targetSteeringAxis = -1f;
                    currentLane = desiredLane;
                    canTurnLeft = false;
                    canTurnRight = true;
                    break;
                }
            case 1: // Center Lane

                if (currentLane == 0)
                {
                    targetSteeringAxis = 1f;
                }
                else if (currentLane == 2)
                {
                    targetSteeringAxis = -1f;
                }
                currentLane = desiredLane;
                canTurnRight = true;
                canTurnLeft = true;
                break;
            case 2: //Right Lane
                if (!canTurnRight)
                    return;
                else
                {
                    targetSteeringAxis = 1f;
                    currentLane = desiredLane;
                    canTurnRight = false;
                    canTurnLeft = true;
                    break;
                }
        }


        isSwitchingLane = true;

        DOTween.Kill(steeringAxisTween);

        AudioManager.Instance?.PlaySFX("ScreechSound"); 
        //This DOTween turns the car's wheels to the direction of the desired turn
        steeringAxisTween = DOTween.To(() => steeringAxis, x => steeringAxis = x, targetSteeringAxis, 0.1f)
        .SetEase(Ease.OutSine)
        .OnComplete(() =>
        {
            steeringAxisTween = DOTween.To(() => steeringAxis, x => steeringAxis = x, 0f, 0.1f)
            .SetEase(Ease.OutSine);
        });

        Vector3 targetPos = new Vector3(targetX, playerController.transform.position.y, playerController.transform.position.z);
        StartCoroutine(SmoothLaneChange(targetPos, 0.25f));
    }

    //This method effect the car's wheel's colliders so that the correct contact is being made
    private void ApplySteering()
    {
        float steeringAngle = steeringAxis * playerController.maxSteeringAngle;
        playerController.frontLeftCollider.steerAngle = steeringAngle;
        playerController.frontRightCollider.steerAngle = steeringAngle;
    }

    //This method is a correction method to make sure the car stays inside their current lane
    public void KeepCarInLane()
    {
        float targetX = 0f;
        switch (currentLane)
        {
            case 0:
                targetX = 40f;
                break;
            case 1:
                targetX = 0;
                break;
            case 2:
                targetX = -40f;
                break;
        }

        //get the distance from the car's nose to the center of the lane
        float distanceFromCenter = targetX - playerNose.transform.position.x;

        Vector3 localVelocity = playerController.transform.InverseTransformDirection(playerController.carRigidbody.linearVelocity);
        float lateralVelocity = localVelocity.x;
        float forwardSpeed = Mathf.Abs(localVelocity.z);


        float basepositionDeadzone = 0.2f;
        float baselateralVelocityDeadzone = 0.2f;

        float positionDeadzone = basepositionDeadzone + (forwardSpeed * 0.01f);
        float lateralVelocityDeadzone = baselateralVelocityDeadzone + (forwardSpeed * 0.01f);

        if (Mathf.Abs(distanceFromCenter) < positionDeadzone)
            distanceFromCenter = 0f;

        if (Mathf.Abs(lateralVelocity) < lateralVelocityDeadzone)
            lateralVelocity = 0f;

        //Make the correct steering axis to get the car more aligned with the center of the lane
        float correctionSteeringAxis = (distanceFromCenter * playerController.centeringForce) - (lateralVelocity * playerController.dampingForce);
        correctionSteeringAxis = Mathf.Clamp(correctionSteeringAxis, -1f, 1f);

        //if the car is still correcting itself then don't allow the player to change lanes
        if (Mathf.Abs(lateralVelocity) < 0.8f && Mathf.Abs(playerController.frontLeftCollider.steerAngle) < 17 && Mathf.Abs(playerController.frontRightCollider.steerAngle) < 17)
        {
            _canChangeLanes = true;
            steeringSpeed = 0.1f;
        }
        else
        {
            steeringSpeed = 2f;
        }

        ApplySteeringCorrection(correctionSteeringAxis);

    }

    //this method effects the car's wheel's colliders for the correcting 
    private void ApplySteeringCorrection(float correctionSteeringAxis)
    {
        float steeringAngle = correctionSteeringAxis * playerController.maxSteeringAngle;
        playerController.frontLeftCollider.steerAngle = -steeringAngle;
        playerController.frontRightCollider.steerAngle = -steeringAngle;
    }

    // This method matches both the position and rotation of the WheelColliders with the WheelMeshes.
    public void AnimateWheelMeshes()
    {
        try
        {
            Quaternion FLWRotation;
            Vector3 FLWPosition;
            playerController.frontLeftCollider.GetWorldPose(out FLWPosition, out FLWRotation);
            playerController.frontLeftMesh.transform.position = FLWPosition;
            playerController.frontLeftMesh.transform.rotation = FLWRotation;

            Quaternion FRWRotation;
            Vector3 FRWPosition;
            playerController.frontRightCollider.GetWorldPose(out FRWPosition, out FRWRotation);
            playerController.frontRightMesh.transform.position = FRWPosition;
            playerController.frontRightMesh.transform.rotation = FRWRotation;

            Quaternion RLWRotation;
            Vector3 RLWPosition;
            playerController.rearLeftCollider.GetWorldPose(out RLWPosition, out RLWRotation);
            playerController.rearLeftMesh.transform.position = RLWPosition;
            playerController.rearLeftMesh.transform.rotation = RLWRotation;

            Quaternion RRWRotation;
            Vector3 RRWPosition;
            playerController.rearRightCollider.GetWorldPose(out RRWPosition, out RRWRotation);
            playerController.rearRightMesh.transform.position = RRWPosition;
            playerController.rearRightMesh.transform.rotation = RRWRotation;
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    //
    //ENGINE AND BRAKING METHODS
    //

    // This method apply positive torque to the wheels in order to go forward.
    public void GoForward()
    {
        //If the forces aplied to the rigidbody in the 'x' asis are greater than
        //50.5f, it means that the car is losing traction, then the car will start emitting particle systems.
        if (Mathf.Abs(playerController.localVelocityX) > 50.5f)
        {
            playerController.isDrifting = true;
        }
        else
        {
            playerController.isDrifting = false;
        }
        DriftCarPS();
        throttleAxis += Time.deltaTime * 3f;
        throttleAxis = Mathf.Clamp01(throttleAxis);

        if (throttleAxis > 1f)
        {
            throttleAxis = 1f;
        }

        if (Mathf.RoundToInt(playerController.carSpeed) < playerController.maxSpeed)
        {
            float torque = playerController.accelerationMultiplier * 60f * throttleAxis;
            ApplyTorque(torque);

        }
        else
        {
            ApplyTorque(0f);
        }

    }

    public void ApplyTorque(float torque)
    {
        playerController.frontLeftCollider.brakeTorque = 0;
        playerController.frontRightCollider.brakeTorque = 0;
        playerController.rearLeftCollider.brakeTorque = 0;
        playerController.rearRightCollider.brakeTorque = 0;

        playerController.frontLeftCollider.motorTorque = torque;
        playerController.frontRightCollider.motorTorque = torque;
        playerController.rearLeftCollider.motorTorque = torque;
        playerController.rearRightCollider.motorTorque = torque;

    }
    
    //This method sets up the amount of traction that the wheels give forwards and during sidways motion
    public void SetupTraction(WheelCollider collider)
    {
        WheelFrictionCurve forwardFriction = collider.forwardFriction;
        forwardFriction.extremumSlip = 1f;
        forwardFriction.extremumValue = 2f;
        forwardFriction.asymptoteSlip = 1.2f;
        forwardFriction.asymptoteValue = 1.7f;
        forwardFriction.stiffness = 20f;
        collider.forwardFriction = forwardFriction;

        WheelFrictionCurve sidewaysFriction = collider.sidewaysFriction;
        sidewaysFriction.extremumSlip = 0.5f;
        sidewaysFriction.extremumValue = 1.7f;
        sidewaysFriction.asymptoteSlip = 1.0f;
        sidewaysFriction.asymptoteValue = 1.0f;
        sidewaysFriction.stiffness = 40f;
        collider.sidewaysFriction = sidewaysFriction;
    }

    //This method controls the drift particles when the car is drifting
    public void DriftCarPS()
    {
        if (playerController.useEffects)
        {
            try
            {
                if (playerController.isDrifting)
                {
                    playerController.RLWParticleSystem.Play();
                    playerController.RRWParticleSystem.Play();
                }
                else if (!playerController.isDrifting)
                {
                    playerController.RLWParticleSystem.Stop();
                    playerController.RRWParticleSystem.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }

            try
            {
                if ((playerController.isTractionLocked || Mathf.Abs(playerController.localVelocityX) > 50f) && Mathf.Abs(playerController.carSpeed) > 12f)
                {
                    playerController.RLWTireSkid.emitting = true;
                    playerController.RRWTireSkid.emitting = true;
                }
                else
                {
                    playerController.RLWTireSkid.emitting = false;
                    playerController.RRWTireSkid.emitting = false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else if (!playerController.useEffects)
        {
            if (playerController.RLWParticleSystem != null)
            {
                playerController.RLWParticleSystem.Stop();
            }
            if (playerController.RRWParticleSystem != null)
            {
                playerController.RRWParticleSystem.Stop();
            }
            if (playerController.RLWTireSkid != null)
            {
                playerController.RLWTireSkid.emitting = false;
            }
            if (playerController.RRWTireSkid != null)
            {
                playerController.RRWTireSkid.emitting = false;
            }
        }

    }

    //This method controls the sounds of the car, such as the engine.
    public void CarSounds()
    {

        if (playerController.useSounds)
        {
            try
            {
                if (playerController.carEngineSound != null)
                {
                    float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(playerController.carRigidbody.linearVelocity.magnitude) / 1000f);
                    smoothPitch = Mathf.Lerp(smoothPitch, engineSoundPitch, Time.deltaTime * 20f);
                    playerController.carEngineSound.pitch = smoothPitch;
                }
                if ((playerController.isDrifting) || (playerController.isTractionLocked && Mathf.Abs(playerController.carSpeed) > 12f))
                {
                    if (!playerController.tireScreechSound.isPlaying)
                    {
                        playerController.tireScreechSound.Play();
                    }
                }
                else if ((!playerController.isDrifting) && (!playerController.isTractionLocked || Mathf.Abs(playerController.carSpeed) < 12f))
                {
                    playerController.tireScreechSound.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else if (!playerController.useSounds)
        {
            if (playerController.carEngineSound != null && playerController.carEngineSound.isPlaying)
            {
                playerController.carEngineSound.Stop();
            }
            if (playerController.tireScreechSound != null && playerController.tireScreechSound.isPlaying)
            {
                playerController.tireScreechSound.Stop();
            }
        }

    }
    //This method controls the exhaust flame particle effect
    public void ExhaustFlamePS()
    {
        if (playerController.useEffects && playerController.RightExhaustFlame != null && playerController.LeftExhaustFlame)
        {
            playerController.LeftExhaustFlame.Play();
            playerController.RightExhaustFlame.Play();
        }
    }

    #region Unused Code
    /*
     * //The following method takes the front car wheels to their default position (rotation = 0). The speed of this movement will depend
    // on the steeringSpeed variable.

    public void ResetSteeringAngle()
    {
        if (steeringAxis < 0f)
        {
            steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        }
        else if (steeringAxis > 0f)
        {
            steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        }
        if (Mathf.Abs(playerController.frontLeftCollider.steerAngle) < 1f)
        {
            steeringAxis = 0f;
        }
        var steeringAngle = steeringAxis * playerController.maxSteeringAngle;
        playerController.frontLeftCollider.steerAngle = Mathf.Lerp(playerController.frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        playerController.frontRightCollider.steerAngle = Mathf.Lerp(playerController.frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }
     *  // This method converts the car speed data from float to string, and then set the text of the UI carSpeedText with this value.
    public void CarSpeedUI()
    {

        if (playerController.useUI)
        {
            try
            {
                float absoluteCarSpeed = Mathf.Abs(playerController.carSpeed);
                playerController.carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }

    }


    

    public void GoReverse()
    {
        //If the forces aplied to the rigidbody in the 'x' asis are greater than
        //3f, it means that the car is losing traction, then the car will start emitting particle systems.
        if (Mathf.Abs(playerController.localVelocityX) > 50.5f)
        {
            playerController.isDrifting = true;
            DriftCarPS();
        }
        else
        {
            playerController.isDrifting = false;
            DriftCarPS();
        }
        // The following part sets the throttle power to -1 smoothly.
        throttleAxis = throttleAxis - (Time.deltaTime * 3f);
        if (throttleAxis < -1f)
        {
            throttleAxis = -1f;
        }
        //If the car is still going forward, then apply brakes in order to avoid strange
        //behaviours. If the local velocity in the 'z' axis is greater than 1f, then it
        //is safe to apply negative torque to go reverse.
        if (playerController.localVelocityZ > 1f)
        {
            Brakes();
        }
        else
        {
            if (Mathf.Abs(Mathf.RoundToInt(playerController.carSpeed)) < playerController.maxReverseSpeed)
            {
                //Apply negative torque in all wheels to go in reverse if maxReverseSpeed has not been reached.
                playerController.frontLeftCollider.brakeTorque = 0;
                playerController.frontLeftCollider.motorTorque = (playerController.accelerationMultiplier * 50f) * throttleAxis;
                playerController.frontRightCollider.brakeTorque = 0;
                playerController.frontRightCollider.motorTorque = (playerController.accelerationMultiplier * 50f) * throttleAxis;
                playerController.rearLeftCollider.brakeTorque = 0;
                playerController.rearLeftCollider.motorTorque = (playerController.accelerationMultiplier * 50f) * throttleAxis;
                playerController.rearRightCollider.brakeTorque = 0;
                playerController.rearRightCollider.motorTorque = (playerController.accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                //If the maxReverseSpeed has been reached, then stop applying torque to the wheels.
                // IMPORTANT: The maxReverseSpeed variable should be considered as an approximation; the speed of the car
                // could be a bit higher than expected.
                playerController.frontLeftCollider.motorTorque = 0;
                playerController.frontRightCollider.motorTorque = 0;
                playerController.rearLeftCollider.motorTorque = 0;
                playerController.rearRightCollider.motorTorque = 0;
            }
        }
    }
    

    //The following function set the motor torque to 0 (in case the user is not pressing either W or S).
    public void ThrottleOff()
    {
        playerController.frontLeftCollider.motorTorque = 0;
        playerController.frontRightCollider.motorTorque = 0;
        playerController.rearLeftCollider.motorTorque = 0;
        playerController.rearRightCollider.motorTorque = 0;
    }

    // The following method decelerates the speed of the car according to the decelerationMultiplier variable, where
    // 1 is the slowest and 10 is the fastest deceleration. This method is called by the function InvokeRepeating,
    // usually every 0.1f when the user is not pressing W (throttle), S (reverse) or Space bar (handbrake).
    
    public void DecelerateCar()
    {
        if (Mathf.Abs(playerController.localVelocityX) > 50.5f)
        {
            playerController.isDrifting = true;
            DriftCarPS();
        }
        else
        {
            playerController.isDrifting = false;
            DriftCarPS();
        }
        // The following part resets the throttle power to 0 smoothly.
        if (throttleAxis != 0f)
        {
            if (throttleAxis > 0f)
            {
                throttleAxis = throttleAxis - (Time.deltaTime * 10f);
            }
            else if (throttleAxis < 0f)
            {
                throttleAxis = throttleAxis + (Time.deltaTime * 10f);
            }
            if (Mathf.Abs(throttleAxis) < 0.15f)
            {
                throttleAxis = 0f;
            }
        }
        playerController.carRigidbody.linearVelocity = playerController.carRigidbody.linearVelocity * (1f / (1f + (0.025f * playerController.decelerationMultiplier)));
        // Since we want to decelerate the car, we are going to remove the torque from the wheels of the car.
        playerController.frontLeftCollider.motorTorque = 0;
        playerController.frontRightCollider.motorTorque = 0;
        playerController.rearLeftCollider.motorTorque = 0;
        playerController.rearRightCollider.motorTorque = 0;
        // If the magnitude of the car's velocity is less than 0.25f (very slow velocity), then stop the car completely and
        // also cancel the invoke of this method.
        if (playerController.carRigidbody.linearVelocity.magnitude < 0.25f)
        {
            playerController.carRigidbody.linearVelocity = Vector3.zero;
            CancelInvoke("DecelerateCar");
        }
    }

    // This function applies brake torque to the wheels according to the brake force given by the user.
    
    public void Brakes()
    {
        playerController.frontLeftCollider.brakeTorque = playerController.brakeForce;
        playerController.frontRightCollider.brakeTorque = playerController.brakeForce;
        playerController.rearLeftCollider.brakeTorque = playerController.brakeForce;
        playerController.rearRightCollider.brakeTorque = playerController.brakeForce;
    }
    */
    // This function is used to make the car lose traction. By using this, the car will start drifting. The amount of traction lost
    // will depend on the handbrakeDriftMultiplier variable. If this value is small, then the car will not drift too much, but if
    // it is high, then you could make the car to feel like going on ice.
    /*
    public void Handbrake()
    {
        CancelInvoke("RecoverTraction");
        // We are going to start losing traction smoothly, there is were our 'driftingAxis' variable takes
        // place. This variable will start from 0 and will reach a top value of 1, which means that the maximum
        // drifting value has been reached. It will increase smoothly by using the variable Time.deltaTime.
        driftingAxis = driftingAxis + (Time.deltaTime);
        float secureStartingPoint = driftingAxis * FLWextremumSlip * playerController.handbrakeDriftMultiplier;

        if (secureStartingPoint < FLWextremumSlip)
        {
            driftingAxis = FLWextremumSlip / (FLWextremumSlip * playerController.handbrakeDriftMultiplier);
        }
        if (driftingAxis > 1f)
        {
            driftingAxis = 1f;
        }
        //If the forces aplied to the rigidbody in the 'x' asis are greater than
        //3f, it means that the car lost its traction, then the car will start emitting particle systems.
        if (Mathf.Abs(playerController.localVelocityX) > 50.5f)
        {
            playerController.isDrifting = true;
        }
        else
        {
            playerController.isDrifting = false;
        }
        //If the 'driftingAxis' value is not 1f, it means that the wheels have not reach their maximum drifting
        //value, so, we are going to continue increasing the sideways friction of the wheels until driftingAxis
        // = 1f.
        if (driftingAxis < 1f)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip * playerController.handbrakeDriftMultiplier * driftingAxis;
            playerController.frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip * playerController.handbrakeDriftMultiplier * driftingAxis;
            playerController.frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip * playerController.handbrakeDriftMultiplier * driftingAxis;
            playerController.rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip * playerController.handbrakeDriftMultiplier * driftingAxis;
            playerController.rearRightCollider.sidewaysFriction = RRwheelFriction;
        }

        // Whenever the player uses the handbrake, it means that the wheels are locked, so we set 'isTractionLocked = true'
        // and, as a consequense, the car starts to emit trails to simulate the wheel skids.
        playerController.isTractionLocked = true;
        DriftCarPS();

    }
    
    // This function is used to emit both the particle systems of the tires' smoke and the trail renderers of the tire skids
    // depending on the value of the bool variables 'isDrifting' and 'isTractionLocked'.
    */



    // This function is used to recover the traction of the car when the user has stopped using the car's handbrake.
    /*
    public void RecoverTraction()
    {
        playerController.isTractionLocked = false;
        driftingAxis = driftingAxis - (Time.deltaTime / 1.5f);
        if (driftingAxis < 0f)
        {
            driftingAxis = 0f;
        }

        //If the 'driftingAxis' value is not 0f, it means that the wheels have not recovered their traction.
        //We are going to continue decreasing the sideways friction of the wheels until we reach the initial
        // car's grip.
        if (FLwheelFriction.extremumSlip > FLWextremumSlip)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip * driftingAxis;
            playerController.frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip * driftingAxis;
            playerController.frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip * driftingAxis;
            playerController.rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip * driftingAxis;
            playerController.rearRightCollider.sidewaysFriction = RRwheelFriction;

            Invoke("RecoverTraction", Time.deltaTime);

        }
        else if (FLwheelFriction.extremumSlip < FLWextremumSlip)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip;
            playerController.frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip;
            playerController.frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip;
            playerController.rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip;
            playerController.rearRightCollider.sidewaysFriction = RRwheelFriction;

            driftingAxis = 0f;
        }
    }

    */
    #endregion

}
