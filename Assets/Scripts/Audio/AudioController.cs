using UnityEngine;

public class AudioController : MonoBehaviour
{
    private FMOD.Studio.EventInstance intenseInstance;
    private FMOD.Studio.EventInstance stealthInstance;
    
    void Start() {
        intenseInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Game/Intense");
        stealthInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Game/Stealth");

        stealthInstance.start();
    }

    public void PlayIntenseTheme() {
        stealthInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        stealthInstance.release();
        intenseInstance.start();
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
