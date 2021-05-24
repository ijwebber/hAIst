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


    float velocity = 0.0f;

    public float acceleration = 0.1f;

    public float deceleration = 0.5f;

    int VelocityHash;
    // Start is called before the first frame update
    void Start()
    {  //set animator component
        animator = GetComponent<Animator>();

        //set player variable
        player = GameObject.Find("Timmy");

        //set controllers for sound and player
        soundController = GameObject.FindObjectOfType<SoundController>();
        playerController = GameObject.FindObjectOfType<PlayerController>();

        //set velocity hash to set value of player velocity in animator controller
        VelocityHash = Animator.StringToHash("Velocity");
    }

    // Update is called once per frame
    void Update()
    {

        // check the current player
        if(photonView.IsMine == false && PhotonNetwork.IsConnected == true){
            return;
        }

        // check for input keys and controls
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

        // check if player is down
        bool isdown = playerController.isDisabled;

        // check if any input has been made, if so, stop the player dancing
        bool stopDancing = isdown || forwardB || backB || leftB || rightB || upArrow || downArrow || leftArrow || rightArrow || spaceBar;


        // dance controls/inputs
        bool isMac = Input.GetKey(KeyCode.Alpha1);
        bool isLock = Input.GetKey(KeyCode.Alpha2);
        bool isFloorDance = Input.GetKey(KeyCode.Alpha3);
        bool isFlair = Input.GetKey(KeyCode.Alpha4);
        
        


        if((forwardB || backB || leftB || rightB || upArrow || downArrow || leftArrow || rightArrow) && velocity < 1.0f){
            velocity += Time.deltaTime * acceleration;
        }
        if(!(forwardB || backB || leftB || rightB || upArrow || downArrow || leftArrow || rightArrow) && velocity > 0.0f){
            velocity -= Time.deltaTime * deceleration;
        }
        if(!(forwardB || backB || leftB || rightB || upArrow || downArrow || leftArrow || rightArrow) && velocity < 0.0f){
            velocity = 0.0f;

        }

        
        // set the velocity parameter based on the velocity of the player in the animator controller
        animator.SetFloat(VelocityHash,velocity);

        // check is player is down and if so stop all actions and set down animation to true
        if(isdown){
            animator.SetBool("danceMacarena", false);
            animator.SetBool("danceLockHipHop", false);
            animator.SetBool("danceFloorDance", false);
            animator.SetBool("danceFlair", false);
            animator.SetBool("isDown",true);
        }
        else{

            // If player is not down, set down transition to false
            animator.SetBool("isDown",false);

            // play crouch animation
            if(Input.GetKey(KeyCode.Space)){
                animator.SetBool("isCrouched",true);
                iscrouched = true;
            }

            // stop playing crouch animation
            if(!(Input.GetKey(KeyCode.Space))){
                animator.SetBool("isCrouched",false);
                
            }

            // play crouch walk animation if both wasd/arrow keys are pressed and player is crouched
            if(iscrouched && (forwardB || backB || leftB || rightB || upArrow || downArrow || leftArrow || rightArrow)){
                animator.SetBool("isCrouchWalk",true);
              
            }


            // stop playing crouch walk animation if player not pressing any keys
            if(!(iscrouched && (forwardB || backB || leftB || rightB || upArrow || downArrow || leftArrow || rightArrow))){

                animator.SetBool("isCrouchWalk",false);
               
            }

            // play walk animation
            if(forwardB || backB || leftB || rightB || upArrow || downArrow || leftArrow || rightArrow){

                animator.SetBool("isWalking",true);
                
            }

            // stop playing walk animation

            if(!(forwardB || backB || leftB || rightB || upArrow || downArrow || leftArrow || rightArrow)){

                animator.SetBool("isWalking",false);
                
            }

            // DANCE animations
            // play various dance animations depending on which key is pressed

            if(stopDancing){
            animator.SetBool("danceMacarena", false);
            animator.SetBool("danceLockHipHop", false);
            animator.SetBool("danceFloorDance", false);
            animator.SetBool("danceFlair", false);
            }
            if(isMac) {
                animator.SetBool("danceMacarena", true);
            }if(isLock) {
                animator.SetBool("danceLockHipHop", true);
            }if(isFloorDance) {
                animator.SetBool("danceFloorDance", true);
            }if(isFlair) {
                animator.SetBool("danceFlair", true);
            }
        }

        if (GetComponent<PlayerMovement>().paused)
        {
            animator.SetBool("isWalking", false);
        }
        
    }

    public void footstep() {  
        if (this.photonView.IsMine) {
            int intensity = (int)(20 * (playerController.ninjaMultiplier));
            if (Input.GetKey(KeyCode.Space)) {
                // intensity = 0;
            } else if (Input.GetKey(KeyCode.LeftShift)) {
                intensity = (int)(30 * (playerController.ninjaMultiplier));
            }
            soundController.sendGrid(player.transform.position, intensity);
        }
    }
    
}
