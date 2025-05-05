using UnityEngine;

[CreateAssetMenu( menuName = "Obstacles/ObstacleConfig")]
public class ObstacleConfig : ScriptableObject
{
    public enum ObstacleType { TowardPlayer, PastPlayer, Traffic }

    public ObstacleType obstacleType;

    public int movementSpeedIndex;
    public Vector3 spawnOffset;
    public Vector3 triggerOffset;
    public bool faceBackward = false;
    public GameObject triggerPrefab;
    public GameObject indicatorPrefab;
    public float lifespan = 5f;

}
