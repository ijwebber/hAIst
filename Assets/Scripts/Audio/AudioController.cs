using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip intenseTheme;
    public AudioClip alertedSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayIntenseTheme() {
        audioSource.clip = intenseTheme;

        if(!audioSource.isPlaying){
            PlayMusic();
        }
        //PlayMusic();
    }

    public void PlayMusic() {
        audioSource.Play();
    }

    public void PauseMusic() {
        audioSource.Pause();
    }
}
