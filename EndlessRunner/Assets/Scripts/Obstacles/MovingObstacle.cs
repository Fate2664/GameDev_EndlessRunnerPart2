using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    private float[] movementSpeed = { 900, 700, 100 };

    [HideInInspector]
    private int _obstacleIndex;
    public int obstacleIndex { get => _obstacleIndex; set => _obstacleIndex = value; }

    private float currentSpeed = 0f;

    private void Start()
    {
        if (_obstacleIndex == 2 || _obstacleIndex == 3)
        {
            currentSpeed = movementSpeed[2];
        }
    }

    private void Update()
    {
        switch (_obstacleIndex)
        {
            case 0:
                float direction0 = CompareTag("MovingObstacleTrigger") ? -1f : 1f;
                MoveObstacle(movementSpeed[0], direction0);
                break;

            case 1:
                MoveObstacle(movementSpeed[1], 1f);
                break;

            case 2:
                HandleTraffic(-transform.forward, currentSpeed, -1f);
                break;

            case 3:
                HandleTraffic(transform.forward, currentSpeed, 1f);
                break;
        }
    }

    private void MoveObstacle(float speed, float directionMultiplier)
    {
        transform.Translate(Vector3.forward * speed * directionMultiplier * Time.deltaTime);
    }

    private void HandleTraffic(Vector3 direction, float targetSpeed, float directionMultiplier)
    {
        float distanceToCheck = 80f;
        bool shouldStop = false;

        Vector3 rayOrigin = transform.position - direction * 1f;
        Debug.DrawRay(rayOrigin, direction * distanceToCheck, Color.red);
        if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, distanceToCheck))
        {
            if (hit.collider.CompareTag("NoSpawnTrigger") || (hit.collider.CompareTag("Obstacle")) &&
                Mathf.Abs(hit.point.x - transform.position.x) < 1f)
            {
                shouldStop = true;
            }
        }
      
        currentSpeed = Mathf.Lerp(currentSpeed, shouldStop ? 0f : targetSpeed, Time.deltaTime * (shouldStop ? 5f : 2f));
        transform.Translate(Vector3.forward * currentSpeed * directionMultiplier * Time.deltaTime);
    }

    

}
