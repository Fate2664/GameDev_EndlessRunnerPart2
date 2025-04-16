/*
MESSAGE FROM CREATOR: This script was coded by Mena. You can use it in your games either these are commercial or
personal projects. You can even add or remove functions as you wish. However, you cannot sell copies of this
script by itself, since it is originally distributed as a free product.
I wish you the best for your project. Good luck!

P.S: If you need more cars, you can check my other vehicle assets on the Unity Asset Store, perhaps you could find
something useful for your game. Best regards, Mena.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PrometeoCarController : MonoBehaviour
{

    //CONTROLS
    /*
      [Space(20)]
      //[Header("CONTROLS")]
      [Space(10)]
      //The following variables lets you to set up touch controls for mobile devices.
      public bool useTouchControls = false;
      public GameObject throttleButton;
      PrometeoTouchInput throttlePTI;
      public GameObject reverseButton;
      PrometeoTouchInput reversePTI;
      public GameObject turnRightButton;
      PrometeoTouchInput turnRightPTI;
      public GameObject turnLeftButton;
      PrometeoTouchInput turnLeftPTI;
      public GameObject handbrakeButton;
      PrometeoTouchInput handbrakePTI;
    */
    
    //PRIVATE VARIABLES

    /*
    IMPORTANT: The following variables should not be modified manually since their values are automatically given via script.
    */
    private PlayerController playerController;
    [HideInInspector]
    public float steeringAxis; // Used to know whether the steering wheel has reached the maximum value. It goes from -1 to 1.
    private float throttleAxis; // Used to know whether the throttle has reached the maximum value. It goes from -1 to 1.
    private float driftingAxis;
    private float initialCarEngineSoundPitch; // Used to store the initial pitch of the car engine sound.
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

    // Start is called before the first frame update
    void Start()
    {
        //In this part, we set the 'carRigidbody' value with the Rigidbody attached to this
        //gameObject. Also, we define the center of mass of the car with the Vector3 given
        //in the inspector.
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        playerController.carRigidbody.centerOfMass = playerController.bodyMassCenter;
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

        // We invoke 2 methods inside this script. CarSpeedUI() changes the text of the UI object that stores
        // the speed of the car and CarSounds() controls the engine and drifting sounds. Both methods are invoked
        // in 0 seconds, and repeatedly called every 0.1 seconds.
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


    // This method converts the car speed data from float to string, and then set the text of the UI carSpeedText with this value.
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

    // This method controls the car sounds. For example, the car engine will sound slow when the car speed is low because the
    // pitch of the sound will be at its lowest point. On the other hand, it will sound fast when the car speed is high because
    // the pitch of the sound will be the sum of the initial pitch + the car speed divided by 100f.
    // Apart from that, the tireScreechSound will play whenever the car starts drifting or losing traction.
    public void CarSounds()
    {

        if (playerController.useSounds)
        {
            try
            {
                if (playerController.carEngineSound != null)
                {
                    float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(playerController.carRigidbody.linearVelocity.magnitude) / 25f);
                    playerController.carEngineSound.pitch = engineSoundPitch;
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

    //
    //STEERING METHODS
    //

    //The following method turns the front car wheels to the left. The speed of this movement will depend on the steeringSpeed variable.
    public void TurnLeft()
    {
        steeringAxis = steeringAxis - (Time.deltaTime * 10f * playerController.steeringSpeed);
        if (steeringAxis < -1f)
        {
            steeringAxis = -1f;
        }
        var steeringAngle = steeringAxis * playerController.maxSteeringAngle;
        playerController.frontLeftCollider.steerAngle = Mathf.Lerp(playerController.frontLeftCollider.steerAngle, steeringAngle, playerController.steeringSpeed);
        playerController.frontRightCollider.steerAngle = Mathf.Lerp(playerController.frontRightCollider.steerAngle, steeringAngle, playerController.steeringSpeed);
    }

    //The following method turns the front car wheels to the right. The speed of this movement will depend on the steeringSpeed variable.
    public void TurnRight()
    {
        steeringAxis = steeringAxis + (Time.deltaTime * 10f * playerController.steeringSpeed);
        if (steeringAxis > 1f)
        {
            steeringAxis = 1f;
        }
        var steeringAngle = steeringAxis * playerController.maxSteeringAngle;
        playerController.frontLeftCollider.steerAngle = Mathf.Lerp(playerController.frontLeftCollider.steerAngle, steeringAngle, playerController.steeringSpeed);
        playerController.frontRightCollider.steerAngle = Mathf.Lerp(playerController.frontRightCollider.steerAngle, steeringAngle, playerController.steeringSpeed);
    }

    //The following method takes the front car wheels to their default position (rotation = 0). The speed of this movement will depend
    // on the steeringSpeed variable.
    public void ResetSteeringAngle()
    {
        if (steeringAxis < 0f)
        {
            steeringAxis = steeringAxis + (Time.deltaTime * 10f * playerController.steeringSpeed);
        }
        else if (steeringAxis > 0f)
        {
            steeringAxis = steeringAxis - (Time.deltaTime * 10f * playerController.steeringSpeed);
        }
        if (Mathf.Abs(playerController.frontLeftCollider.steerAngle) < 1f)
        {
            steeringAxis = 0f;
        }
        var steeringAngle = steeringAxis * playerController.maxSteeringAngle;
        playerController.frontLeftCollider.steerAngle = Mathf.Lerp(playerController.frontLeftCollider.steerAngle, steeringAngle, playerController.steeringSpeed);
        playerController.frontRightCollider.steerAngle = Mathf.Lerp(playerController.frontRightCollider.steerAngle, steeringAngle, playerController.steeringSpeed);
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
        //3f, it means that the car is losing traction, then the car will start emitting particle systems.
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
        //If the car is going backwards, then apply brakes in order to avoid strange
        //behaviours. If the local velocity in the 'z' axis is less than -1f, then it
        //is safe to apply positive torque to go forward.
        if (playerController.localVelocityZ < -1f)
        {
            Brakes();
            return;
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

    public void SetupTraction(WheelCollider collider)
    {
        WheelFrictionCurve forwardFriction = collider.forwardFriction;
        forwardFriction.extremumSlip = 1f;
        forwardFriction.extremumValue = 2f;
        forwardFriction.asymptoteSlip = 1.2f;
        forwardFriction.asymptoteValue = 1.7f;
        forwardFriction.stiffness = 25f;
        collider.forwardFriction = forwardFriction;

        WheelFrictionCurve sidewaysFriction = collider.sidewaysFriction;
        sidewaysFriction.extremumSlip = 0.5f;
        sidewaysFriction.extremumValue = 1.7f;
        sidewaysFriction.asymptoteSlip = 1.0f;
        sidewaysFriction.asymptoteValue = 1.0f;
        sidewaysFriction.stiffness = 20f;
        collider.sidewaysFriction = sidewaysFriction;
    }

    // This method apply negative torque to the wheels in order to go backwards.
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

    // This function is used to make the car lose traction. By using this, the car will start drifting. The amount of traction lost
    // will depend on the handbrakeDriftMultiplier variable. If this value is small, then the car will not drift too much, but if
    // it is high, then you could make the car to feel like going on ice.
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
    public void DriftCarPS()
    {
        Debug.Log("Drifting");
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

    // This function is used to recover the traction of the car when the user has stopped using the car's handbrake.
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
            FLwheelFriction.extremumSlip = FLWextremumSlip * playerController.handbrakeDriftMultiplier * driftingAxis;
            playerController.frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip * playerController.handbrakeDriftMultiplier * driftingAxis;
            playerController.frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip * playerController.handbrakeDriftMultiplier * driftingAxis;
            playerController.rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip * playerController.handbrakeDriftMultiplier * driftingAxis;
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

}
