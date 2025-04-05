using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public AudioSource playSoundFX(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // Ensure soundFXObject is not null, instantiate a new AudioSource from the prefab
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        // Optionally, destroy the audio source after the clip is finished
        Destroy(audioSource.gameObject, audioSource.clip.length);

        return audioSource; // Return the AudioSource so it can be paused/unpaused
    }
}
