using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header ("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header ("Audio Clips")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;
    public AudioClip crash;
    public AudioClip engine;
    public AudioClip pickup;
    public AudioClip menuClick;

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
    }
    private void Start()
    {
        musicSource.clip = menuMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            if (musicSource.isPlaying && musicSource.clip != clip)
            {
                musicSource.Stop();
            }
            sfxSource.PlayOneShot(clip);
        }
    }

}
