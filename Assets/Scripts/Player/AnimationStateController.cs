using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AnimationStateController : MonoBehaviourPun
{

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {

        if(photonView.IsMine == false && PhotonNetwork.IsConnected == true){
            return;
        }

        bool forwardB = Input.GetKey(KeyCode.W);
        bool backB = Input.GetKey(KeyCode.S);
        bool leftB = Input.GetKey(KeyCode.A);
        bool rightB = Input.GetKey(KeyCode.D);
        bool iscrouched = animator.GetBool("isCrouched");

        if(Input.GetKey(KeyCode.Space)){
            animator.SetBool("isCrouched",true);
            iscrouched = true;
            }

        if(!(Input.GetKey(KeyCode.Space))){
            animator.SetBool("isCrouched",false);
        }

        if(iscrouched && (forwardB || backB || leftB || rightB)){
            animator.SetBool("isCrouchWalk",true);
        }

        if(!(iscrouched && (forwardB || backB || leftB || rightB))){
            animator.SetBool("isCrouchWalk",false);
        }

        if(Input.GetKey(KeyCode.W) ||Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)  || Input.GetKey(KeyCode.A)  ){
            animator.SetBool("isWalking",true);
        }

        if(!(forwardB || backB || leftB || rightB)){
            animator.SetBool("isWalking",false);
        }

        if(!(Input.GetKey(KeyCode.Space))){
            animator.SetBool("isCrouchWalk",false);
        }

        

        
    
        
    }


    
}
