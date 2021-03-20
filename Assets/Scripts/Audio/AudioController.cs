using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip intenseTheme;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayIntenseTheme() {
        audioSource.clip = intenseTheme;
        PlayMusic();
    }

    public void PlayMusic() {
        audioSource.Play();
    }

    public void PauseMusic() {
        audioSource.Pause();
    }
}
