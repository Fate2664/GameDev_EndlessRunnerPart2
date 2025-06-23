using UnityEngine;
using System;


public class AudioManager : MonoBehaviour
{
    [Header ("Audio Clips")]
    [SerializeField] Sound[] sounds;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;  // Singleton instance
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instances
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
        PlaySFX("MenuMusic");  // Play the menu music on start

    }

    // Method to play a specific sound effect
    public void PlaySFX(string name)
    {
        if (Array.Exists(sounds, sound => sound.name == name && sound.source.isPlaying))
        {
            Array.Find(sounds, sound => sound.name == name)?.source.Stop();
        }
        if (Array.Find(sounds, sound => sound.name == name) == null)
            return;

        Array.Find(sounds, sound => sound.name == name)?.source.Play();
    }

    // Method to stop a specific sound
    public void StopSFX(string name)
    {
        Sound sound = Array.Find(sounds, s => s.name == name);
        if (sound != null && sound.source.isPlaying)
        {
            sound.source.Stop();
        }
    }

}
