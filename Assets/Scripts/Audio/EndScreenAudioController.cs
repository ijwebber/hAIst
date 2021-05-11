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

    private bool isNotNull;

    void Start() {
        if (winMain != null) {
            isNotNull = true;
        } else {
            isNotNull = false;
        }
    } 

    public void PlayWin() {
        if (isNotNull) {
            mainPlayer.clip = winMain;
            introPlayer.clip = winIntro;

            double startTime = AudioSettings.dspTime + 0.2;
            double introDuration = (double) introPlayer.clip.samples / introPlayer.clip.frequency;
            introPlayer.PlayScheduled(startTime);
            mainPlayer.PlayScheduled(introDuration + startTime);
        }
    }

    public void PlayLoss() {
        if (isNotNull) {
            mainPlayer.clip = lossMain;
            introPlayer.clip = lossIntro;

            double startTime = AudioSettings.dspTime + 0.2;
            double introDuration = (double) introPlayer.clip.samples / introPlayer.clip.frequency;
            introPlayer.PlayScheduled(startTime);
            mainPlayer.PlayScheduled(introDuration + startTime);
        }
    }

}
