using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    //This script is for moving any obstacle 

    //Movement idexes for different moving obstacles:
    // 0 = moving past player
    // 1 = moving towards player
    // 2 = traffic
    private float[] movementSpeed = { 900, 700, 100 };


    [HideInInspector]
    private int _obstacleIndex;
    public int obstacleIndex { get => _obstacleIndex; set => _obstacleIndex = value; }

    private float direction = 0f;
    private float currentSpeed = 0f;

    private void Start()
    {
        //traffic uses both index 2 and 3 so they need to be the same speed
        if (_obstacleIndex == 2 || _obstacleIndex == 3)
        {
            currentSpeed = movementSpeed[2];
        }

    }

    private void Update()
    {
        //Get the obstacle index to determine the speed of that obstacle type
        switch (_obstacleIndex)
        {
            case 0:
                direction = CompareTag("MovingObstacleTrigger") ? -1f : 1f;
                MoveObstacle(movementSpeed[0], direction);
                break;

            case 1:
                direction = CompareTag("MovingObstacleTrigger") ? -1f : 1f;
                MoveObstacle(-movementSpeed[1], direction);
                break;

            case 2:
                HandleTraffic(-transform.forward, currentSpeed, -1f);
                break;

            case 3:
                HandleTraffic(transform.forward, currentSpeed, 1f);
                break;
        }
    }

    //Move obstacle method
    private void MoveObstacle(float speed, float directionMultiplier)
    {
        transform.Translate(Vector3.forward * speed * directionMultiplier * Time.deltaTime);
    }

    //Move traffic method
    private void HandleTraffic(Vector3 direction, float targetSpeed, float directionMultiplier)
    {
        float distanceToCheck = 80f;
        bool shouldStop = false;

        //Send out a raycast to check if there is an obstacle infront of the traffic car
        Vector3 rayOrigin = transform.position - direction * 1f;

        if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, distanceToCheck))
        {
            if (hit.collider.CompareTag("NoSpawnTrigger") || (hit.collider.CompareTag("Obstacle")) &&
                Mathf.Abs(hit.point.x - transform.position.x) < 1f)
            {
                shouldStop = true;
            }
        }
        //stop the traffic car if there is an obstacle
        currentSpeed = Mathf.Lerp(currentSpeed, shouldStop ? 0f : targetSpeed, Time.deltaTime * (shouldStop ? 5f : 2f));
        transform.Translate(Vector3.forward * currentSpeed * directionMultiplier * Time.deltaTime);
    }


}
