using UnityEngine;

public class CameraController : MonoBehaviour
{
    //This script is used to control the camera behavior as to follow the player in a third person look
    private Transform player;

    public float yOffset = 5f;
    public float zOffset = -25f;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;    //get the player's position
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = new Vector3(player.position.x, player.position.y + yOffset, player.position.z - zOffset);  //position the camera behind the player's position after each frame
        }
    }
}
