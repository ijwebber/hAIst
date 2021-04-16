using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateBG : MonoBehaviour
{
    public Animator anim;
    public GameObject MapMenu;
    public bool animating = false;
    private GameObject screen;
    // Start is called before the first frame update
    void Start()
    {
        // anim.ResetTrigger("Zoom");
    }
    void Update() {
        if (MapMenu.activeInHierarchy) {
            animating = false;
        }
    }

    public void nextScreen() {
        animating = false;
        MapMenu.SetActive(false);
        if (screen != null) {
            screen.SetActive(true);
        }
    }
    public void zoom() {
        animating = true;
        anim.SetTrigger("Zoom");
    }

    public void unZoom(GameObject nextScreen) {
        animating = true;
        MapMenu.SetActive(false);
        screen = nextScreen;
        anim.ResetTrigger("Zoom");
        anim.enabled = true;
    }
    public void pauseAnimation() {
        animating = false;
        MapMenu.SetActive(true);
        anim.enabled = false;
    }
}
