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
    void FixedUpdate()
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

        bool spaceBar = Input.GetKey(KeyCode.Space);

        bool iscrouched = animator.GetBool("isCrouched");
        bool isdown = GetComponent<PlayerMovement>().disabled;

        bool stopDancing = forwardB || backB || leftB || rightB || upArrow || downArrow || leftArrow || rightArrow || spaceBar;
        
        bool isDancing = handleDancing(stopDancing);


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

            if(isDancing) {
                animator.SetBool("isDancing", true);
            } else if (animator.GetBool("isDancing")) {
                animator.SetBool("isDancing", false);
            }


        }

        if (GetComponent<PlayerMovement>().paused)
        {
            animator.SetBool("isWalking", false);
        }
        
    }

    public void footstep() {
        int intensity = (int)(20 * (1 - .5*playerController.upgrades.ninja));
        if (Input.GetKey(KeyCode.Space)) {
            intensity = 0;
        } else if (Input.GetKey(KeyCode.LeftShift)) {
            intensity = (int)(30 * (1 - .5*playerController.upgrades.ninja));
        }
        soundController.sendGrid(player.transform.position, intensity);
    }

    bool handleDancing(bool stopDancing) {
        bool currentlyDancing = animator.GetBool("isDancing");

        bool isMac = !stopDancing && Input.GetKey(KeyCode.Alpha1);
        bool isLock = !stopDancing && Input.GetKey(KeyCode.Alpha2);
        bool isFloorDance = !stopDancing && Input.GetKey(KeyCode.Alpha3);
        bool isFlair = !stopDancing && Input.GetKey(KeyCode.Alpha4);

        bool isDancing = true;

        if (isMac) {
            animator.SetBool("danceMacarena", true);
            animator.SetBool("danceLockHipHop", false);
            animator.SetBool("danceFloorDance", false);
            animator.SetBool("danceFlair", false);
        } else if (isLock) {
            animator.SetBool("danceMacarena", false);
            animator.SetBool("danceLockHipHop", true);
            animator.SetBool("danceFloorDance", false);
            animator.SetBool("danceFlair", false);
        } else if (isFloorDance) {
            animator.SetBool("danceMacarena", false);
            animator.SetBool("danceLockHipHop", false);
            animator.SetBool("danceFloorDance", true);
            animator.SetBool("danceFlair", false);
        } else if (isFlair) {
            animator.SetBool("danceMacarena", false);
            animator.SetBool("danceLockHipHop", false);
            animator.SetBool("danceFloorDance", false);
            animator.SetBool("danceFlair", true);
        } else if (stopDancing) {
            animator.SetBool("danceMacarena", false);
            animator.SetBool("danceLockHipHop", false);
            animator.SetBool("danceFloorDance", false);
            animator.SetBool("danceFlair", false);
            isDancing = false;
        }

        return isDancing;
    }
    
}
