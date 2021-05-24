using UnityEngine;
using Photon.Pun;

public class EndScreenAudioController : MonoBehaviourPun
{
    [SerializeField] private AudioSource introPlayer;
    [SerializeField] private AudioSource mainPlayer;
    [SerializeField] private AudioClip winMain;
    [SerializeField] private AudioClip winIntro;
    [SerializeField] private AudioClip lossMain;
    [SerializeField] private AudioClip lossIntro;

    private bool isNotNull = true;

    private bool introStarted = false;

    void Start() {
        /*
        if (winMain != null) {
            isNotNull = true;
        } else {
            isNotNull = false;
        }*/
    } 

    void Update() {
        if (!introStarted && introPlayer.isPlaying) {
            introStarted = true;
        }

        if (!mainPlayer.isPlaying && introStarted && !introPlayer.isPlaying) {
            mainPlayer.Play();
        }
    }
    

    // Play the themes with the intro
    public void PlayWin() {
        mainPlayer.clip = winMain;
        introPlayer.clip = winIntro;

        double startTime = AudioSettings.dspTime + 0.2;
        double introDuration = (double) introPlayer.clip.samples / introPlayer.clip.frequency;
        introPlayer.PlayScheduled(startTime);
        mainPlayer.PlayScheduled(introDuration + startTime);
    }

    public void PlayLoss() {
        mainPlayer.clip = lossMain;
        introPlayer.clip = lossIntro;

        double startTime = AudioSettings.dspTime + 0.2;
        double introDuration = (double) introPlayer.clip.samples / introPlayer.clip.frequency;
        introPlayer.PlayScheduled(startTime);
        mainPlayer.PlayScheduled(introDuration + startTime);
    }

}
