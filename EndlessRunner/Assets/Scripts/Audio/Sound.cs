using UnityEngine;
using UnityEngine.Audio;

public enum SoundCategory
{
    Master,
    Music,
    Effects,
    Menu
}


[System.Serializable]
//this class represents a sound that can be played in the game
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;
    public bool loop;

    public SoundCategory category = SoundCategory.Master;

    [HideInInspector]
    public AudioSource source;
}
