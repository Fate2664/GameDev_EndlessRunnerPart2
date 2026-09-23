using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //This script is used to control the camera behavior as to follow the player in a third person look
    private Transform player;
    private CinemachineBrain brain;

    public float yOffset = 5f;
    public float zOffset = -25f;

    void Start()
    {
        brain = GetComponent<CinemachineBrain>();
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }
    }

    void LateUpdate()
    {
        // A live Cinemachine camera owns both position and rotation, including
        // during blends. Keep the original follow behaviour for other scenes.
        if (brain != null && brain.isActiveAndEnabled && brain.ActiveVirtualCamera != null)
            return;

        if (player != null)
        {
            transform.position = new Vector3(player.position.x, player.position.y + yOffset, player.position.z - zOffset);  //position the camera behind the player's position after each frame
        }
    }
}
