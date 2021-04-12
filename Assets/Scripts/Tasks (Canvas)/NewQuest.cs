using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewQuest : MonoBehaviour
{
    [SerializeField] private Animator anim;
    public void newQuest() {
        anim.SetTrigger("newQuest");
    }

    public void endAnimation() {
        anim.ResetTrigger("newQuest");
    }
}
