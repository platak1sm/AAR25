using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioClip sceneAudio; 
    private AudioSource audioSource;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.volume = 1.0f;
        audioSource.spatialBlend = 0.0f; 
        audioSource.loop = true; 
        audioSource.playOnAwake = false;

        if (sceneAudio != null)
        {
            audioSource.clip = sceneAudio;
            audioSource.Play();
            Debug.Log($"AudioManager playing sceneAudio: {sceneAudio.name}");
        }
        else
        {
            Debug.LogWarning("AudioManager: sceneAudio not assigned in Inspector");
        }
    }

    public void PlayAudio(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
            Debug.Log($"AudioManager playing clip: {clip.name}");
        }
    }

    public void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("AudioManager stopped audio");
        }
    }
}