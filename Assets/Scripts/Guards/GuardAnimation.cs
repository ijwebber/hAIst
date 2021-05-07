using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GuardAnimation : MonoBehaviourPun
{
    // Start is called before the first frame update

    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine == false){
            return;
        }
        
        bool isdown = GetComponent<GuardMovement>().guardDisabled;

        if (GetComponent<GuardMovement>().sleepy && GetComponent<GuardMovement>().state != State.chase && GetComponent<GuardMovement>().agent.velocity.magnitude == 0)
        {
            animator.SetBool("Sleeping", true);
        } else { animator.SetBool("Sleeping", false); };

        if(isdown || GetComponent<GuardMovement>().agent.isStopped){animator.SetBool("isKnockedOut",true);}
        else{animator.SetBool("isKnockedOut",false);}

        
        


    }
}
