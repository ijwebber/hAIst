using UnityEngine;

public class MenuAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioClip synthMenu;
    [SerializeField] private AudioClip orchMenu;

    void Start() {
        float rand = Random.Range(0,2);
        if (rand == 0) {
            musicPlayer.clip = synthMenu;
        } else {
            musicPlayer.clip = orchMenu;
        }

        musicPlayer.Play();
    }
}
