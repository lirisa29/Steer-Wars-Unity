using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton instance to ensure only one AudioManager exists
    public static AudioManager Instance { get; private set; }
    
    // Array of sounds defined in the Unity Inspector 
    public SoundEntry[] sounds; // Assigned in the Inspector
    private HashTable<string, AudioClip> soundTable;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        soundTable = new HashTable<string, AudioClip>();

        // Load all sounds from the array into the hash table
        foreach (var sound in sounds)
        {
            if (!string.IsNullOrEmpty(sound.name) && sound.clip != null)
            {
                soundTable.Add(sound.name, sound.clip);
            }
        }
    }

    private void Start()
    {
        PlayBackgroundMusic("BGM_Gameplay");
    }

    private void PlayBackgroundMusic(string name)
    {
        AudioClip clip = soundTable[name];
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySound(string name)
    {
        // Retrieve the clip from the sound table and play it
        AudioClip clip = soundTable[name];
        sfxSource.PlayOneShot(clip);
    }
    
    public void StopAllSounds()
    {
        sfxSource.Stop();
    }
}
