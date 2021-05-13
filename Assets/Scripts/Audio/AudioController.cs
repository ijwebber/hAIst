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

    [SerializeField] private AudioClip[] DoorOpening;
    [SerializeField] private AudioClip[] GuardGrunt;
    [SerializeField] private AudioClip[] GuardHey;
    [SerializeField] private AudioClip[] KeypadPress;
    [SerializeField] private AudioClip KeypadSuccess;
    [SerializeField] private AudioClip KeypadFailure;
    [SerializeField] private AudioClip[] PlayerOof;
    [SerializeField] private AudioClip HelicopterSFX;


    private bool isNotNull;

    void Start() {
        if (intenseMain != null) {
            isNotNull = true;
        } else {
            isNotNull = false;
        }
    } 

    void Update() {
        float n = mainPlayer.volume + 0.05f;
        sfxPlayer1.volume = n;
        sfxPlayer2.volume = n;
    }

    public void PlayIntenseTheme() {
        if (isNotNull) {
            mainPlayer.Pause();
            addPlayer.Pause();
            mainPlayer.clip = intenseMain;
            addPlayer.clip = intenseAdd;

            introPlayer.volume = mainPlayer.volume;

            double startTime = AudioSettings.dspTime + 0.2;
            double introDuration = (double) introPlayer.clip.samples / introPlayer.clip.frequency;
            introPlayer.PlayScheduled(startTime);
            mainPlayer.PlayScheduled(introDuration + startTime);
            addPlayer.PlayScheduled(introDuration + startTime);

            mainPlayer.loop = true;
        }
    }

    public void EnableAdditional() {
        addPlayer.volume = mainPlayer.volume;
    }

    private void PlaySFX(AudioClip clip) {
        if (isNotNull) {
            if (alternate) {
                sfxPlayer1.clip = clip;
                sfxPlayer1.Play();
            } else {
                sfxPlayer2.clip = clip;
                sfxPlayer2.Play();
            }
            alternate = !alternate;
        }
    }

    public void PlayLowValue() {
        PlaySFX(lowValueSFX);
    }

    public void PlayHighValue() {
        PlaySFX(highValueSFX);
    }

    private AudioClip chooseOne(AudioClip[] list) {
        int n = Random.Range(0, list.Length);
        return list[n];
    }

    public void PlayDoorOpening() {
        PlaySFX(chooseOne(DoorOpening));
    }

    public void PlayGuardGrunt() {
        PlaySFX(chooseOne(GuardGrunt));
    }

    public void PlayGuardHey() {
        PlaySFX(chooseOne(GuardHey));
    }

    public void PlayKeypadPress() {
        PlaySFX(chooseOne(KeypadPress));
    }

    public void PlayKeypadSuccess() {
        PlaySFX(KeypadSuccess);
    }

    public void PlayKeypadFailure() {
        PlaySFX(KeypadFailure);
    }

    public void PlayPlayerOof() {
        PlaySFX(chooseOne(PlayerOof));
    }

    public void PlayHelicopter() {
        PlaySFX(HelicopterSFX);
    }
}
