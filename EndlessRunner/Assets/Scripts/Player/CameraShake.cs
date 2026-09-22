using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Vehicle")]
    [Tooltip("Leave empty when this camera is parented to the player car.")]
    [SerializeField] private Rigidbody vehicleRigidbody;
    [Tooltip("Optional fallback when the camera is not a child of the car.")]
    [SerializeField] private PlayerController playerController;

    [Header("Speed Response")]
    [Tooltip("The car speed at which the shake begins.")]
    [SerializeField] private float minShakeSpeedKph = 35f;
    [Tooltip("The car speed at which the shake reaches full strength.")]
    [SerializeField] private float maxShakeSpeedKph = 180f;
    [Tooltip("How quickly the shake follows changes in car speed.")]
    [SerializeField] private float intensityResponse = 4f;

    [Header("Close-Up Shake")]
    [Tooltip("Maximum local position offset. Keep this small for a camera mounted to the car.")]
    [SerializeField] private Vector3 positionAmplitude = new Vector3(0.008f, 0.012f, 0.004f);
    [Tooltip("Maximum rotation offset, in degrees.")]
    [SerializeField] private Vector3 rotationAmplitude = new Vector3(0.35f, 0.25f, 0.5f);
    [Tooltip("How quickly the shake noise changes.")]
    [SerializeField] private float shakeFrequency = 15f;

    private Vector3 baseLocalPosition;
    private Quaternion baseLocalRotation;
    private Vector3 positionOffset;
    private Quaternion rotationOffset = Quaternion.identity;
    private float currentIntensity;
    private float noiseSeed;

    private void Awake()
    {
        FindVehicle();
        CaptureBasePose();
        noiseSeed = Random.value * 1000f;
    }

    private void OnEnable()
    {
        CaptureBasePose();
    }

    private void LateUpdate()
    {
        FindVehicle();

        float speedKph = vehicleRigidbody != null
            ? vehicleRigidbody.linearVelocity.magnitude * 3.6f
            : 0f;
        float targetIntensity = Mathf.InverseLerp(minShakeSpeedKph, maxShakeSpeedKph, speedKph);
        currentIntensity = Mathf.MoveTowards(
            currentIntensity,
            targetIntensity,
            intensityResponse * Time.deltaTime);

        float noiseTime = Time.time * shakeFrequency;
        Vector3 noise = new Vector3(
            SignedNoise(noiseTime, 0f),
            SignedNoise(noiseTime, 17.3f),
            SignedNoise(noiseTime, 41.7f));

        positionOffset = Vector3.Scale(noise, positionAmplitude) * currentIntensity;
        rotationOffset = Quaternion.Euler(Vector3.Scale(noise, rotationAmplitude) * currentIntensity);

        transform.localPosition = baseLocalPosition + positionOffset;
        transform.localRotation = baseLocalRotation * rotationOffset;
    }

    private void OnDisable()
    {
        // Do not leave the close-up camera tilted or offset after this component is turned off.
        transform.localPosition = baseLocalPosition;
        transform.localRotation = baseLocalRotation;
        positionOffset = Vector3.zero;
        rotationOffset = Quaternion.identity;
        currentIntensity = 0f;
    }

    private void FindVehicle()
    {
        if (vehicleRigidbody == null)
        {
            vehicleRigidbody = GetComponentInParent<Rigidbody>();
        }

        if (vehicleRigidbody == null)
        {
            if (playerController == null)
            {
                playerController = GetComponentInParent<PlayerController>();
            }

            if (playerController == null)
            {
                playerController = FindFirstObjectByType<PlayerController>();
            }

            if (playerController != null)
            {
                vehicleRigidbody = playerController.carRigidbody;
            }
        }
    }

    private void CaptureBasePose()
    {
        baseLocalPosition = transform.localPosition;
        baseLocalRotation = transform.localRotation;
    }

    private float SignedNoise(float time, float axisOffset)
    {
        return Mathf.PerlinNoise(noiseSeed + axisOffset, time) * 2f - 1f;
    }
}
