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

        if(isdown){animator.SetBool("isKnockedOut",true);}
        else{animator.SetBool("isKnockedOut",false);}
        
    }
}
