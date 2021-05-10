using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource mainPlayer;
    [SerializeField] private AudioSource addPlayer;
    [SerializeField] private AudioSource introPlayer;

    [SerializeField] private AudioClip intenseMain;
    [SerializeField] private AudioClip intenseAdd;

    [SerializeField] private AudioClip intenseMainIntro;
    [SerializeField] private AudioClip intenseAddIntro;
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            PlayIntenseTheme();
        } 
    }

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

    /*public void PlayLowValue() {
        itemPlayer.clip = lowValueSFX;
        itemPlayer.Play();
    }

    public void PlayHighValue() {
        itemPlayer.clip = highValueSFX;
        itemPlayer.Play();
    }*/
}
