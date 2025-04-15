using UnityEngine;
using System.Collections.Generic;
public class MovingObstacle : MonoBehaviour
{

    private float[] MovementSpeed = { 500, 300 };       //set the different speeds for the different obstacles
    public static int obstacleIndex;



    // Update is called once per frame
    void Update()
    {
        switch (obstacleIndex)          //check which speed it should be
        {
            //Translate the trigger box depending on the movement speed of that obstacle
            case 0:
                if (this.CompareTag("MovingObstacleTrigger"))
                {
                    transform.Translate(Vector3.forward * -MovementSpeed[0] * Time.deltaTime);
                }
                else
                {
                    transform.Translate(Vector3.forward * MovementSpeed[0] * Time.deltaTime);
                }
                break;
            case 1:
                transform.Translate(Vector3.forward * MovementSpeed[1] * Time.deltaTime);
                break;
        }

    }
}
