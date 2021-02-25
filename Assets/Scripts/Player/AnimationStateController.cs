using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AnimationStateController : MonoBehaviourPun
{

    Animator animator;
    GameObject player;
    SoundController soundController;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Timmy");
        soundController = GameObject.FindObjectOfType<SoundController>();
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
        bool isdown = GetComponent<PlayerMovement>().disabled;
        AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);

        float currentFrame = animatorState.normalizedTime;
        if (currentFrame > 1) {
            currentFrame-=Mathf.FloorToInt(currentFrame);
        }
        if (animator.GetBool("isWalking") && (currentFrame >=0.2f && currentFrame <= 0.3f || currentFrame >= .7f && currentFrame <= .8f)) {
            Debug.Log("frame " + currentFrame);
            int intensity = 20;
            if (Input.GetKey(KeyCode.LeftShift)) {
                intensity = 30;
            }
            soundController.sendGrid(player.transform.position, intensity);
            // this.photonView.RPC("updateGrid", RpcTarget.All, player.transform.position.x, player.transform.position.y, player.transform.position.z, 30);
        }
        // if (currentFrame == 1) {
        //     this.photonView.RPC("updateGrid", RpcTarget.All, player.transform.position.x, player.transform.position.y, player.transform.position.z, 30);
        // }

        if(isdown){
            animator.SetBool("isDown",true);
        }
        else{
            animator.SetBool("isDown",false);

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
    
}
