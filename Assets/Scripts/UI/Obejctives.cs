using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obejctives : MonoBehaviour
{
    public bool objectivesToggle = false;
    [SerializeField] Animator anim;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            objectivesToggle = !objectivesToggle;
            if (objectivesToggle) {
                anim.SetTrigger("OpenObjectives");
                anim.ResetTrigger("NewQuest");
            } else {
                anim.ResetTrigger("OpenObjectives");
                anim.enabled = true;
            }
        }
    }

    public void pauseAnimation() {
        if (objectivesToggle) {
            anim.enabled = false;
            anim.ResetTrigger("NewQuest");
        }
    }

    public void newQuest() {
        if (!objectivesToggle) {
            anim.SetTrigger("NewQuest");
        }
    }
}
