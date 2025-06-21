using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using Unity.Services.CloudSave;
using Unity.Services.Leaderboards;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;

public class DistanceManager : MonoBehaviour
{
    //This script manages the distance covered by the player

    public UnityEvent BossDistanceReached;
    public UnityEvent BossExitDistanceReached;

    public BossSpawner bossSpawner;

    private float startZ;
    private float actDistance = 0;
    private float _distanceCovered;
    public float distanceCovered { get { return _distanceCovered; } }
    private float _virtualDistanceCovered;
    public float virtualDistanceCovered { get { return _virtualDistanceCovered; } }

    public static DistanceManager Instance { get; private set; }

    public List<TextMeshProUGUI> distanceValue;

    private bool bossSpawnEventFired = false;
    private bool bossDespawnEventFired = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;  //Singleton instance
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);  //Destroy duplicate instances
        }
    }


    void Start()
    {
        startZ = transform.position.z;
        if (BossDistanceReached == null) BossDistanceReached = new UnityEvent();
        if (BossExitDistanceReached == null) BossExitDistanceReached = new UnityEvent();
    }

    void Update()
    {
        //We get the actual z value that the player covers and increment the distance amount after every 100 units
        actDistance = -(transform.position.z - startZ);
        if (actDistance >= 100)
        {
            _virtualDistanceCovered++;
            _distanceCovered++;
            startZ = transform.position.z;
        }
        if (distanceValue != null)
        {

            for (int i = 0; i < distanceValue.Count; i++)
            {
                if (distanceValue[i] != null)
                {
                    distanceValue[i].text = _distanceCovered.ToString("0");
                }
               
            }
            
        }

        if (!bossSpawnEventFired && _virtualDistanceCovered > bossSpawner.DistanceToSpawn)
        {
            BossDistanceReached.Invoke();  //Invoke the event when the boss distance is reached
            bossSpawnEventFired = true;  //Set the flag to true to avoid multiple invocations
        }
        if (!bossSpawner.bossDefeated && bossSpawner.isBossActive)
        {
            bossSpawner.SpawnBossObstacles();  //Spawn the boss obstacles if the boss is active
        }
        if (!bossDespawnEventFired && bossSpawner.isBossActive && _virtualDistanceCovered - bossSpawner.DistanceToSpawn >= bossSpawner.DistanceToDespawn)
        {
            BossExitDistanceReached.Invoke();  //Invoke the event when the boss exit distance is reached
            bossDespawnEventFired = true;  //Set the flag to true to avoid multiple invocations
        }
    }

    public void ResetVirtualDistance()
    {
        _virtualDistanceCovered = 0;
        bossSpawnEventFired = false;
        bossDespawnEventFired = false;
    }

    public async void SaveDistance()
    {
        var data = new Dictionary<string, object> { { "distance_covered", _distanceCovered } };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);  //Save the score to the cloud

        try
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync("EndlessRunner_DistanceLeaderboard", _distanceCovered);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error submitting distance to leaderboard: " + e.Message);
        }
    }

    public async void LoadDistance()
    {
        var cloudData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "distance_covered" });  //Load the score from the cloud
        if (cloudData.TryGetValue("distance_covered", out var cloudDistance))
        {
            _distanceCovered = float.Parse(cloudDistance.ToString());
            foreach (var text in distanceValue)
            {
                text.text = _distanceCovered.ToString("0");  //Update the UI with the loaded score
            }
        }

    }

    public void ResetDistance()
    {
        _distanceCovered = 0;
        _virtualDistanceCovered = 0;
        startZ = transform.position.z;

        foreach (var text in distanceValue)
        {
            if (text != null)
                text.text = "0";
        }
    }
}
