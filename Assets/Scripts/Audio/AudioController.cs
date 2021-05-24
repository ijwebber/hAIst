using UnityEngine;

public class AudioController : MonoBehaviour
{

    // Getting AudioSources
    [SerializeField] private AudioSource mainPlayer;
    [SerializeField] private AudioSource addPlayer;
    [SerializeField] private AudioSource introPlayer;

    [SerializeField] private AudioSource sfxPlayer1;
    [SerializeField] private AudioSource sfxPlayer2;

    // Used to alternate between the two sfx players
    private bool alternate = false;

    // Getting AudioClips
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

    // Checks if the audio clip exists
    private bool isNotNull;
    // True once the intense intro has been played
    private bool introStarted = false;

    void Start() {
        // Checks if the audio is filled (for josh)
        if (intenseMain != null) {
            isNotNull = true;
        } else {
            isNotNull = false;
        }
    } 

    void Update() {

        // Corrects volume if it has been changed (future proofing for a volume slider)
        float n = mainPlayer.volume + 0.05f;
        sfxPlayer1.volume = n;
        sfxPlayer2.volume = n;


        // Check if the intro has started and hasn't already been set to true.
        if (!introStarted && introPlayer.isPlaying) {
            introStarted = true;
        }

        // Make sure the player never stops for no reason
        if (!mainPlayer.isPlaying && introStarted && !introPlayer.isPlaying) {
            mainPlayer.Play();
            addPlayer.Play();
        }


        // Testing controls
        if (Input.GetKeyDown(KeyCode.L)) {
            PlayIntenseTheme();
        }

        if (Input.GetKeyDown(KeyCode.Semicolon)) {
            EnableAdditional();
        }
    }
    

    // Start the intense theme loop with the intro section
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

    // Enable the additional music on top of the main player
    public void EnableAdditional() {
        addPlayer.volume = mainPlayer.volume;
    }

    // Play a sound effect using this, alternates between two players so that two can play at the same time.
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


    // Choose a random sound effect from a list
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
}
