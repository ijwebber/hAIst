using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource mainPlayer;
    [SerializeField] private AudioSource addPlayer;
    [SerializeField] private AudioSource introPlayer;

    [SerializeField] private AudioSource sfxPlayer1;
    [SerializeField] private AudioSource sfxPlayer2;
    private bool alternate = false;

    [SerializeField] private AudioClip intenseMain;
    [SerializeField] private AudioClip intenseAdd;

    [SerializeField] private AudioClip intenseMainIntro;
    [SerializeField] private AudioClip intenseAddIntro;

    [SerializeField] private AudioClip lowValueSFX;
    [SerializeField] private AudioClip highValueSFX;

    public void PlayIntenseTheme() {
        mainPlayer.Pause();
        addPlayer.Pause();
        mainPlayer.clip = intenseMain;
        addPlayer.clip = intenseAdd;

        double startTime = AudioSettings.dspTime + 0.2;
        double introDuration = (double) introPlayer.clip.samples / introPlayer.clip.frequency;
        introPlayer.PlayScheduled(startTime);
        mainPlayer.PlayScheduled(introDuration + startTime);
        addPlayer.PlayScheduled(introDuration + startTime);
    }

    public void EnableAdditional() {
        addPlayer.volume = mainPlayer.volume;
    }

    private void PlaySFX(AudioClip clip) {
        if (alternate) {
            sfxPlayer1.clip = clip;
            sfxPlayer1.Play();
        } else {
            sfxPlayer2.clip = clip;
            sfxPlayer2.Play();
        }
        alternate = !alternate;
    }

    public void PlayLowValue() {
        PlaySFX(lowValueSFX);
    }

    public void PlayHighValue() {
        PlaySFX(highValueSFX);
    }
}
