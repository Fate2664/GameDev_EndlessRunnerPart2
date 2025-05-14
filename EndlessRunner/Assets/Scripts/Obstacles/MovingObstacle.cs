using UnityEngine;
using System.Collections.Generic;
public class MovingObstacle : MonoBehaviour
{

    private float[] movementSpeed = { 900, 700, 100 };       //set the different speeds for the different obstacles
    [HideInInspector]
    private int _obstacleIndex;
    public int obstacleIndex { get { return _obstacleIndex; } set {  _obstacleIndex = value; } }



    // Update is called once per frame
    void Update()
    {
        switch (_obstacleIndex)          //check which speed it should be
        {
            //Translate the trigger box depending on the movement speed of that obstacle
            case 0:
                if (this.CompareTag("MovingObstacleTrigger"))
                {
                    transform.Translate(Vector3.forward * -movementSpeed[0] * Time.deltaTime);
                }
                else
                {
                    transform.Translate(Vector3.forward * movementSpeed[0] * Time.deltaTime);
                }
                break;
            case 1:
                transform.Translate(Vector3.forward * movementSpeed[1] * Time.deltaTime);
                break;
            case 2:
                transform.Translate(Vector3.forward * -movementSpeed[2] * Time.deltaTime);
                break;
            case 3:
                transform.Translate(Vector3.forward * movementSpeed[2] * Time.deltaTime);
                break;
        }

    }
}
