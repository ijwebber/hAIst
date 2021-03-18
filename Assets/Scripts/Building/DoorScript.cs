using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public Animator anim;
    // Start is called before the first frame update
    void Awake()
    {
        anim.ResetTrigger("OpenDoor");
    }

    public void OpenDoor() {
        anim.SetTrigger("OpenDoor");
    }

    public void CloseDoor() {
        anim.ResetTrigger("OpenDoor");
        anim.enabled = true;
    }

    void pauseAnimationEvent() {
        anim.enabled = false;
    }
}
