using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource musicPlayer;
    public AudioSource itemPlayer;
    [SerializeField] private AudioClip intenseTheme;
    [SerializeField] private AudioClip alertedSound;
    [SerializeField] private AudioClip lowValueSFX;
    [SerializeField] private AudioClip highValueSFX;

    public void PlayIntenseTheme() {
        musicPlayer.clip = intenseTheme;

        if(!musicPlayer.isPlaying){
            PlayMusic();
        }
    }

    public void PlayMusic() {
        musicPlayer.Play();
    }

    public void PauseMusic() {
        musicPlayer.Pause();
    }

    public void PlayLowValue() {
        itemPlayer.clip = lowValueSFX;
        itemPlayer.Play();
    }

    public void PlayHighValue() {
        itemPlayer.clip = highValueSFX;
        itemPlayer.Play();
    }
}
