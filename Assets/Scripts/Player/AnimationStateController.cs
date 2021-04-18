using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AnimationStateController : MonoBehaviourPun
{

    Animator animator;
    GameObject player;
    SoundController soundController;
    PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Timmy");
        soundController = GameObject.FindObjectOfType<SoundController>();
        playerController = GameObject.FindObjectOfType<PlayerController>();
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

        bool upArrow = Input.GetKey(KeyCode.UpArrow);
        bool downArrow = Input.GetKey(KeyCode.DownArrow);
        bool leftArrow = Input.GetKey(KeyCode.LeftArrow);
        bool rightArrow = Input.GetKey(KeyCode.RightArrow);

        bool iscrouched = animator.GetBool("isCrouched");
        bool isdown = GetComponent<PlayerMovement>().disabled;

        // do footsteps
        AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);
        float currentFrame = animatorState.normalizedTime;
        if (currentFrame > 1) {
            currentFrame-=Mathf.FloorToInt(currentFrame);
        }
        if (animator.GetBool("isWalking") && (currentFrame >=0.2f && currentFrame <= 0.3f || currentFrame >= .7f && currentFrame <= .8f)) {
            int intensity = 20 - (playerController.upgrades.ninja*2);
            if (Input.GetKey(KeyCode.LeftShift)) {
                intensity = 30 - (playerController.upgrades.ninja*2);
            } else if (Input.GetKey(KeyCode.Space)) {
                intensity = 0;
            }
            soundController.sendGrid(player.transform.position, intensity);
        }

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

            if(iscrouched && (forwardB || backB || leftB || rightB || upArrow || downArrow || leftArrow || rightArrow)){
                animator.SetBool("isCrouchWalk",true);
            }

            if(!(iscrouched && (forwardB || backB || leftB || rightB || upArrow || downArrow || leftArrow || rightArrow))){
                animator.SetBool("isCrouchWalk",false);
            }

            if(forwardB || backB || leftB || rightB || upArrow || downArrow || leftArrow || rightArrow){
                animator.SetBool("isWalking",true);
            }

            if(!(forwardB || backB || leftB || rightB || upArrow || downArrow || leftArrow || rightArrow)){
                animator.SetBool("isWalking",false);
            }

            if(!(Input.GetKey(KeyCode.Space))){
                animator.SetBool("isCrouchWalk",false);
            }


        }

        if (GetComponent<PlayerMovement>().paused)
        {
            animator.SetBool("isWalking", false);
        }
        
    }
    
}
