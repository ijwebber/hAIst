using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource mainPlayer;
    [SerializeField] private AudioSource addPlayer;

    [SerializeField] private AudioClip intenseMain;
    [SerializeField] private AudioClip intenseAdd;

    [SerializeField] private AudioClip intenseMainIntro;
    [SerializeField] private AudioClip intenseAddIntro;
    

    public void PlayIntenseTheme() {
        mainPlayer.clip = intenseMain;
        addPlayer.clip = intenseAdd;

        if (!mainPlayer.isPlaying) {
            mainPlayer.Play();
        }

        if (!addPlayer.isPlaying) {
            addPlayer.Play();
        }
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
